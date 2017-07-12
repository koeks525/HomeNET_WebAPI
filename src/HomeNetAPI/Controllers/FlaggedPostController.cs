using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class FlaggedPostController : Controller
    {
        private IFlaggedPostRepository flaggedPostRepository;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private UserManager<User> userManager;
        private IHouseRepository houseRepository;
        private IHousePostRepository housePostRepository;

        public FlaggedPostController(IFlaggedPostRepository flaggedPostRepository, UserManager<User> userManager, IHouseRepository houseRepository, IHousePostRepository housePostRepository)
        {
            this.housePostRepository = housePostRepository;
            this.houseRepository = houseRepository;
            this.userManager = userManager;
            this.flaggedPostRepository = flaggedPostRepository;
        }

        [HttpPost]
        public async Task<IActionResult> FlagHousePost([FromBody] HousePostFlag flaggedHousePost, [FromForm] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<HousePostFlag> response = new SingleResponse<HousePostFlag>();
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
                    response.Message = "No user was found with provided details";
                    response.Model = null;
                    return NotFound(response);
                }

                var selectedHousePost = await Task.Run(() =>
                {
                    return housePostRepository.GetHousePost(flaggedHousePost.HousePostID);
                });
                if (selectedHousePost == null)
                {
                    response.DidError = true;
                    response.Message = "No house was found for the linked post";
                    response.Model = null;
                    return NotFound(response);
                }

                //Add a check to verify membership to house
                HousePostFlag flaggedPost = new HousePostFlag();
                flaggedPost.HousePost = selectedHousePost;
                flaggedPost.DateFlagged = DateTime.Now.ToString();
                flaggedPost.HousePostID = selectedHousePost.HousePostID;
                flaggedPost.Message = flaggedHousePost.Message;
                flaggedPost.IsDeleted = 0;
                selectedHousePost.IsFlagged = 1;

                var updateHousePost = await Task.Run(() =>
                {
                    return housePostRepository.UpdateHousePost(selectedHousePost);
                });
                if (updateHousePost == null)
                {
                    response.DidError = true;
                    response.Message = "Failed to update the original post. The post may have been deleted";
                    response.Model = null;
                    return NotFound();
                }
                var flagResult = await Task.Run(() =>
                {
                    return flaggedPostRepository.FlagHousePost(flaggedPost);
                });
                if (flaggedPost != null)
                {
                    response.DidError = false;
                    response.Message = "House post has been flagged successfully!";
                    response.Model = flaggedPost;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with flagging the post";
                    response.Model = flaggedPost;
                    return BadRequest(response);
                }



            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBannedHousePosts([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            ListResponse<HousePostFlag> response = new ListResponse<HousePostFlag>();
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
                var bannedPostData = await Task.Run(() =>
                {
                    return flaggedPostRepository.GetFlaggedPosts(houseID);
                });
                if (bannedPostData == null)
                {
                    response.DidError = true;
                    response.Message = "No flagged posts were found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Here are the following banned posts data";
                    response.Model = bannedPostData;
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

        [HttpGet]
        public async Task<IActionResult> GetHousePendingPosts([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            ListResponse<HousePostFlag> response = new ListResponse<HousePostFlag>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid credentials to the server";
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
                    response.Message = "No house was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }
                var pendingPostData = await Task.Run(() =>
                {
                    return flaggedPostRepository.GetPendingPosts(houseID);
                });
                if (pendingPostData == null)
                {
                    response.DidError = true;
                    response.Message = "No pending posts were found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Found flagged posts for the selected house";
                    response.Model = pendingPostData;
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
