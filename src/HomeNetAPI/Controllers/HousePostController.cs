using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.Models;
using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using HomeNetAPI.Services;
using HomeNetAPI.ViewModels;
using SkiaSharp;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    //Route should be House/{houseId}/HouseControls
    [Route("/[controller]/[action]")]
    public class HousePostController : Controller
    {
        private UserManager<User> userManager;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private IHousePostRepository housePostRepository;
        private IHouseMemberRepository houseMemberRepository;
        private IHouseRepository houseRepository;
        private IImageProcessor imageProcessor;


        public HousePostController(UserManager<User> userManager, IHousePostRepository housePostRepository, IHouseRepository houseRepository, IImageProcessor imageProcessor)
        {
            this.userManager = userManager;
            this.housePostRepository = housePostRepository;
            this.houseRepository = houseRepository;
            this.imageProcessor = imageProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetHousePosts([FromQuery] int houseId, [FromQuery] String clientCode)
        {
            ListResponse<HousePost> response = new ListResponse<HousePost>();
            try
            {
                if (clientCode == androidClient)
                {
                    if (houseId < 0)
                    {
                        response.DidError = true;
                        response.Message = "Please send valid house identitifaction";
                        response.Model = null;
                        return NotFound(response);
                    }

                    var results = await Task.Run(() =>
                    {
                        return housePostRepository.GetHousePosts(houseId);
                    });
                    if (results != null)
                    {
                        response.DidError = false;
                        response.Message = "House posts under the following house have been found";
                        response.Model = results;
                        return Ok(response);
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No posts have been found for the house";
                        response.Model = null;
                        return NotFound(response);
                    }
                } else
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpPost]
        //A user may pass a photo or video - see how to handle this
        public async Task<IActionResult> AddHousePost([FromQuery] int houseID, [FromForm] String title, [FromForm] String postText, [FromForm] String datePosted, [FromForm] String location, [FromQuery] String clientCode, [FromForm] IFormFile file, [FromForm] String userEmailAddress)
        {
            SingleResponse<HousePost> response = new SingleResponse<HousePost>();
            if (clientCode == androidClient)
            {
                try
                {
                    if (postText != null && userEmailAddress != null)
                    {
                        var user = await userManager.FindByEmailAsync(userEmailAddress);
                        if (user != null)
                        {
                            HousePost housePost = new HousePost()
                            {
                                HousePostID = 0,
                                Title = title,
                                PostText = postText,
                                DatePosted = DateTime.Now.ToString(),
                                Location = location

                            };

                            var selectedHouse = await Task.Run(() =>
                            {
                                return houseRepository.GetHouse(houseID);
                            });

                            if (selectedHouse == null)
                            {
                                response.DidError = true;
                                response.Message = "No house was found with the supplied house information";
                                response.Model = null;
                                return NotFound(response);
                            }

                            List<HouseMember> houseMemberships = await Task.Run(() =>
                            {
                                return houseMemberRepository.GetHouseMember(user.Id);
                            });

                            if (houseMemberships == null)
                            {
                                response.DidError = true;
                                response.Message = "You are not registered for any houses. Please register for a house, and try again";
                                response.Model = null;
                                return NotFound(response);
                            }

                            foreach (HouseMember membership in houseMemberships)
                            {
                                if (membership.HouseID == selectedHouse.HouseID)
                                {
                                    housePost.HouseMember = membership;
                                    housePost.HouseMemberID = membership.HouseMemberID;
                                }
                            }

                            if (housePost.HouseMember == null)
                            {
                                response.DidError = true;
                                response.Message = "You are not subscribed to the selected house. Please try again";
                                response.Model = null;
                                return NotFound(response);
                            }

                            if (file != null)
                            {
                                //Check the file and then upload - if it is a picture it must be jpg or png
                                if (file.Length > 2.5e+7)
                                {
                                    response.DidError = true;
                                    response.Message = "The uploaded file (picture or video) cannot be greater than 10MB. Please try another file";
                                    response.Model = null;
                                    response.Model = housePost;
                                    return BadRequest(response);
                                }

                                if (file.ContentType != "image/jpeg" || file.ContentType != "video/*")
                                {
                                    response.DidError = true;
                                    response.Message = "HomeNET only accepts images and/or videos less than 25MB";
                                    response.Model = housePost;
                                    return BadRequest(response);
                                }

                                String directory = $"C:/HomeNET/Houses/{selectedHouse.HouseID}/";
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }

                                String finalDestination = directory + file.FileName;
                                using (var stream = new FileStream(finalDestination, FileMode.Create, FileAccess.ReadWrite))
                                {
                                    await file.CopyToAsync(stream);
                                    housePost.MediaResource = finalDestination;

                                }
                                    if (file.ContentType == "image/jpeg")
                                    {
                                        SKBitmap bitmap = SKBitmap.Decode(finalDestination);
                                        String resizedLocation = imageProcessor.ResizeImage(bitmap, finalDestination, file.FileName);
                                        housePost.ResizedMediaResource = resizedLocation;
                                        var postResult = await Task.Run(() =>
                                       {
                                           return housePostRepository.AddHousePost(housePost);
                                       });
                                        if (postResult != null)
                                        {
                                            response.DidError = false;
                                            response.Message = "House Post and Image saved successfully!";
                                            response.Model = postResult;
                                            return Ok(response);
                                        } else
                                        {
                                            response.DidError = true;
                                            response.Message = "Something went wrong with saving the post and image data. Please try again";
                                            System.IO.File.Delete(finalDestination);
                                            System.IO.File.Delete(resizedLocation);
                                            housePost.MediaResource = "";
                                            housePost.ResizedMediaResource = "";
                                            response.Model = housePost;
                                            return BadRequest(response);
                                        }

                                    }

                                    var finalResult = await Task.Run(() =>
                                    {
                                        return housePostRepository.AddHousePost(housePost);
                                    });
                                    if (finalResult != null)
                                    {
                                        response.DidError = false;
                                        response.Message = "House Post and media saved successfully!";
                                        response.Model = finalResult;
                                        return Ok(response);
                                    } else
                                    {
                                        response.DidError = true;
                                        System.IO.File.Delete(finalDestination); //Delete the file
                                        response.Message = "House post and media could not be saved!";
                                        response.Model = finalResult;
                                        return BadRequest(response);
                                    }

                                

                            } else
                            {
                                var finalResult = await Task.Run(() =>
                                {
                                    return housePostRepository.AddHousePost(housePost);
                                });
                                if (finalResult != null)
                                {
                                    response.DidError = false;
                                    response.Message = "Your post has been saved! ";
                                    response.Model = finalResult;
                                    return Ok(response);
                                } else
                                {
                                    response.DidError = true;
                                    response.Message = "Something went wrong with saving the post. Please try again";
                                    response.Model = housePost;
                                    return BadRequest(response);
                                }
                            }
                        } else
                        {
                            response.DidError = true;
                            response.Message = "No user was found with the given email address. Please check again";
                            response.Model = null;
                            return NotFound(response);
                        }

                    } else
                    {
                        response.DidError = true;
                        response.Message = "Please provide a post text is required";
                        response.Model = null;
                        return BadRequest(response);
                    }
                } catch (Exception error)
                {
                    response.DidError = true;
                    response.Message = error.Message;
                    response.Model = null;
                    return BadRequest(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide valid client credentials to the server";
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHousePost([FromQuery] String clientCode, [FromQuery] int housePostId)
        {
            SingleResponse<HousePost> response = new SingleResponse<HousePost>();
            try
            {
                if (clientCode == androidClient)
                {
                    var housePost = await Task.Run(() =>
                    {
                        return housePostRepository.GetHousePosts(housePostId);
                    });
                    if (housePost == null)
                    {
                        response.DidError = true;
                        response.Message = "House Post was not found on the system. Please try again";
                        response.Model = null;
                        return NotFound(response);
                    }

                    var deleteResult = await Task.Run(() =>
                    {
                        return housePostRepository.DeleteHousePost(housePostId);
                    });
                    if (deleteResult == null)
                    {
                        response.DidError = true;
                        response.Message = "Error deleting post. Please refresh feed";
                        response.Model = null;
                        return BadRequest(response);
                    }
                    else
                    {
                        response.DidError = false;
                        response.Message = "House Post Deleted Successfully!";
                        response.Model = deleteResult;
                        return Ok(response);
                    }
                } else
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }


            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> FlagHousePost([FromQuery] String clientCode, [FromQuery] int housePostID, [FromForm] String flagReason, [FromForm] String emailAddress)
        {
            SingleResponse<HousePostFlag> response = new SingleResponse<HousePostFlag>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                //Get the House Post
                var housePost = await Task.Run(() =>
                {
                    return housePostRepository.GetHousePost(housePostID);
                });
                if (housePost == null)
                {
                    response.DidError = true;
                    response.Message = "No house post was found with the given information. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                //If House post is null, attempt to find the linking membership information on the server
                if (housePost.HouseMember == null)
                {
                    var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                    if (selectedUser == null)
                    {
                        response.DidError = true;
                        response.Message = "User data is not registered onto the system. Please try again later";
                        response.Model = null;
                        return NotFound(response);
                    }

                    var houseMemberships = await Task.Run(() =>
                    {
                        return houseMemberRepository.GetHouseMember(selectedUser.Id);
                    });
                    if (houseMemberships == null)
                    {
                        response.DidError = true;
                        response.Message = "No house memberships found. Please join a house";
                        response.Model = null;
                        return NotFound(response);
                    }

                    foreach (HouseMember membership in houseMemberships)
                    {
                        if (membership.HouseMemberID == housePost.HouseMemberID)
                        {
                            housePost.HouseMember = membership;
                        }
                    }

                    if (housePost.HouseMember == null)
                    {
                        response.DidError = true;
                        response.Message = "No house membership could be found with the post. Please try again";
                        response.Model = null;
                        return BadRequest(response);
                    }

                }

                HousePostFlag flaggedPost = new HousePostFlag()
                {
                    HousePostFlagID = 0,
                    DateFlagged = DateTime.Now.ToString(),
                    IsDeleted = 0,
                    IsFlagged = 1,
                    HousePost = housePost,
                    HousePostID = housePost.HousePostID,
                    HouseMemberID = housePost.HouseMemberID,
                    Message = flagReason
                };

                var flagPostResult = await Task.Run(() =>
                {
                    return housePostRepository.FlagHousePost(flaggedPost);
                });
                if (flagPostResult == null)
                {
                    response.DidError = true;
                    response.Message = "Error occurred while flagging post. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "House post flagged successfully!";
                    response.Model = null;
                    return Ok(response);
                }


        } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message;
                response.Model = null;
                return BadRequest(response);
            }
        }

    }
}
