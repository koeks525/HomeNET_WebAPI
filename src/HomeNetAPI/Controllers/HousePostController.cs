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


        public HousePostController(UserManager<User> userManager, IHousePostRepository housePostRepository, IHouseRepository houseRepository, IImageProcessor imageProcessor, IHouseMemberRepository houseMemberRepository)
        {
            this.houseMemberRepository = houseMemberRepository;
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
        ///Add a new photo for a selected user in a selected house. 
        public async Task<IActionResult> AddHousePost([FromQuery] int houseID, [FromForm] String emailAddress, [FromForm] String postText, [FromForm] String location, [FromQuery] String clientCode, [FromForm] IFormFile file)
        {
            SingleResponse<HousePost> response = new SingleResponse<HousePost>();
            try
            {
                if (androidClient != clientCode)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                if (postText == null)
                {
                    response.DidError = true;
                    response.Message = "Please send valid post data to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the supplied data";
                    response.Model = null;
                    return NotFound(response);
                }

                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(houseID);
                });
                if (selectedHouse ==  null)
                {
                    response.DidError = true;
                    response.Message = "No house was found with the given ID";
                    response.Model = null;
                    return NotFound(response);
                }

                HousePost newPost = new HousePost();
                newPost.PostText = postText;
                newPost.DatePosted = DateTime.Now.ToString();
                if (location != null)
                {
                    newPost.Location = location;
                }
                //UserID and HouseID must align.
                var houseMembership = await Task.Run(() =>
                {
                    return houseMemberRepository.GetMembership(selectedHouse.HouseID, selectedUser.Id);
                });
                if (houseMembership == null)
                {
                    response.DidError = true;
                    response.Message = "The selected user does not appear to have a membership with the house";
                    response.Model = null;
                    return NotFound(response);
                }
                newPost.HouseMemberID = houseMembership.HouseMemberID;
                newPost.IsDeleted = 0;
                newPost.IsFlagged = 0;
                if (location != null)
                {
                    newPost.Location = location;
                }
                if (file != null)
                {//Add a new image 
                    String finalFileName = "";
                    String directory = $"C:/HomeNET/Houses/{selectedHouse.HouseID}/Posts";
                    if (file.FileName.Contains(":"))
                    {
                        finalFileName = file.FileName.Replace(":", "_");
                    } else
                    {
                        finalFileName = file.FileName;
                    }
                    finalFileName = GenerateRandomString() + finalFileName;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    using (var fileStream = new FileStream(directory + "/"+finalFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        newPost.MediaResource = directory + "/" + finalFileName;
                        await file.CopyToAsync(fileStream);
                        var result = await Task.Run(() =>
                        {
                            return housePostRepository.AddHousePost(newPost);

                        });
                        if (result != null)
                        {
                            response.DidError = false;
                            response.Model = result;
                            response.Message = "New post created successfully!";
                            return Ok(response);
                        } else
                        {
                            response.DidError = true;
                            response.Message = "Error Adding new Post";
                            response.Model = null;
                            return BadRequest(response);
                        }
                    }

                } else
                {
                    var addResult = await Task.Run(() =>
                    {
                        return housePostRepository.AddHousePost(newPost);
                    });
                    if (addResult != null)
                    {
                        response.DidError = false;
                        response.Message = "New post added successfully!";
                        response.Model = addResult;
                        return Ok(response);
                    } else
                    {
                        response.DidError = false;
                        response.Message = "Error adding house post. Please try again";
                        response.Model = null;
                        return BadRequest(response);
                    }
                }

            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
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

        [HttpGet] 
        public async Task<IActionResult> GetAllHousePosts([FromQuery] String emailAddress, [FromQuery] String clientCode)
        {
            ListResponse<HousePostViewModel> response = new ListResponse<HousePostViewModel>();
            List<House> subscribedHouses = new List<House>();
            List<HouseMember> houseMembers = new List<HouseMember>();
            List<HousePost> housePosts = new List<HousePost>();
            List<HousePostViewModel> finalPostList = new List<HousePostViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var currentUser = await userManager.FindByEmailAsync(emailAddress);
                if (currentUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the given credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                var userMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMember(currentUser.Id);
                });
                if (userMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "The current user is not subscribed to any houses.";
                    response.Model = null;
                    return BadRequest(response);
                }
                foreach (HouseMember subscription in userMemberships)
                {
                    var house = await Task.Run(() =>
                    {
                        return houseRepository.GetHouse(subscription.HouseID);
                    });
                    if (house != null)
                    {
                        subscribedHouses.Add(house);
                    }
                }
                if (subscribedHouses.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No houses were found for the subscriptions";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (House house in subscribedHouses)
                {
                    var memberships = await Task.Run(() =>
                    {
                        return houseMemberRepository.GetHouseMemberships(house.HouseID);
                    });
                    if (memberships != null)
                    {
                        foreach (HouseMember member in memberships)
                        {
                            houseMembers.Add(member);
                        }
                    }
                }
                if (houseMembers == null)
                {
                    response.DidError = true;
                    response.Message = "No house members were found for subscribed houses";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HouseMember resultHouseMembers in houseMembers)
                {
                    var posts = await Task.Run(() =>
                    {
                        return housePostRepository.GetHousePosts(resultHouseMembers.HouseMemberID);
                    });
                    if (posts != null)
                    {
                        foreach(HousePost post in posts)
                        {
                            housePosts.Add(post);
                        }
                    }
                }
                if (housePosts.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No posts were found";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HousePost post in housePosts)
                {
                    var model = new HousePostViewModel()
                    {
                        DatePosted = post.DatePosted,
                        PostText = post.PostText,
                        HouseMemberID = post.HouseMemberID,
                        MediaResource = post.MediaResource,
                        IsDeleted = post.IsDeleted,
                        IsFlagged = post.IsFlagged,
                        HousePostID = post.HousePostID

                    };
                    var houseMember = houseMembers.First(i => i.HouseMemberID == post.HouseMemberID);
                    if (houseMember != null)
                    {
                        var finalUser = await userManager.FindByIdAsync(Convert.ToString(houseMember.UserID));
                        if (finalUser != null)
                        {
                            model.Name = finalUser.Name;
                            model.Surname = finalUser.Surname;
                            model.EmailAddress = finalUser.Email;
                            finalPostList.Add(model);
                        }
                    }
                }
                if (finalPostList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No posts were found";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Herewith the house posts";
                    response.Model = finalPostList;
                    return Ok(response);
                }


            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }
        }

        private String GenerateRandomString()
        {
            Random random = new Random();
            String finalString = "";
            for (int a = 0; a < 10; a++)
            {
                finalString += Convert.ToString(random.Next(1, 50));
            }
            return finalString.Trim();
        }


    }
}
