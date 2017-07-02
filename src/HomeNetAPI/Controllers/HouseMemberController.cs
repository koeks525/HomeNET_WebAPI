using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("[controller]/[action]/")]
    public class HouseMemberController : Controller
    {
        private IHouseMemberRepository houseMemberRepository;
        private UserManager<User> userManager;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private IHouseRepository houseRepository;

        public HouseMemberController(IHouseMemberRepository houseMemberRepository, UserManager<User> userManager, IHouseRepository houseRepository)
        {
            this.houseMemberRepository = houseMemberRepository;
            this.userManager = userManager;
            this.houseRepository = houseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveHouseMembers([FromQuery] int houseID, [FromQuery] string clientCode)
        {
            ListResponse<HouseMember> response = new ListResponse<HouseMember>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client details to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var activeCall = await Task.Run(() =>
                {
                    return houseMemberRepository.GetActiveHouseMembers(houseID);
                });
                if (activeCall != null)
                {
                    response.DidError = false;
                    response.Message = "Here are active members for the selected house";
                    response.Model = activeCall;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No active house members were found";
                    response.Model = null;
                    return NotFound(response);
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
        public async Task<IActionResult> GetPendingHouseMembers([FromQuery] int houseID, [FromQuery] string clientCode)
        {
            ListResponse<HouseMember> response = new ListResponse<HouseMember>();
            try
            {
                if (androidClient != clientCode)
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var pendingCall = await Task.Run(() =>
                {
                    return houseMemberRepository.GetPendingHouseMembers(houseID);
                });
                if (pendingCall != null)
                {
                    response.DidError = false;
                    response.Message = "Here is a list of users pending approval for the house";
                    response.Model = pendingCall;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No pending users were found for the given house";
                    response.Model = null;
                    return NotFound(response);
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
        public async Task<IActionResult> GetBannedHouseMembers([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            ListResponse<HouseMember> response = new ListResponse<HouseMember>();
            try
            {
                if (androidClient != clientCode)
                {
                    response.DidError = true;
                    response.Message = "Please provide valid client code to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var bannedCall = await Task.Run(() =>
                {
                    return houseMemberRepository.GetBannedHouseMembers(houseID);
                });
                if (bannedCall != null)
                {
                    response.DidError = false;
                    response.Message = "Here are a list of banned users for the house";
                    response.Model = bannedCall;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No banned users were found for the selected house";
                    response.Model = null;
                    return NotFound(response);
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
        public async Task<IActionResult> JoinHouse([FromForm] String emailAddress, [FromQuery] int houseID, [FromQuery] String clientCode)
        {
            SingleResponse<HouseMember> response = new SingleResponse<HouseMember>();
            try
            {
                if (androidClient != clientCode)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                if (emailAddress == null)
                {
                    response.DidError = true;
                    response.Message = "Please send a valid email address to the server";
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
                    response.Message = "No house was found with the supplied data";
                    response.Model = null;
                    return NotFound(response);
                }

                HouseMember houseMember = new HouseMember()
                {
                    HouseMemberID = 0,
                    HouseID = houseID,
                    House = selectedHouse,
                    DateApplied = DateTime.Now.ToString(),
                    IsDeleted = 0,
                    UserID = selectedUser.Id,
                    User = selectedUser
                };

                var houseMembershipCall = await Task.Run(() =>
                {
                    return houseMemberRepository.AddHouseMember(houseMember);
                });
                if (houseMembershipCall != null)
                {
                    response.DidError = false;
                    response.Message = "House Member has been added to the house successfully! Wait for the system admin to approve!";
                    response.Model = houseMembershipCall;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with adding the house member to the system";
                    response.Model = houseMember;
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

        [HttpPut]
        public async Task<IActionResult> ApproveHouseMember([FromQuery] String emailAddress, [FromQuery] int houseMemberID ,[FromQuery] String clientCode) {
            SingleResponse<HouseMember> response = new SingleResponse<HouseMember>();
            try
            {
               if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }

                var selectedMembership = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMembership(houseMemberID);
                });
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No membership could be found with the given details";
                    response.Model = null;
                    return NotFound(response);
                }

                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(selectedMembership.HouseID);
                });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No linking house could be found";
                    response.Model = null;
                    return NotFound(response);
                }

                if (selectedMembership.HouseID != selectedHouse.HouseID)
                {
                    response.DidError = true;
                    response.Message = "Mismatch in house and membership";
                    response.Model = null;
                    return BadRequest(response);
                }

                selectedMembership.ApprovalStatus = 0;
                selectedMembership.DateApproved = DateTime.Now.ToString();

                //Figure out how to send notification to the user regarding their request
                



                var approval = await Task.Run(() =>
                {
                    return houseMemberRepository.UpdateMembership(selectedMembership);
                });
                if (approval != null)
                {
                    response.DidError = false;
                    response.Message = "House Member has been approved successfully!";
                    response.Model = approval;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with approving the house member";
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
        private async Task<IActionResult> GetHouseMember([FromQuery] int houseMemberID, [FromQuery] String clientCode)
        {
            SingleResponse<HouseMember> response = new SingleResponse<HouseMember>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedMembership = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMembership(houseMemberID);
                });
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "The house member data returned no results";
                    response.Model = null;
                    return NotFound(response);
                }
                response.DidError = false;
                response.Model = selectedMembership;
                response.Message = "Herewith the selected membership:";
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


