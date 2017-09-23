using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.Services;
using HomeNetAPI.ViewModels;

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
        private IFirebaseMessagingService messagingService;
        private IMailMessage emailService;

        public HouseMemberController(IHouseMemberRepository houseMemberRepository, UserManager<User> userManager, IHouseRepository houseRepository, IFirebaseMessagingService messagingService, IMailMessage emailService)
        {
            this.emailService = emailService;
            this.messagingService = messagingService;
            this.houseMemberRepository = houseMemberRepository;
            this.userManager = userManager;
            this.houseRepository = houseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveHouseMembers([FromQuery] int houseID, [FromQuery] string clientCode)
        {
            ListResponse<HouseMemberViewModel> response = new ListResponse<HouseMemberViewModel>();
            List<HouseMemberViewModel> activeUserList = new List<HouseMemberViewModel>();
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
                    foreach (HouseMember member in activeCall)
                    {
                        var foundUser = await userManager.FindByIdAsync(Convert.ToString(member.UserID));
                        if (foundUser != null)
                        {
                            var model = new HouseMemberViewModel()
                            {
                                UserID = foundUser.Id,
                                Name = foundUser.Name,
                                Surname = foundUser.Surname,
                                EmailAddress = foundUser.Email,
                                HouseMemberID = member.HouseMemberID,
                                CountryID = foundUser.CountryID,
                                Reason = ""
                            };
                            activeUserList.Add(model);
                        }
                    }
                    if (activeUserList.Count > 0)
                    {
                        response.DidError = false;
                        response.Message = "Here are active members for the selected house";
                        response.Model = activeUserList;
                        return Ok(response);
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No users were found";
                        response.Model = null;
                        return NotFound(response);
                    }
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
            List<HouseMemberViewModel> houseMemberList = new List<HouseMemberViewModel>();
            ListResponse<HouseMemberViewModel> response = new ListResponse<HouseMemberViewModel>();
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
                    foreach (HouseMember member in pendingCall)
                    {
                        var foundUser = await userManager.FindByIdAsync(Convert.ToString(member.UserID));
                        if (foundUser != null)
                        {
                            var foundMember = new HouseMemberViewModel()
                            {
                                UserID = foundUser.Id,
                                Name = foundUser.Name,
                                Surname = foundUser.Surname,
                                CountryID = foundUser.CountryID,
                                EmailAddress = foundUser.Email,
                                HouseMemberID = member.HouseMemberID,
                                Reason = ""
                            };
                            houseMemberList.Add(foundMember);
                        }
                    }
                    if (houseMemberList.Count > 0)
                    {
                        
                        response.DidError = false;
                        response.Message = "Herewith pending house members";
                        response.Model = houseMemberList;
                        return Ok(response);
                        
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No pending house members found";
                        response.Model = null;
                        return NotFound(response);
                    }
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
        public async Task<IActionResult> GetBannedMembers([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            ListResponse<HouseMemberViewModel> response = new ListResponse<HouseMemberViewModel>();
            List<HouseMemberViewModel> bannedUserList = new List<HouseMemberViewModel>();
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
                    foreach (HouseMember thisMember in bannedCall)
                    {
                        var foundUser = await userManager.FindByIdAsync(Convert.ToString(thisMember.UserID));
                        if (foundUser != null)
                        {
                            var bannedMember = new HouseMemberViewModel()
                            {
                                HouseMemberID = thisMember.HouseMemberID,
                                Name = foundUser.Name,
                                Surname = foundUser.Surname,
                                EmailAddress = foundUser.Email,
                                Reason = "",
                                UserID = foundUser.Id,
                            };
                            bannedUserList.Add(bannedMember);
                           
                        }
                    }
                    if (bannedUserList.Count > 0)
                    {
                        response.DidError = false;
                        response.Message = "Here are a list of banned users for the house";
                        response.Model = bannedUserList;
                        return Ok(response);
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No house members found";
                        response.Model = null;
                        return NotFound(response);
                    }
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

        [HttpGet]
        public async Task<IActionResult> ApproveHouseMember([FromQuery] int houseID,[FromQuery] String emailAddress,  [FromQuery] String adminEmail, [FromQuery] String clientCode)
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
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (adminUser == null || selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user credentials were found on the system";
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
                    response.Message = "No house was found";
                    response.Model = null;
                    return NotFound(response);
                }
                if (selectedHouse.UserID != adminUser.Id)
                {
                    response.DidError = true;
                    response.Message = "The selected user is not an admin of the house, therefore the user cannot approve members";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (selectedMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "There are no house members in this house. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                }
                var userMembership = selectedMemberships.First(i => i.UserID == selectedUser.Id && i.HouseID == selectedHouse.HouseID && i.ApprovalStatus == 1);
                if (userMembership == null)
                {
                    response.DidError = true;
                    response.Message = "The selected user is not a member of the selected house. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                userMembership.ApprovalStatus = 0; //0 Approved
                userMembership.DateApproved = DateTime.Now.ToString();
                var updateMembership = await Task.Run(() =>
                {
                    return houseMemberRepository.UpdateMembership(userMembership);
                });
                if (updateMembership != null)
                {
                    await messagingService.SendFirebaseMessage(updateMembership.HouseMemberID, $"{selectedHouse.Name}: House Membership Approved!", $"Congratulations, {selectedUser.Name}, your membership to {selectedHouse.Name} has been approved! Log in to the mobile app and start networking!","membership_approved", updateMembership.DateApproved, selectedUser.FirebaseMessagingToken, "");
                    bool result = await Task.Run(() => { return emailService.SendMailMessage($"{selectedUser.Name} {selectedUser.Surname}", selectedUser.Email, $"{selectedHouse.Name}: House Membership Approved!", $"Congratulations {selectedUser.Name},\n\nYour request to join a house ({selectedHouse.Name}) has been approved! Log in to the mobile application on your device to start the networking experience. \n\nKind Regards,\n\nHomeNET Administrative Services"); });
                    response.DidError = false;
                    response.Message = "House membership approved";
                    response.Model = updateMembership;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Somethting went wrong with approving your membership. Please try again later";
                    response.Model = null;
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

        [HttpGet]
        public async Task<IActionResult> DeclineHouseMember([FromQuery] int houseID, [FromQuery] String emailAddress, [FromQuery] String adminEmail, [FromQuery] String clientCode)
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
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (adminUser == null || selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user details were found with the supplied data";
                    response.Model = null;
                    return NotFound(response);
                }
                if (selectedHouse.UserID != adminUser.Id)
                {
                    response.DidError = true;
                    response.Message = "The selected user is not an administrator for the house";
                    response.Model = null;
                    return BadRequest(response);
                }
                var houseMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (houseMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "The house selected does not have any active memberships. Please try again later";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedMembership = houseMemberships.First(i => i.HouseID == selectedHouse.HouseID && i.UserID == selectedUser.Id && i.ApprovalStatus != 2);
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No membership was found for the selected user";
                    response.Model = null;
                    return NotFound(response);
                }
                selectedMembership.ApprovalStatus = 2;
                selectedMembership.DateLeft = DateTime.Now.ToString();
                var updateTask = await Task.Run(() =>
                {
                    return houseMemberRepository.UpdateMembership(selectedMembership);
                });
                if (updateTask != null)
                {
                    response.DidError = false;
                    response.Message = "User request declined successfully!";
                    response.Model = updateTask;
                    await messagingService.SendFirebaseMessage(updateTask.HouseMemberID, $"{selectedHouse.Name}: Join Request Declined", $"{selectedHouse.Name}: Unfortunately, the administrator of the house has declined your request. If you beleive this was an error, please speak to them or re-join.","membership_declined", Convert.ToString(updateTask.IsDeleted), selectedUser.FirebaseMessagingToken, "");
                    bool result = await Task.Run(() =>
                    {
                        return emailService.SendMailMessage($"{selectedUser.Name} {selectedUser.Surname}", selectedUser.Email, $"{selectedHouse.Name}: Join Request Declined!", $"Hi, \n\nUnfortunately, your request for joining house {selectedHouse.Name} has been declined by the house administrator. If you beleive this is an error, please speak to the house administrator. Alternatively, you could request to join the house again");
                    });
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with declining the join request";
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
        public async Task<IActionResult> GetBannedHouseMembers([FromQuery] int houseID, [FromQuery] string adminEmail, [FromQuery] String clientCode)
        {
            ListResponse<HouseMemberViewModel> response = new ListResponse<HouseMemberViewModel>();
            List<HouseMemberViewModel> bannedUserList = new List<HouseMemberViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
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
                    response.Message = "No house was found for the selected data";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (houseMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "No memberships were found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                var bannedMemberships = houseMemberships.Where(i => i.HouseID == selectedHouse.HouseID && i.ApprovalStatus == 2).ToList();
                if (bannedMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "No banned members were found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HouseMember member in bannedMemberships)
                {
                    var foundUser = await userManager.FindByIdAsync(Convert.ToString(member.UserID));
                    if (foundUser != null)
                    {
                        var bannedMember = new HouseMemberViewModel()
                        {
                            UserID = foundUser.Id,
                            Name = foundUser.Name,
                            Surname = foundUser.Surname,
                            EmailAddress = foundUser.Email,
                            CountryID = foundUser.CountryID,
                            Reason = ""
                        };
                        bannedUserList.Add(bannedMember);
                    }
                }
                if (bannedUserList.Count > 0)
                {
                    response.DidError = false;
                    response.Message = "Herewit the banned users";
                    response.Model = bannedUserList;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No banned users found";
                    response.Model = null;
                    return NotFound(response);
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
        public async Task<IActionResult> LeaveHouse([FromQuery] int houseMemberID, [FromQuery] int houseID, [FromQuery] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<HouseMember> response = new SingleResponse<HouseMember>();
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
                    response.Message = "The sent email address does not match any records on our system";
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
                var selectedMembership = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMembership(houseMemberID);
                });
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No membership was found with the supplied data";
                    response.Model = null;
                    return NotFound(response);
                }
                if (selectedMembership.HouseID != selectedHouse.HouseID)
                {
                    response.DidError = true;
                    response.Message = "You are not a member of the selected house";
                    response.Model = null;
                    return BadRequest(response);
                }
                selectedMembership.DateLeft = DateTime.Now.ToString();
                selectedMembership.ApprovalStatus = 2;
                var updateTask = await Task.Run(() =>
                {
                    return houseMemberRepository.UpdateMembership(selectedMembership);
                });
                if (updateTask == null)
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with updating house membership";
                    response.Model = null;
                    return BadRequest(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "You have left the house successfully... you will be logged out";
                    response.Model = updateTask;
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


