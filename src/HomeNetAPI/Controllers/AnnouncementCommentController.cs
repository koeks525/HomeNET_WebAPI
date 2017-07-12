using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.Repository;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Identity;

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
        public async Task<IActionResult> CreateAnnouncementComment([FromForm] int houseID, [FromForm] int houseAnnouncementID, [FromForm] String comment, [FromForm] String emailAddress, [FromQuery] String clientCode)
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
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No house was found with the selected data";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedMemberships = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (selectedMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "This house does not have any subsribed members";
                    response.Model = null;
                    return BadRequest(response);
                }
                HouseMember selectedMembership = null;
                foreach (HouseMember member in selectedMemberships)
                {
                    if (member.UserID == selectedUser.Id)
                    {
                        selectedMembership = member;
                        break;
                    }
                }
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "You are not subscribed to this house";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedAnnouncement = await Task.Run(() =>
                {
                    return announcementRepository.GetHouseAnnouncement(houseAnnouncementID);
                });
                if (selectedAnnouncement == null)
                {
                    response.DidError = true;
                    response.Message = "No announcement was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }
                AnnouncementComment newComment = new AnnouncementComment();
                newComment.Comment = comment;
                newComment.DateAdded = DateTime.Now.ToString();
                newComment.IsDeleted = 0;
                newComment.IsFlagged = 0;
                newComment.HouseAnnouncementID = selectedAnnouncement.HouseAnnouncementID;
                newComment.HouseMemberID = selectedMembership.HouseMemberID;
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
            ListResponse<AnnouncementComment> response = new ListResponse<AnnouncementComment>();
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
                    response.Message = "No comments found";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Comments found for the following announcement";
                    response.Model = comments;
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
