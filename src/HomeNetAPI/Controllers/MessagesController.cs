using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.Models;
using HomeNetAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.Services;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class MessagesController : Controller
    {
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private String key = "AIzaSyBhLv8gbKVzEIhtfYYSIcCRUkbS7z61qT0";
        private IMessageThreadMessageRepository messageThreadMessageRepository;
        private IMessageThreadRepository messageThreadRepository;
        private IMessageThreadParticipantRepository participantRepository;
        private IHouseRepository houseRepository;
        private IHouseMemberRepository houseMemberRepository;
        private UserManager<User> userManager;
        private IFirebaseMessagingService messagingService;
        private IMailMessage mailService;


        //READ: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration 

        public MessagesController(IMessageThreadMessageRepository messageThreadMessageRepository, IMessageThreadParticipantRepository participantRepository, IMessageThreadRepository messageThreadRepository, IHouseRepository houseRepository, UserManager<User> userManager, IHouseMemberRepository houseMemberRepository, IFirebaseMessagingService messagingService, IMailMessage mailService)
        {
            this.messageThreadMessageRepository = messageThreadMessageRepository;
            this.messageThreadRepository = messageThreadRepository;
            this.participantRepository = participantRepository;
            this.houseRepository = houseRepository;
            this.houseMemberRepository = houseMemberRepository;
            this.userManager = userManager;
            this.messagingService = messagingService;
            this.mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessageThread([FromBody] NewMessageThreadViewModel newMessage, [FromQuery] String clientCode)
        {
            SingleResponse<MessageThread> response = new SingleResponse<MessageThread>();
            try
            {
                List<User> recepientUsers = new List<User>();
                List<HouseMember> recepients = new List<HouseMember>();
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                if (newMessage == null)
                {
                    response.DidError = true;
                    response.Message = "Missing data. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                var senderUser = await userManager.FindByEmailAsync(newMessage.SenderEmail);
                if (senderUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the supplied data";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedUser = await userManager.FindByEmailAsync(newMessage.EmailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(newMessage.HouseID);
                });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "No house was found with the given data";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseMemberships = await Task.Run(() => { return houseMemberRepository.GetHouseMemberships(newMessage.HouseID); });
                if (houseMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "The selected house does not have any memberships linked to it. Please subscribe to the house before sending messages to members";
                    response.Model = null;
                    return NotFound(response);
                }
                if (houseMemberships.Count < 2)
                {
                    response.DidError = true;
                    response.Message = "To send messages, your house must have at least 2 members.";
                    response.Model = null;
                    return BadRequest(response);
                }
                var senderMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMember(senderUser.Id);
                });
                if (senderMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "The sender is not subscribed to any houses. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                var userMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMember(selectedUser.Id);
                });
                if (userMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "The receipient is not subscribed to any houses";
                    response.Model = null;
                    return NotFound(response);
                }
                var userMembership = userMemberships.First(i => i.HouseID == newMessage.HouseID);
                var senderMembership = senderMemberships.First(i => i.HouseID == newMessage.HouseID);
                if (userMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No memberships found for the recepient user";
                    response.Model = null;
                    return NotFound(response);
                }
                if (senderMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No memberships found for the user";
                    response.Model = null;
                    return NotFound(response);
                }

                MessageThread newThread = new MessageThread()
                {
                    HouseMemberID = senderMembership.HouseMemberID,
                    IsDeleted = 0,
                    Title = newMessage.ThreadTitle,
                    Priority = 0,
                    MessageThreadID = 0,
                    HouseMember = senderMembership
                };
                var participant = new MessageThreadParticipant()
                {
                    HouseMemberID = userMembership.HouseMemberID,
                    IsDeleted = 0,
                    MessageThreadParticipantID = 0,

                };
                var createThreadCall = await Task.Run(() =>
                {
                    return messageThreadRepository.CreateMessageThread(newThread);
                });
                if (createThreadCall == null)
                {
                    response.DidError = true;
                    response.Message = "Error Creating message thread";
                    response.Model = null;
                    return BadRequest(response);
                }
                participant.MessageThreadID = createThreadCall.MessageThreadID;
                var addParticipant = await Task.Run(() =>
                {
                    return participantRepository.AddParticipant(participant);
                });
                if (addParticipant == null)
                {
                    response.DidError = true;
                    response.Message = "Error adding participant. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                }
                var message = new MessageThreadMessage()
                {
                    DateSent = DateTime.Now.ToString(),
                    HouseMemberID = senderMembership.HouseMemberID,
                    Message = newMessage.ThreadMessage,
                    MessageThreadID = createThreadCall.MessageThreadID,
                    MessageThreadMessageID = 0,
                };
                var addMessageCall = await Task.Run(() =>
                {
                    return messageThreadMessageRepository.AddMessageToThread(message);
                });
                if (addMessageCall == null)
                {
                    response.DidError = true;
                    response.Message = "error adding message to thread. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Message created and sent successfully";
                    response.Model = createThreadCall;
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
        public async Task<IActionResult> AddMessageToThread([FromBody] MessageViewModel messageModel, [FromQuery] String clientCode)
        {
            SingleResponse<MessageThreadMessage> response = new SingleResponse<MessageThreadMessage>();
            try
            {
                List<User> participantList = new List<User>();
                List<HouseMember> memberList = new List<HouseMember>();
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                if (messageModel == null)
                {
                    response.DidError = true;
                    response.Message = "Missing data. Please send valid data";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedUser = await userManager.FindByEmailAsync(messageModel.EmailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No valid user was found";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMember(selectedUser.Id);
                });
                if (houseMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "No house memberships were found for the selected user";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseMembership = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMembership(messageModel.HouseMemberID);
                });
                if (houseMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No house membership was found";
                    response.Model = null;
                    return NotFound(response);
                }
                var userMembership = houseMemberships.First(i => i.HouseID == houseMembership.HouseID);
                if (userMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No matching house could be found";
                    response.Model = null;
                    return NotFound(response);
                }


                MessageThreadMessage newMessage = new MessageThreadMessage()
                {
                    DateSent = DateTime.Now.ToString(),
                    HouseMemberID = userMembership.HouseMemberID,
                    Message = messageModel.Message,
                    MessageThreadID = messageModel.MessageThreadID,
                    MessageThreadMessageID = 0,
                };

                var newTask = await Task.Run(() =>
                {
                    return messageThreadMessageRepository.AddMessageToThread(newMessage);
                });
                if (newTask == null)
                {
                    response.DidError = true;
                    response.Message = "Someting went wrong with adding the conversation. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                }
                var participants = await Task.Run(() =>
                {
                    return participantRepository.GetMessageParticipants(newTask.MessageThreadID);
                });

                foreach (MessageThreadParticipant participant in participants)
                {
                    var membership = await Task.Run(() =>
                    {
                        return houseMemberRepository.GetHouseMembership(participant.HouseMemberID);
                    });
                    if (membership != null)
                    {
                        var foundUser = await userManager.FindByIdAsync(Convert.ToString(membership.UserID));
                        if (foundUser != null)
                        {
                            participantList.Add(foundUser);
                        }
                    }
                }
                if (participantList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No participants were found in this list";
                    response.Model = null;
                    return NotFound(response);
                }

                foreach (User thisUser in participantList)
                {
                    var result = await Task.Run(() =>
                    {
                        return messagingService.SendFirebaseMessage($"New Message From {selectedUser.Name}", $"A New message has been sent by {selectedUser.Name}.", thisUser.FirebaseMessagingToken, key);
                    });
                }
                response.DidError = false;
                response.Message = "Message has been added!";
                response.Model = newMessage;
                return Ok(response);
            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageParticipants([FromQuery] int messageThreadID, [FromQuery] String clientCode)
        {
            ListResponse<ParticipantViewModel> response = new ListResponse<ParticipantViewModel>();
            List<User> userList = new List<User>();
            List<ParticipantViewModel> modelList = new List<ParticipantViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedMessageThread = await Task.Run(() =>
                {
                    return messageThreadRepository.GetMessageThread(messageThreadID);
                });
                if (selectedMessageThread == null)
                {
                    response.DidError = true;
                    response.Message = "No message thread was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }
                var participantList = await Task.Run(() =>
                {
                    return participantRepository.GetMessageParticipants(selectedMessageThread.MessageThreadID);
                });
                if (participantList == null)
                {
                    response.DidError = true;
                    response.Message = "No participants were found";
                    response.Model = null;
                    return NotFound();
                }
                foreach (MessageThreadParticipant participant in participantList)
                {
                    var selectedUser = await Task.Run(() =>
                    {
                        return houseMemberRepository.GetHouseMembership(participant.HouseMemberID);
                    });
                    if (selectedUser == null)
                    {
                        break;
                    }
                    var foundUser = await userManager.FindByIdAsync(Convert.ToString(selectedUser.UserID));
                    if (foundUser == null)
                    {
                        break;
                    }
                    ParticipantViewModel model = new ParticipantViewModel()
                    {
                        EmailAddress = foundUser.Email,
                        Name = foundUser.Name,
                        Surname = foundUser.Surname,
                        HouseMemberID = selectedUser.HouseMemberID,
                        IsDeleted = 0,
                        MessageThreadID = selectedMessageThread.MessageThreadID,
                        MessageThreadParticipantID = participant.MessageThreadParticipantID,
                        UserID = foundUser.Id
                    };
                    modelList.Add(model);
                    
                }
                if (participantList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No participant data was found";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Here are users that were found";
                    response.Model = modelList;
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
        public async Task<IActionResult> GetMessagesInThread([FromQuery] int messageThreadID, [FromQuery] String clientCode)
        {
            ListResponse<MessagesViewModel> response = new ListResponse<MessagesViewModel>();
            List<MessagesViewModel> messageList = new List<MessagesViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedMessageThread = await Task.Run(() =>
                {
                    return messageThreadRepository.GetMessageThread(messageThreadID);
                });
                if (selectedMessageThread == null)
                {
                    response.DidError = true;
                    response.Message = "No message threads were found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                var messages = await Task.Run(() =>
                {
                    return messageThreadMessageRepository.GetThreadMessages(selectedMessageThread.MessageThreadID);
                });
                if (messages == null)
                {
                    response.DidError = true;
                    response.Message = "No messages were foud for the thread";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (MessageThreadMessage item in messages)
                {
                    var houseMember = await Task.Run(() =>
                    {
                        return houseMemberRepository.GetHouseMembership(item.HouseMemberID);
                    });
                    if (houseMember == null)
                    {
                        break;
                    }
                    var selectedUser = await userManager.FindByIdAsync(Convert.ToString(houseMember.UserID));
                    if (selectedUser == null)
                    {
                        break;
                    }

                    MessagesViewModel model = new MessagesViewModel()
                    {
                        DateSent = item.DateSent,
                        EmailAddress = selectedUser.Email,
                        HouseMemberID = houseMember.HouseMemberID,
                        Message = item.Message,
                        MessageThreadID = selectedMessageThread.MessageThreadID,
                        Name = selectedUser.Name,
                        Surname = selectedUser.Surname,
                        MessageThreadMessageID = item.MessageThreadMessageID
                    };
                    messageList.Add(model);
                }
                if (messageList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No messages were found in this list";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Message = "Here are messages for this thread";
                    response.Model = messageList;
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
        public async Task<IActionResult> GetMessageThread([FromQuery] String emailAddress, [FromQuery] String clientCode)
        {
            ListResponse<MessageThread> response = new ListResponse<MessageThread>();
            List<MessageThread> messageList = new List<MessageThread>();
            List<House> houseList = new List<House>();
            List<HouseMember> houseListMemberships = new List<HouseMember>();
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
                    response.Message = "No user record found";
                    response.Model = null;
                    return NotFound(response);
                }
                var userMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMember(selectedUser.Id);
                });
                if (userMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "No house memberships have been found";
                    response.Model = null;
                    return NotFound(response);
                }

                foreach (HouseMember member in userMemberships)
                {
                    var messageThreads = await Task.Run(() =>
                    {
                        return messageThreadRepository.GetMessageThreadForMembership(member.HouseMemberID);
                    });
                    if (messageThreads != null)
                    {
                        foreach (MessageThread currentThread in messageThreads)
                        {
                            if (!messageList.Contains(currentThread))
                            {
                                messageList.Add(currentThread);
                            }
                        }
                    }
                }
                if (messageList.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "No messages have been found";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    response.DidError = false;
                    response.Model = messageList;
                    response.Message = "Herewith the messages";
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
