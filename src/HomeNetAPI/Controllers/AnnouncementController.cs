using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.ViewModels;
using HomeNetAPI.Models;
using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.Services;
using Microsoft.AspNetCore.Identity;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class AnnouncementController : Controller 
    {
        private IAnnouncementRepository announcementRepository;
        private IFirebaseMessagingService messagingService;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private IHouseRepository houseRepository;
        private UserManager<User> userManager;
        private IHouseMemberRepository memberRepository;

        public AnnouncementController(IAnnouncementRepository announcementRepository, IFirebaseMessagingService messagingService, IHouseRepository houseRepository, UserManager<User> userManager, IHouseMemberRepository memberRepository)
        {
            this.announcementRepository = announcementRepository;
            this.messagingService = messagingService;
            this.houseRepository = houseRepository;
            this.userManager = userManager;
            this.memberRepository = memberRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetHouseAnnouncements([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            ListResponse<HouseAnnouncement> response = new ListResponse<HouseAnnouncement>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(houseID);
                });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No house was found with the given credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                var announcements = await Task.Run(() =>
                {
                    return announcementRepository.GetHouseAnnouncements(houseID);
                });
                if (announcements == null)
                {
                    response.DidError = true;
                    response.Message = "No announcements were found for the house. Why not create a new announcement";
                    response.Model = null;
                    return NotFound(response);
                }
                else
                {
                    response.DidError = false;
                    response.Message = "Here are announcements found for the selected house";
                    response.Model = announcements;
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
        public async Task<IActionResult> GetUserAnnouncements([FromQuery] int houseID, [FromQuery] String emailAddress, [FromQuery] String clientCode)
        {
            ListResponse<HouseAnnouncement> response = new ListResponse<HouseAnnouncement>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the given credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseAnnouncements = await Task.Run(() =>
                {
                    return announcementRepository.GetUserAnnoucements(selectedUser.Id);
                });
                if (houseAnnouncements == null)
                {
                    response.DidError = true;
                    response.Message = "No announcements were found for the selected user";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Here are the following announcements by the selected user";
                    response.Model = houseAnnouncements;
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
        public async Task<IActionResult> CreateAnnouncement([FromBody] String title, [FromBody] String message, [FromQuery] int houseID, [FromQuery] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<HouseAnnouncement> response = new SingleResponse<HouseAnnouncement>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                if (title == null || message == null)
                {
                    response.DidError = true;
                    response.Message = "Please send a title and message contents for your announcement";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedUser = await Task.Run(() =>
                {
                    return userManager.FindByEmailAsync(emailAddress);
                });
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the given credentials";
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
                    response.Message = "No house was found with the given credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedMembership = await Task.Run(() =>
                {
                    return memberRepository.GetHouseMember(selectedUser.Id, selectedHouse.HouseID);
                });
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "You are not subscribed to the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                var newAnnouncement = new HouseAnnouncement();
                newAnnouncement.IsDeleted = 0;
                newAnnouncement.Title = title.Trim();
                newAnnouncement.Message = message;
                newAnnouncement.HouseID = selectedHouse.HouseID;
                newAnnouncement.HouseMemberID = selectedMembership.HouseMemberID;
                newAnnouncement.DateAdded = DateTime.Now.ToString();
                newAnnouncement.HouseAnnouncementID = 0;
                var addResult = await Task.Run(() =>
                {
                    return announcementRepository.AddHouseAnnouncement(newAnnouncement);
                });
                if (addResult == null)
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with adding the new announcement";
                    response.Model = null;
                    return BadRequest(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Announcement added successfully";
                    response.Model = newAnnouncement;
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
