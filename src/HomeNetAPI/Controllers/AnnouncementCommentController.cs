using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.Repository;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.ViewModels;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class AnnouncementCommentController : Controller
    {
        private UserManager<User> userManager;
        private IHouseRepository houseRepository;
        private IHouseMemberRepository memberRepository;
        private IAnnouncementRepository announcementRepository;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private IAnnouncementCommentRepository commentRepository;

        public AnnouncementCommentController(UserManager<User> userManager, IHouseRepository houseRepository, IHouseMemberRepository memberRepository, IAnnouncementRepository announcementRepository, IAnnouncementCommentRepository commentRepository)
        {
            this.userManager = userManager;
            this.houseRepository = houseRepository;
            this.memberRepository = memberRepository;
            this.announcementRepository = announcementRepository;
            this.commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncementComment([FromBody] NewCommentViewModel model, [FromQuery] String clientCode)
        {
            SingleResponse<AnnouncementComment> response = new SingleResponse<AnnouncementComment>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedUser = await userManager.FindByEmailAsync(model.EmailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the supplied data";
                    response.Model = null;
                    return NotFound(response);
                }

                var memberships = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMember(selectedUser.Id);
                });
                if (memberships == null)
                {
                    response.DidError = true;
                    response.Message = "No memberships were found for the selected user";
                    response.Model = null;
                    return NotFound(response);
                }
               
                var selectedAnnouncement = await Task.Run(() =>
                {
                    return announcementRepository.GetHouseAnnouncement(model.HouseAnnouncementID);
                });
                if (selectedAnnouncement == null)
                {
                    response.DidError = true;
                    response.Message = "No announcement was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }

                var resultMembership = memberships.First(i => i.HouseID == selectedAnnouncement.HouseID);
                if (resultMembership == null)
                {
                    response.DidError = true;
                    response.Message = "You are not subscribed to the selected house. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                }
                AnnouncementComment newComment = new AnnouncementComment();
                newComment.Comment = model.Comment;
                newComment.DateAdded = DateTime.Now.ToString();
                newComment.IsDeleted = 0;
                newComment.IsFlagged = 0;
                newComment.HouseAnnouncementID = selectedAnnouncement.HouseAnnouncementID;
                newComment.HouseMemberID = resultMembership.HouseMemberID;
                newComment.AnnouncementCommentID = 0;
                var result = await Task.Run(() =>
                {
                    return commentRepository.AddAnnouncementComment(newComment);
                });
                if (result == null)
                {
                    response.DidError = true;
                    response.Message = "Comment failed!";
                    response.Model = null;
                    return BadRequest(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Comment added successfully";
                    response.Model = newComment;
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
        public async Task<IActionResult> GetAnnouncementComments([FromQuery] int houseAnnouncementID, [FromQuery] String clientCode)
        {
            ListResponse<AnnouncementCommentViewModel> response = new ListResponse<AnnouncementCommentViewModel>();
            List<AnnouncementCommentViewModel> commentList = new List<AnnouncementCommentViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var comments = await Task.Run(() =>
                {
                    return commentRepository.GetAnnoucementComments(houseAnnouncementID);
                });
                if (comments == null)
                {
                    response.DidError = false;
                    response.Model = null;
                    response.Message = "No comments found";
                    return NotFound(response);
                }
                foreach (AnnouncementComment comment in comments)
                {
                    var houseMembership = await Task.Run(() =>
                    {
                        return memberRepository.GetHouseMembership(comment.HouseMemberID);
                    });
                    if (houseMembership != null)
                    {
                        var user = await userManager.FindByIdAsync(Convert.ToString(houseMembership.UserID));
                        if (user != null)
                        {
                            AnnouncementCommentViewModel model = new AnnouncementCommentViewModel()
                            {
                                Name = user.Name,
                                Surname = user.Surname,
                                EmailAddress = user.Email,
                                DateAdded = comment.DateAdded,
                                HouseAnnouncementID = comment.HouseAnnouncementID,
                                AnnouncementCommentID = comment.AnnouncementCommentID,
                                Comment = comment.Comment,
                                HouseMemberID = comment.HouseMemberID,
                                IsDeleted = comment.IsDeleted,
                                IsFlagged = comment.IsFlagged
                            };
                            commentList.Add(model);
                        }
                    }
                }
                if (commentList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No comments found";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Here are the comments";
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
    }
}
