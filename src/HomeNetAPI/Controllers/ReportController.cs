using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using HomeNetAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class ReportController : Controller
    {
        private IHouseRepository houseRepository;
        private IHousePostRepository housePostRepository;
        private IHouseMemberRepository houseMemberRepository;
        private IUserRepository userRepository;
        private UserManager<User> userManager;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";

        public ReportController(IHousePostRepository housePostRepository, IHouseRepository houseRepository, UserManager<User> userManager, IHouseMemberRepository houseMemberRepository, IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.houseMemberRepository = houseMemberRepository;
            this.houseRepository = houseRepository;
            this.housePostRepository = housePostRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetHouseOverviewReport([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            SingleResponse<HomeData> response = new SingleResponse<HomeData>();
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
                    response.Message = "No house was found with the given ID";
                    response.Model = null;
                    return NotFound(response);
                }
                HomeData homeData = new HomeData();
                homeData.TotalActiveUsers = 0;
                homeData.TotalBannedUsers = 0;
                homeData.TotalPendingUsers = 0;
                homeData.TotalPosts = 0;
                homeData.TotalUsers = 0;

                var housePostData = await Task.Run(() => { return housePostRepository.GetHousePosts(houseID); });
                if (housePostData != null)
                {
                    homeData.TotalPosts = housePostData.Count();
                }
                var houseMemberData = await Task.Run(() => { return houseMemberRepository.GetActiveHouseMembers(houseID); });
                if (houseMemberData != null)
                {
                    homeData.TotalActiveUsers = houseMemberData.Count();
                }
                houseMemberData = await Task.Run(() => { return houseMemberRepository.GetBannedHouseMembers(houseID); });
                if (houseMemberData != null)
                {
                    homeData.TotalBannedUsers = houseMemberData.Count();
                }
                houseMemberData = await Task.Run(() => { return houseMemberRepository.GetPendingHouseMembers(houseID); });
                if (houseMemberData != null)
                {
                    homeData.TotalPendingUsers = houseMemberData.Count();
                }
                homeData.DateCreated = selectedHouse.DateCreated;
                response.DidError = false;
                response.Model = homeData;
                response.Message = "Here is basic reporting data for the selected house: ";
                return Ok(response);
            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOverviewReport([FromForm] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<UserData> response = new SingleResponse<UserData>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please provide valid client details to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with these credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                UserData userData = new UserData();
                userData.TotalAnnouncements = 0;
                userData.TotalHousesJoined = 0;
                userData.TotalPosts = 0;

                var selectedPosts = await Task.Run(() =>
                {
                    return userRepository.GetHousePosts(selectedUser.Id);

                });
                if (selectedPosts != null)
                {
                    userData.TotalPosts = selectedPosts.Count();
                }






                response.DidError = false;
                response.Message = "Here is what the server can tell about the user";
                response.Model = userData;
                return Ok(response);


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
