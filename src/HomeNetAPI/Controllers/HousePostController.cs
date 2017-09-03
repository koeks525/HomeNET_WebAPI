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
        private String token = "AIzaSyBhLv8gbKVzEIhtfYYSIcCRUkbS7z61qT0";
        private IHousePostRepository housePostRepository;
        private IHouseMemberRepository houseMemberRepository;
        private IHouseRepository houseRepository;
        private IImageProcessor imageProcessor;
        private IFirebaseMessagingService messagingService;


        public HousePostController(UserManager<User> userManager, IHousePostRepository housePostRepository, IHouseRepository houseRepository, IImageProcessor imageProcessor, IHouseMemberRepository houseMemberRepository, IFirebaseMessagingService messagingService)
        {
            this.messagingService = messagingService;
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
            List<User> subscribedMembers = new List<Models.User>();
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

                var houseMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (houseMemberships != null)
                {
                    foreach (HouseMember member in houseMemberships)
                    {
                        var foundUser = await userManager.FindByIdAsync(Convert.ToString(member.UserID));
                        if (foundUser != null)
                        {
                            if (foundUser.Id != selectedUser.Id)
                            {
                                subscribedMembers.Add(foundUser);
                            }
                        }
                    }
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
                        String directory = $"C:/HomeNET/Posts/Houses/{selectedHouse.HouseID}";
                        if (file.FileName.Contains(":"))
                        {
                            finalFileName = file.FileName.Replace(":", "_");
                        }
                        else
                        {
                            finalFileName = file.FileName;
                        }
                        finalFileName = GenerateRandomString() + finalFileName;
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        using (var fileStream = new FileStream(directory + "/" + finalFileName, FileMode.Create, FileAccess.ReadWrite))
                        {
                            newPost.MediaResource = directory + "/" + finalFileName;
                            await file.CopyToAsync(fileStream);
                            var result = await Task.Run(() =>
                            {
                                return housePostRepository.AddHousePost(newPost);

                            });
                            if (result != null)
                            {
                                if (subscribedMembers.Count > 0)
                            {
                                foreach (User currentUser in subscribedMembers)
                                {
                                   await messagingService.SendFirebaseMessage(2, $"{selectedHouse.Name}: New Post!", $"{selectedUser.Name} added a new post to your house! Log into HomeNET now to view the post! ", currentUser.FirebaseMessagingToken, token);
                                }
                            }
                                response.DidError = false;
                                response.Model = result;
                                response.Message = "New post created successfully!";
                                return Ok(response);
                            }
                            else
                            {
                                response.DidError = true;
                                response.Message = "Error Adding new Post";
                                response.Model = null;
                                return BadRequest(response);
                            }
                        }

                    }
                    else
                    {
                        var addResult = await Task.Run(() =>
                        {
                            return housePostRepository.AddHousePost(newPost);
                        });
                        if (addResult != null)
                        {
                        if (subscribedMembers.Count > 0)
                        {
                            foreach (User currentUser in subscribedMembers)
                            {
                                await messagingService.SendFirebaseMessage(2, $"{selectedHouse.Name}: New Post!", $"{selectedUser.Name} added a new post to your house! Log into HomeNET now to view the post! ", currentUser.FirebaseMessagingToken, token);
                            }
                        }

                        response.DidError = false;
                            response.Message = "New post added successfully!";
                            response.Model = addResult;
                            return Ok(response);
                        }
                        else
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
                var membership = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMembership(housePost.HouseMemberID);
                });
                if (membership == null)
                {
                    response.DidError = true;
                    response.Message = "No house membership could be found. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(membership.HouseID);
                });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No house was found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseAdmin = await userManager.FindByIdAsync(Convert.ToString(selectedHouse.UserID));
                var user = await userManager.FindByEmailAsync(emailAddress);
                if (user == null)
                {
                    response.DidError = true;
                    response.Message = "Sorry, your email address does not return any records on the system, therefore, you cannot flag posts";
                    response.Model = null;
                    return NotFound(response);
                }
                var userMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMember(user.Id);
                });
                if (userMemberships == null)
                {
                    response.DidError = true;
                    response.Model = null;
                    response.Message = "The selected user is not subscribed to any houses. Please try again later";
                    return NotFound(response);
                }
                var resultMembership = userMemberships.First(i => i.UserID == user.Id && i.HouseID == selectedHouse.HouseID);
                if (resultMembership == null)
                {
                    response.DidError = true;
                    response.Model = null;
                    response.Message = "You do not have a membership to the current house you are wishing to report to";
                    return NotFound(response);
                }
                if (houseAdmin == null)
                {
                    response.DidError = true;
                    response.Message = "No administrator was found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                var newFlag = new HousePostFlag()
                {
                    DateFlagged = DateTime.Now.ToString(),
                    HouseMemberID = resultMembership.HouseMemberID,
                    HousePostID = housePost.HousePostID,
                    IsDeleted = 0,
                    IsFlagged = 1,
                    Message = flagReason,
                };
                var flagCall = await Task.Run(() =>
                {
                    return housePostRepository.FlagHousePost(newFlag);
                });
                if (flagCall == null)
                {
                    response.DidError = true;
                    response.Message = "Error occurred while flagging the house post. Please try again later";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    var updatePost = await Task.Run(() =>
                    {
                        return housePostRepository.UpdateHousePost(housePost);
                    });
                    if (updatePost != null)
                    {
                        await messagingService.SendFirebaseMessage(10, $"{selectedHouse.Name}: New Post Flagged", $"{user.Name} {user.Surname} flagged a house post in your house. Please login to the app to deal with this", houseAdmin.FirebaseMessagingToken, token);
                        housePost.IsFlagged = 1;
                        response.DidError = false;
                        response.Message = "House post flagged successfully!";
                        response.Model = newFlag;
                        return Ok(response);
                    } else
                    {
                        response.DidError = true;
                        response.Message = "House post has been flagged, however, this could not be updated on the main post";
                        response.Model = newFlag;
                        return BadRequest(response);
                    }
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
        public async Task<IActionResult> GetAllMultimediaPosts([FromQuery] String emailAddress, [FromQuery] String clientCode)
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
                        foreach (HousePost post in posts)
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
                            if (model.MediaResource != "" && model.MediaResource != null)
                            {
                                finalPostList.Add(model);
                            }
                        }
                    }
                }
                if (finalPostList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No posts were found";
                    response.Model = null;
                    return NotFound(response);
                }
                else
                {
                    response.DidError = false;
                    response.Message = "Herewith the house posts";
                    response.Model = finalPostList;
                    return Ok(response);
                }


            }
            catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHousePostImage([FromQuery] int housePostID, [FromQuery] String clientCode)
        {
            SingleResponse<FileStream> response = new SingleResponse<FileStream>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var housePost = await Task.Run(() =>
                {
                    return housePostRepository.GetHousePost(housePostID);
                });
                if (housePost == null)
                {
                    response.DidError = true;
                    response.Message = "No house post was found";
                    response.Model = null;
                    return NotFound(response);
                }
                if (housePost.MediaResource == "" || housePost.MediaResource == null)
                {
                    response.DidError = true;
                    response.Message = "The selected house post does not have an image linked to it";
                    response.Model = null;
                    return NotFound(response);
                }
                var file = new FileStream(housePost.MediaResource, FileMode.Open, FileAccess.Read);
                if (file != null)
                {
                    return File(file, "image/jpeg");
                } else
                {
                    response.DidError = true;
                    response.Message = "Error opening file";
                    response.Model = null;
                    return BadRequest(response);
                }
            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
                response.Model = null;
                return NotFound(response);
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
