using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HomeNetAPI.Models;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using SkiaSharp;
using HomeNetAPI.Services;
using HomeNetAPI.ViewModels;
using MimeKit;
using System.Collections.Generic;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class HouseController : Controller
    {
        private IHouseRepository houseRepository;
        private IHousePostRepository postRepository;
        private IHousePostMetaDataRepository metaDataRepository;
        private IUserRepository userRepository;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private String firebaseToken = " AIzaSyBhLv8gbKVzEIhtfYYSIcCRUkbS7z61qT0";
        private UserManager<User> userManager;
        private IHouseImageRepository profileRepository;
        private IImageProcessor imageProcessor;
        private IHouseMemberRepository houseMemberRepository;
        private IMailMessage mailService;
        private IFirebaseMessagingService firebaseService;
        private IAnnouncementRepository announcementRepository;
        private ICommentRepository commentRepository;

        public HouseController(IHouseRepository houseRepository, IUserRepository userRepository, UserManager<User> userManager, IHouseImageRepository profileRepository, IImageProcessor imageProcessor, IHouseMemberRepository houseMemberRepository, IMailMessage mailService, IFirebaseMessagingService firebaseService, IHousePostRepository postRepository, IHousePostMetaDataRepository metaDataRepository, IAnnouncementRepository announcementRepository, ICommentRepository commentRepository)
        {
            this.announcementRepository = announcementRepository;
            this.postRepository = postRepository;
            this.metaDataRepository = metaDataRepository;
            this.firebaseService = firebaseService;
            this.mailService = mailService;
            this.houseMemberRepository = houseMemberRepository;
            this.houseRepository = houseRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.profileRepository = profileRepository;
            this.imageProcessor = imageProcessor;
            this.commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHouse([FromForm] string houseName, [FromForm] string description, [FromForm] string emailAddress, [FromQuery] string clientCode, [FromForm] IFormFile imageFile)
        {
            SingleResponse<House> response = new SingleResponse<House>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                if (houseName == null || description == null || emailAddress == null || imageFile == null)
                {
                    response.DidError = true;
                    response.Message = "Please ensure the request sends a valid house name, description and identification information";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the given identification data";
                    response.Model = null;
                    return NotFound(response);
                }

                if (imageFile.ContentType != "image/jpeg")
                {
                    response.DidError = true;
                    response.Message = "Only image files are accepted by the server. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                }

                if (imageFile.Length > 2.5e+7)
                {
                    response.DidError = true;
                    response.Message = "Image files cannot exceed 25MB in size. Please try again";
                    response.Model = null;
                    return BadRequest(response);
                }

                var newHouse = new House()
                {
                    HouseID = 0,
                    Name = houseName,
                    Description = description,
                    DateCreated = DateTime.Now.ToString(),
                    UserID = selectedUser.Id,
                    User = selectedUser,
                    IsDeleted = 0
                };

                var createdHouse = await Task.Run(() =>
                {
                    return houseRepository.CreateHouse(newHouse);
                });
                if (createdHouse == null)
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with creating the house";
                    response.Model = newHouse;
                    return BadRequest(response);
                }

                String directory = $"C:/HomeNET/Houses/{createdHouse.HouseID}";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                String newFileName = imageFile.FileName;
                //The compressed file has got colons - this is giving problems with C#
                if (imageFile.FileName.Contains(":"))
                {
                    newFileName = imageFile.FileName.Replace(":", "_");
                }
                newFileName = GenerateRandomString() + newFileName;
                //Source: https://stackoverflow.com/questions/26741191/ioexception-the-process-cannot-access-the-file-file-path-because-it-is-being 
                using (var fileStream = new FileStream(directory + "/" + newFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    await imageFile.CopyToAsync(fileStream);
                    createdHouse.HouseImage = directory + "/" + newFileName;
                }


                var finalResult = await Task.Run(() =>
                {
                    return houseRepository.UpdateHouse(createdHouse);
                });
                if (finalResult != null)
                {
                    var houseMembership = new HouseMember()
                    {
                        HouseID = finalResult.HouseID,
                        DateApplied = DateTime.Now.ToString(),
                        DateApproved = DateTime.Now.ToString(),
                        UserID = selectedUser.Id,
                        ApprovalStatus = 1,

                    };
                    var addMember = await Task.Run(() =>
                    {
                        return houseMemberRepository.AddHouseMember(houseMembership);
                    });
                    if (addMember != null)
                    {
                        response.DidError = false;
                        response.Message = $"House {houseName} has been created successfully!";
                        response.Model = finalResult;
                        return Ok(response);
                    } else
                    {
                        response.DidError = false;
                        response.Message = $"House {houseName} has been created successfully! Membership couldn't be added";
                        response.Model = finalResult;
                        return Ok(response);
                    }
                    
                } else
                {
                    response.DidError = false;
                    response.Message = "Something went wrong with creating the house. ";
                    response.Model = createdHouse;
                    var remove = await Task.Run(() =>
                    {
                        return houseRepository.DeleteHouse(createdHouse.HouseID);
                    });
                    if (remove != null)
                    {
                        response.Message += "Changes made have been rolled back";
                        return BadRequest(response);
                    }
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

        [HttpPost]
        public async Task<IActionResult> UpdateHouse([FromQuery] int houseID, [FromForm] String houseName, [FromForm] String houseDescription, [FromForm] String emailAddress, [FromForm] int isPrivate, [FromForm] String oneTimePin, [FromForm] IFormFile imageFile, [FromQuery] String clientCode)
        {
            SingleResponse<House> response = new SingleResponse<House>();
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
                    response.Message = "No house was found with the provided credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the supplied credentials";
                    response.Model = null;
                    return NotFound(response);
                }
                if (selectedHouse.UserID != selectedUser.Id)
                {
                    response.DidError = true;
                    response.Model = null;
                    response.Message = "You are not the administrator of the house. Only the house admin can modify house settings";
                    return BadRequest(response);
                }
                if (selectedHouse.HouseImage == null && imageFile == null)
                {
                    response.DidError = true;
                    response.Message = "Your house was created without a profile picture. Please add a picture to the house";
                    response.Model = null;
                    return BadRequest(response);
                }
                if (imageFile == null)
                {
                    House updateHouse = new House();
                    updateHouse.HouseID = selectedHouse.HouseID;
                    updateHouse.Description = houseDescription.Trim();
                    updateHouse.Name = houseName;
                    updateHouse.IsPrivate = isPrivate;
                    if (oneTimePin != null)
                    {
                        updateHouse.OneTimePin = oneTimePin;
                    }
                    updateHouse.IsDeleted = 0;
                    updateHouse.HouseImage = selectedHouse.HouseImage;
                    updateHouse.DateCreated = selectedHouse.DateCreated;

                    var updateHouseTask = await Task.Run(() =>
                    {
                        return houseRepository.UpdateHouse(updateHouse);
                    });
                    if (updateHouseTask == null)
                    {
                        response.DidError = true;
                        response.Message = "An error occurred updating the house. Please try again";
                        response.Model = null;
                        BadRequest(response);
                    }
                    else
                    {
                        response.DidError = true;
                        response.Message = "House update successfully!";
                        response.Model = updateHouseTask;
                        return Ok(response);
                    }
                }
                else
                {
                    String oldFileLocation = selectedHouse.HouseImage;
                    String directory = $"C:/HomeNET/Houses/{selectedHouse.HouseID}";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    String fileName = imageFile.FileName;
                    if (fileName.Contains(":"))
                    {
                        fileName = fileName.Replace(":", "_");
                    }
                    if (imageFile.Length > 1e+7)
                    {
                        response.DidError = true;
                        response.Message = "Uploaded file cannot be larger than 10MB";
                        response.Model = null;
                        return BadRequest(response);
                    }
                    if (imageFile.ContentType != "image/jpeg")
                    {
                        response.DidError = true;
                        response.Message = "Only image files are accepted at this point. ";
                        response.Model = null;
                        return BadRequest(response);
                    }

                    using (var fileStream = new FileStream(directory + "/" + fileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await imageFile.CopyToAsync(fileStream);
                        House finalHouse = new House();
                        finalHouse.HouseID = selectedHouse.HouseID;
                        finalHouse.Description = houseDescription;
                        finalHouse.Name = houseName;
                        finalHouse.IsDeleted = 0;
                        finalHouse.IsPrivate = isPrivate;
                        finalHouse.HouseImage = directory + "/" + fileName;
                        if (oneTimePin != null)
                        {
                            finalHouse.OneTimePin = oneTimePin;
                        }
                        var updateHouseTask = await Task.Run(() =>
                        {
                            return houseRepository.UpdateHouse(finalHouse);
                        });
                        if (updateHouseTask == null)
                        {
                            response.DidError = true;
                            response.Message = "Error updating the selected house. Please try again";
                            response.Model = null;
                            return BadRequest(response);
                        }
                        else
                        {
                            response.DidError = false;
                            response.Message = "House updated successfully!";
                            response.Model = null;
                            return Ok(response);
                        }
                    }
                    
                }
            }
             catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "\n" + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }

            return BadRequest(response);
        }
    
        
        [HttpGet]
        public async Task<IActionResult> GetHouses([FromQuery] string emailAddress, [FromQuery] string clientCode)
        {
            ListResponse<House> response = new ListResponse<House>();
            try
            {
                if (androidClient != clientCode)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                if (emailAddress == "" || emailAddress == null)
                {
                    response.DidError = true;
                    response.Message = "Please send an email address to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the provided credentials";
                    response.Model = null;
                    return NotFound(response);
                }

                var selectedHouses = await Task.Run(() =>
                {
                    return houseRepository.GetHouses(selectedUser.Id);
                });
                if (selectedHouses != null)
                {
                    response.DidError = false;
                    response.Model = selectedHouses;
                    response.Message = "Here are houses linked to the selected user";
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No houses were found for the selected user";
                    response.Model = null;
                    return NotFound(response);
                }
            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.StackTrace + "\n" + error.Message;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet] 
        public async Task<IActionResult> GetHouse([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            SingleResponse<House> response = new SingleResponse<House>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client details to the server";
                    response.Model = null;
                    return BadRequest(response);
                }
                var selectedHouse = await Task.Run(() =>
                {
                    return houseRepository.GetHouse(houseID);
                });
                if (selectedHouse != null)
                {
                    response.DidError = false;
                    response.Message = "Here is the selected house";
                    response.Model = selectedHouse;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No house was found with the given ID";
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
        public async Task<IActionResult> GetHouseProfileImage([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            SingleResponse<FileStream> response = new SingleResponse<FileStream>();
            try
            {
               if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client details to the server";
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
                    response.Message = "No house was found with the given details";
                    response.Model = null;
                    return NotFound(response);
                }
                if (selectedHouse.HouseImage != null)
                {
                    var foundImage = System.IO.File.Open(selectedHouse.HouseImage, FileMode.Open, FileAccess.Read);
                    if (foundImage != null)
                    {
                        return File(foundImage, "image/jpeg");
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No profile image was found with the house";
                        response.Model = null;
                        return NotFound(response);
                    }
                } else
                {
                    response.DidError = true;
                    response.Message = "No Profile image meta data was found";
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
        public async Task<IActionResult> SearchHouses([FromBody] SearchViewModel searchParameter, [FromQuery] String clientCode)
        {
            ListResponse<House> response = new ListResponse<House>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedHouses = await Task.Run(() =>
                {
                    return houseRepository.SearchHouses(searchParameter.searchParameters);
                });
                if (selectedHouses != null)
                {
                    response.DidError = false;
                    response.Message = "Here are found houses";
                    response.Model = selectedHouses;
                    return Ok(response);
                } else
                {
                    response.DidError = false;
                    response.Model = null;
                    response.Message = "No houses were found with the given parameters";
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
        public async Task<IActionResult> JoinHouse([FromQuery] int houseID, [FromQuery] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<HouseMember> response = new SingleResponse<HouseMember>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid android client details to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the provided details";
                    response.Model = null;
                    return BadRequest(response);
                }
                //We need to check if the user is not a member of the house (regardless of joined, pending, or deleted). 
                var selectedHouse = await Task.Run(() => { return houseRepository.GetHouse(houseID); });
                if (selectedHouse == null)
                {
                    response.DidError = true;
                    response.Message = "The house you wish to join does not exist in system records";
                    response.Model = null;
                    return NotFound(response);
                }

                if (selectedHouse.UserID == selectedUser.Id)
                {
                    response.DidError = true;
                    response.Message = "You are the administrator of the house. You cannot join as a member of the house";
                    response.Model = null;
                    return BadRequest(response);
                }
                var memberships = await Task.Run(() => { return houseMemberRepository.GetHouseMemberships(houseID); });
                if (memberships != null)
                {
                    foreach (HouseMember membership in memberships) {
                        if (membership.UserID == selectedUser.Id)
                        {
                            response.DidError = true;
                            response.Message = "You are already subscribed to this house. If you are banned or still awaiting approval, please wait for the house administrator's response";
                            response.Model = null;
                            return BadRequest(response);
                        }
                    }
                }
                var administrator = await userManager.FindByIdAsync(Convert.ToString(selectedHouse.UserID));
                HouseMember member = new HouseMember();
                var joinHouseTask = await Task.Run(() =>
                {
                    member = new HouseMember()
                    {
                        HouseID = selectedHouse.HouseID,
                        DateApplied = DateTime.Now.ToString(),
                        ApprovalStatus = 1,
                        IsDeleted = 0,
                        UserID = selectedUser.Id,
                        HouseMemberID = 0,

                    };
                    return houseMemberRepository.AddHouseMember(member);
                });
                if (joinHouseTask != null)
                {
                    String message = $"Hi {administrator.UserName}\n\nOn the {DateTime.Now.ToString()}, a new user requested to join your house. As the system administrator, you have the option of approving the request or denying the request. For your convenience, here are some basic details to the user: \n\nUsername: {selectedUser.UserName}\nEmail Address: {selectedUser.Email}\nName: {selectedUser.Name}\n\nIf you have not received the push notification on your device, please login to the mobile app and deal with the request.\n\nRegards,\nHomeNET Administratative Services";
                    String toUser = $"Hi {selectedUser.Name},\n\nThank you for taking interest in {selectedHouse.Name}. The administrator has received your join request, and will be attending to this shortly.\nRegards,\nHomeNET Administrative Services";
                    bool sentToAdmin = mailService.SendMailMessage(administrator.Name, administrator.Email, $"{selectedHouse.Name}: New Join Request", message);
                    bool sentToUser = mailService.SendMailMessage(selectedUser.Name, selectedUser.Email, $"{selectedHouse.Name}: New Join Request Received", toUser);
                    if (administrator.FirebaseMessagingToken != null)
                    {
                        bool sent = await firebaseService.SendFirebaseMessage(3, $"{selectedHouse.Name}: New User Join Request", $"{selectedUser.Name} has requested to join your house. Please attend to this", administrator.FirebaseMessagingToken, firebaseToken);
                        response.DidError = false;
                        response.Message = $"You have successfully requested membership for {selectedHouse.Name}. The admin has been notified and will respond to your request. ";
                        response.Model = member;
                        return Ok(response);
                    }
                    //Notification to phone here
                    response.DidError = false;
                    response.Message = "New House join has been processed. Please wait for the administrator";
                    response.Model = member;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Your house join could not be processed. Please try again later";
                    response.Model = null;
                    return BadRequest(response);
                }



            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + "@Line " + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetSubscribedHouses([FromForm] String emailAddress, [FromQuery] String clientCode)
        {
            ListResponse<House> response = new ListResponse<House>();
            try
            {
                List<House> houseList = new List<House>();
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
                    response.Message = "No user was found with the provided information";
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
                    response.Message = "No memberships were found for the selected user. Please try joining a house";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach(HouseMember member in houseMemberships)
                {
                    var house = await Task.Run(() =>
                    {
                        return houseRepository.GetHouse(member.HouseID);
                    });
                    if (house != null)
                    {
                        houseList.Add(house);
                    }
                }
                if (houseList.Count > 0)
                {
                    response.DidError = false;
                    response.Message = "Herewith the subscribed houses";
                    response.Model = houseList;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No houses found";
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
        public async Task<IActionResult> GetUsersInHouse([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            ListResponse<UserViewModel> response = new ListResponse<UserViewModel>();
            List<UserViewModel> userList = new List<UserViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return NotFound(response);
                }

                var houseMembers = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(houseID);
                });
                if (houseMembers == null)
                {
                    response.DidError = true;
                    response.Model = null;
                    response.Message = "No house members were found for the selected house";
                    return NotFound(response);
                }
                foreach (HouseMember member in houseMembers)
                {
                    var foundUser = await userManager.FindByIdAsync(Convert.ToString(member.UserID));
                    if (foundUser != null)
                    {
                        UserViewModel model = new UserViewModel()
                        {
                            EmailAddress = foundUser.Email,
                            Name = foundUser.Name,
                            Surname = foundUser.Surname,
                            UserID = foundUser.Id,
                            UserName = foundUser.UserName,
                            ProfilePicture = foundUser.ProfileImage
                        };
                        userList.Add(model);
                    }
                }
                if (userList.Count > 0)
                {
                    response.DidError = false;
                    response.Model = userList;
                    response.Message = "Found users";
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "No users were found";
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
        public async Task<IActionResult> GenerateHouseMetricsReport([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            SingleResponse<HouseViewModel> response = new SingleResponse<HouseViewModel>();
            List<HousePost> housePostList = new List<HousePost>();
            List<HousePostComment> commentList = new List<HousePostComment>();
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
                    response.Message = "No house was found with the provided data";
                    response.Model = null;
                    return NotFound(response);
                }
                var selectedOwner = await userManager.FindByIdAsync(Convert.ToString(selectedHouse.UserID));
                if (selectedOwner == null)
                {
                    response.DidError = true;
                    response.Message = "No owner data was found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                var houseMembers = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (houseMembers == null)
                {
                    response.DidError = true;
                    response.Message = "No house members were found, therefore this report is invalid";
                    response.Model = null;
                    return NotFound(response);
                }
                foreach (HouseMember member in houseMembers)
                {
                    var postsFound = await Task.Run(() =>
                    {
                        return postRepository.GetHousePosts(member.HouseMemberID);
                    });
                    if (postsFound != null)
                    {
                        foreach (HousePost foundPost in postsFound)
                        {
                            housePostList.Add(foundPost);
                        }
                    }
                }
                var announcements = await Task.Run(() =>
                {
                    return announcementRepository.GetHouseAnnouncements(selectedHouse.HouseID);
                });
                foreach (HousePost thisPost in housePostList)
                {
                    var comments = await Task.Run(() =>
                    {
                        return commentRepository.GetComments(thisPost.HousePostID);
                    });
                    if (comments != null)
                    {
                        foreach (HousePostComment comment in comments)
                        {
                            commentList.Add(comment);
                        }
                    }
                }

                var bannedMembers = await Task.Run(() =>
                {
                    return houseMemberRepository.GetBannedHouseMembers(selectedHouse.HouseID);
                });

                var model = new HouseViewModel()
                {
                    HouseID = selectedHouse.HouseID,
                    Name = selectedHouse.Name,
                    Description = selectedHouse.Description,
                    DateCreated = selectedHouse.DateCreated,
                    Owner = selectedOwner.Name + " " + selectedOwner.Surname,
                    HouseImage = selectedHouse.HouseImage,
                    TotalAnnouncements = announcements.Count,
                    TotalComments = commentList.Count,
                    TotalMembers = houseMembers.Count,
                    TotalPosts = housePostList.Count,
                };
                if (bannedMembers != null)
                {
                    model.BannedMembers = bannedMembers.Count;
                } else
                {
                    model.BannedMembers = 0;
                }
                response.DidError = false;
                response.Message = "Herewith house metrics";
                response.Model = model;
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
        public async Task<IActionResult> LeaveHouse([FromQuery] int houseID, [FromQuery] String emailAddress, [FromQuery] String clientCode)
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

                var selectedUser = await userManager.FindByEmailAsync(emailAddress);
                if (selectedUser == null)
                {
                    response.DidError = true;
                    response.Message = "No user was found with the provided data";
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
                var houseMemberships = await Task.Run(() =>
                {
                    return houseMemberRepository.GetHouseMemberships(selectedHouse.HouseID);
                });
                if (houseMemberships == null)
                {
                    response.DidError = true;
                    response.Message = "The selected house does not have any memberships. Please try another house";
                    response.Model = null;
                    return NotFound(response);
                }
                HouseMember userMembership = null;
                foreach (HouseMember member in houseMemberships)
                {
                    if (member.UserID == selectedUser.Id && member.HouseID == selectedHouse.HouseID)
                    {
                        userMembership = member;
                        break;
                    }
                }
                if (userMembership == null)
                {
                    response.DidError = true;
                    response.Message = "No house membership for the selected user was found for the selected house";
                    response.Model = null;
                    return NotFound(response);
                }
                userMembership.DateLeft = DateTime.Now.ToString();
                userMembership.ApprovalStatus = 3;
                var update = await Task.Run(() =>
                {
                    return houseMemberRepository.UpdateMembership(userMembership);
                });
                if (update != null)
                {
                    response.DidError = false;
                    response.Message = "House member has left";
                    response.Model = update;
                    return Ok(response);
                } else
                {
                    response.DidError = true;
                    response.Message = "Something went wrong with updating house membership";
                    response.Model = null;
                    return BadRequest(response);
                }

            }
            catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + " " + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpPost]
        private String GenerateRandomString()
        {
            Random random = new Random();
            String finalString = "";
            for (int a = 0; a < 10; a++)
            {
                finalString += Convert.ToString(random.Next(1, 50));
            }
            return finalString.Trim();
        }
    }
}
 