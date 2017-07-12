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
    public class AnnouncementController : Controller 
    {
        private IAnnouncementRepository announcementRepository;
        private IFirebaseMessagingService messagingService;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private IHouseRepository houseRepository;
        private UserManager<User> userManager;

        public AnnouncementController(IAnnouncementRepository announcementRepository, IFirebaseMessagingService messagingService, IHouseRepository houseRepository, UserManager<User> userManager)
        {
            this.announcementRepository = announcementRepository;
            this.messagingService = messagingService;
            this.houseRepository = houseRepository;
            this.userManager = userManager;
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

    }
}
