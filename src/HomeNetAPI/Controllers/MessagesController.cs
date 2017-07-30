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
                var selectedMembership = houseMemberships.First(i => i.HouseID == newMessage.HouseID);
                if (selectedMembership == null)
                {
                    response.DidError = true;
                    response.Message = "You are not subscribed to the selected house. Please subscribe";
                    response.Model = null;
                    return NotFound(response);
                }
                if (newMessage.Participants == null)
                {
                    response.DidError = true;
                    response.Message = "Please add recepients to the list";
                    response.Model = null;
                    return NotFound(response);
                }
               
                foreach (MessageThreadParticipant participant in newMessage.Participants)
                {
                    var result = await Task.Run(() =>
                    {
                        return houseMemberRepository.GetHouseMembership(participant.HouseMemberID);

                    });
                    if (result != null)
                    {
                        recepients.Add(result);
                    }
                }
                if (recepients.Count <= 0)
                {
                    response.DidError = true;
                    response.Message = "Please add recepients to the list";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HouseMember member in recepients)
                {
                    var result = await userManager.FindByIdAsync(Convert.ToString(member.UserID));
                    if (result != null)
                    {
                        recepientUsers.Add(result);
                    }
                }
                MessageThread newThread = new MessageThread()
                {
                    HouseMemberID = selectedMembership.HouseMemberID,
                    IsDeleted = 0,
                    Title = newMessage.ThreadTitle,
                    Priority = 0,
                    MessageThreadID = 0,
                    HouseMember = selectedMembership
                };
                var createThread = await Task.Run(() =>
                {
                    return messageThreadRepository.CreateMessageThread(newThread);
                });
                if (createThread == null)
                {
                    response.DidError = true;
                    response.Message = "Error Creating Message. Please try again later";
                    response.Model = null;
                    return BadRequest(response);
                }
                MessageThreadMessage firstMessage = new MessageThreadMessage();
                firstMessage.HouseMemberID = selectedMembership.HouseMemberID;
                firstMessage.Message = newMessage.ThreadMessage;
                firstMessage.MessageThreadID = createThread.MessageThreadID;
                firstMessage.DateSent = DateTime.Now.ToString();
                firstMessage.MessageThreadMessageID = 0;
                var firstMessageCall = await Task.Run(() =>
                {
                    return messageThreadMessageRepository.AddMessageToThread(firstMessage);
                });
                if (firstMessageCall == null)
                {
                    response.DidError = true;
                    response.Message = "Error Adding message to thread";
                    response.Model = null;
                    return BadRequest(response);
                }
                bool finalResponse = false;
                foreach (User user in recepientUsers)
                {
                    MessageThreadParticipant participant = new MessageThreadParticipant();
                    participant.HouseMemberID = recepients.First(i => i.UserID == user.Id).HouseID;
                    participant.IsDeleted = 0;
                    participant.MessageThreadID = createThread.MessageThreadID;
                    participant.MessageThreadParticipantID = 0;
                    var participantCall = await Task.Run(() =>
                    {
                        return participantRepository.AddParticipant(participant);
                    });
                    if (participantCall == null)
                    {
                        response.DidError = true;
                        response.Message = "Error adding participant to the message";
                        response.Model = null;
                        return BadRequest(response);
                    }
                    var mailResponse = mailService.SendMailMessage(user.Email, $"{user.Name}", $"{selectedHouse.Name}: New Message", $"Hi {user.Name}, \n\nA new message was sent to you by {selectedUser.Name} on the {DateTime.Now.ToString()}. The contents of this message reads: \n\n {newMessage.ThreadMessage}\n\n. Please login to the application to respond to this message. \nRegards,\nHomeNET Administrative Services");

                    var firebaseCall = await Task.Run(() =>
                    {
                        return messagingService.SendFirebaseMessage($"{selectedHouse.Name}: New Message", $"A new message has been sent to you by {selectedUser.Name}. Tap to read", user.FirebaseMessagingToken, key);
                    });
                    if (firebaseCall)
                    {
                        finalResponse = true;
                    } else
                    {
                        finalResponse = false;
                        break; //Get out the loop
                    }

                }
                if (finalResponse)
                {
                    response.DidError = false;
                    response.Message = "Message Conversation Created Successfully! Recepients have been notified!";
                    response.Model = createThread;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with creating the new message";
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

        [HttpPost]
        public async Task<IActionResult> AddMessageToThread([FromBody] MessageViewModel messageModel, [FromQuery] String clientCode)
        {
            SingleResponse<MessageThreadMessage> response = new SingleResponse<MessageThreadMessage>();
            try
            {
                List<User> participantList = new List<User>();
                List<HouseMember> memberList = new List<HouseMember>();
                if (clientCode == androidClient)
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
                MessageThreadMessage newMessage = new MessageThreadMessage()
                {
                    DateSent = DateTime.Now.ToString(),
                    HouseMemberID = messageModel.HouseMemberID,
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

                foreach (HouseMember participant in participants)
                {
                    var foundUser = await userManager.FindByIdAsync(Convert.ToString(participant.UserID));
                    if (foundUser != null)
                    {
                        participantList.Add(foundUser);
                    }
                }
                if (participantList == null)
                {
                    response.DidError = true;
                    response.Message = "No participants were found for the selected thread";
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
    }
}
