using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.ViewModels;
using HomeNetAPI.Services;

namespace HomeNetAPI.Controllers
{
    [Route("/[controller]/[action]")]
    [Authorize]
    public class CommentController : Controller
    {
        private ICommentRepository commentRepository;
        private IHousePostRepository postRepository;
        private IHouseMemberRepository memberRepository;
        private IHouseRepository houseRepository;
        private UserManager<User> userManager;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private String key = "AIzaSyBhLv8gbKVzEIhtfYYSIcCRUkbS7z61qT0";
        private IFirebaseMessagingService messagingService;
        

        public CommentController(ICommentRepository commentRepository, IHousePostRepository postRepository, UserManager<User> userManager, IHouseMemberRepository memberRepository, IHouseRepository houseRepository, IFirebaseMessagingService messagingService)
        {
            this.messagingService = messagingService;
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
            this.userManager = userManager;
            this.memberRepository = memberRepository;
            this.houseRepository = houseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] int housePostID, [FromQuery] String clientCode)
        {
            ListResponse<CommentViewModel> response = new ListResponse<CommentViewModel>();
            List<CommentViewModel> commentList = new List<CommentViewModel>();
            try
            {
                var housePost = await Task.Run(() =>
                {
                    return postRepository.GetHousePost(housePostID);
                });
                if (housePost == null)
                {
                    response.DidError = true;
                    response.Message = "No house post was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }
                var ownerMembership = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMembership(housePost.HouseMemberID);
                });
                if (ownerMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No owner information was found. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(ownerMembership.HouseID);
                });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No house was found with the provided details";
                    response.Model = null;
                    return NotFound(response);
                }
                var housePostComments = await Task.Run(() =>
                {
                    return commentRepository.GetComments(housePost.HousePostID);
                });
                if (housePostComments == null)
                {
                    response.DidError = true;
                    response.Message = "No comments were found";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HousePostComment comment in housePostComments)
                {
                    var membership = await Task.Run(() =>
                    {
                        return memberRepository.GetHouseMembership(comment.HouseMemberID);
                    });
                    if (membership != null)
                    {
                        var foundUser = await userManager.FindByIdAsync(Convert.ToString(membership.UserID));
                        if (foundUser != null)
                        {
                            var model = new CommentViewModel()
                            {
                                Name = foundUser.Name,
                                Surname = foundUser.Surname,
                                EmailAddress = foundUser.Email,
                                HouseMemberID = membership.HouseMemberID,
                                HousePostID = housePost.HousePostID,
                                Comment = comment.Comment,
                                DateAdded = comment.DatePosted,
                                HousePostCommentID = comment.HousePostCommentID
                            };
                            commentList.Add(model);
                        }
                    }
                    
                }
                if (commentList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No comments were returned";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Herewith the comments";
                    response.Model = commentList;
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

        [HttpPost] 
        public async Task<IActionResult> AddComment([FromBody] CommentParticalModel model, [FromQuery] String clientCode) 
        {
            SingleResponse<CommentViewModel> response = new SingleResponse<CommentViewModel>();
            List<User> userList = new List<Models.User>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                if (model == null)
                {
                    response.DidError = true;
                    response.Message = "Please send valid comment data to the server";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedUser = await userManager.FindByEmailAsync(model.EmailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the provided information";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedPost = await Task.Run(() =>
                 {
                     return postRepository.GetHousePost(model.HousePostID);
                 });
                if (selectedPost == null)
                {
                    response.DidError = true;
                    response.Model = null;
                    response.Message = "No house post was found with the request";
                    return NotFound(response);
                }
                
                var ownerMembership = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMembership(selectedPost.HouseMemberID);
                });
                if (ownerMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No owner information was found";
                    response.Model = null;
                    return NotFound(response);
                }
                var ownerUser = await userManager.FindByIdAsync(Convert.ToString(ownerMembership.UserID));
                if (ownerUser == null)
                {
                    response.DidError = true;
                    response.Message = "No owner information found";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(ownerMembership.HouseID);
                });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No house found";
                    response.Model = null;
                    return NotFound(response);
                }
                HouseMember userMembership = null;
                var userMemberships = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMember(selectedUser.Id);
                });
                if (userMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "No memberships were found";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HouseMember member in userMemberships)
                {
                    if (member.HouseID == ownerMembership.HouseID)
                    {
                        userMembership = member;
                        break;
                    }
                }
                if (userMembership == null)
                {
                    response.DidError = true;
                    response.Message = "You are not subscribed to the owner's house. Please subscribe and try again";
                    response.Model = null;
                    return BadRequest(response);
                }
                var houseMemberships = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMemberships(userMembership.HouseID);
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
                                userList.Add(foundUser);
                            }
                        }
                    }
                }

                var newComment = new HousePostComment()
                {
                    Comment = model.Comment,
                    DatePosted = DateTime.Now.ToString(),
                    HouseMemberID = userMembership.HouseMemberID,
                    HousePostID = selectedPost.HousePostID,
                    IsDeleted = 0,
                    IsFlagged = 0

                };
                var newCommentCall = await Task.Run(() =>
                {
                    return commentRepository.AddComment(newComment);
                });
                if (newCommentCall == null)
                {
                    response.DidError = true;
                    response.Model = null;
                    response.Message = "Error saving comment. Please try again";
                    return BadRequest(response);
                } else
                {
                    foreach (User thisUser in userList)
                    {
                        await messagingService.SendFirebaseMessage(3, $"{selectedHouse.Name}: New Comment on {ownerUser.Name} Post", $"A new comment has been posted on {ownerUser.Name}'s post!. Log into HomeNET to view the comment", thisUser.FirebaseMessagingToken, key);
                    }


                    var commentModel = new CommentViewModel()
                    {
                        Name = selectedUser.Name,
                        Surname = selectedUser.Surname,
                        EmailAddress = selectedUser.Email,
                        Comment = newCommentCall.Comment,
                        DateAdded = newCommentCall.DatePosted,
                        HouseMemberID = userMembership.HouseMemberID,
                        HousePostCommentID = newCommentCall.HousePostCommentID,
                        HousePostID = selectedPost.HousePostID
                    };
                    response.DidError = false;
                    response.Message = "Here with your new comment";
                    response.Model = commentModel;
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





    }
}
