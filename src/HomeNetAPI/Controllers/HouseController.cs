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

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class HouseController : Controller
    {
        private IHouseRepository houseRepository;
        private IUserRepository userRepository;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private String firebaseToken = " AIzaSyBhLv8gbKVzEIhtfYYSIcCRUkbS7z61qT0";
        private UserManager<User> userManager;
        private IHouseImageRepository profileRepository;
        private IImageProcessor imageProcessor;
        private IHouseMemberRepository houseMemberRepository;
        private IMailMessage mailService;
        private IFirebaseMessagingService firebaseService;

        public HouseController(IHouseRepository houseRepository, IUserRepository userRepository, UserManager<User> userManager, IHouseImageRepository profileRepository, IImageProcessor imageProcessor, IHouseMemberRepository houseMemberRepository, IMailMessage mailService, IFirebaseMessagingService firebaseService)
        {
            this.firebaseService = firebaseService;
            this.mailService = mailService;
            this.houseMemberRepository = houseMemberRepository;
            this.houseRepository = houseRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.profileRepository = profileRepository;
            this.imageProcessor = imageProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHouse([FromForm] string houseName, [FromForm] string description, [FromForm] string houseLocation, [FromForm] string emailAddress, [FromQuery] string clientCode, [FromForm] IFormFile imageFile)
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
                    Location = houseLocation,
                    DateCreated = DateTime.Now.ToString(),
                    OwnerID = selectedUser.Id,
                    User = selectedUser
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

                //Source: https://stackoverflow.com/questions/26741191/ioexception-the-process-cannot-access-the-file-file-path-because-it-is-being 
                using (var fileStream = new FileStream(directory+"/"+newFileName, FileMode.Create, FileAccess.ReadWrite))
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
                        response.DidError = false;
                        response.Message = $"House {houseName} has been created successfully!";
                        response.Model = finalResult;
                        return Ok(response);
                    } else
                    {
                        response.DidError = false;
                        response.Message = "Something went wrong with creating the house. ";
                        response.Model = createdHouse;
                        var remove = await Task.Run(() =>
                        {
                            return houseRepository.DeleteHouse(createdHouse.HouseID);
                        });
                        if (remove !=null)
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

        [HttpPut]
        public async Task<IActionResult> UpdateHouse([FromBody] House updateHouse, [FromQuery] string clientCode, [FromBody] IFormFile imageFile)
        {
            SingleResponse<House> response = new SingleResponse<House>();

            try
            {
                if (ModelState.IsValid)
                {
                    if (clientCode == androidClient)
                    {
                        var houseToUpdate = await Task.Run(() => { return houseRepository.GetHouse(updateHouse.HouseID); });
                        if (houseToUpdate != null)
                        {
                            houseToUpdate.Name = updateHouse.Name;
                            houseToUpdate.Description = updateHouse.Description;
                            houseToUpdate.Location = updateHouse.Location;
                            if (imageFile != null)
                            {
                                String houseDirectory = $"HomeNET/Houses/{houseToUpdate.HouseID}";
                                if (!Directory.Exists(houseDirectory))
                                {
                                    Directory.CreateDirectory(houseDirectory);
                                }

                                FileInfo fileInfo = new FileInfo(imageFile.FileName);
                                if (fileInfo.Length > 1e+7)
                                {
                                    response.DidError = true;
                                    response.Message = "The uploaded file cannot be greater tha 10MB in size. Please try another file";
                                    response.Model = updateHouse;
                                    return BadRequest(response);
                                }
                                if (fileInfo.Extension != "jpg" || fileInfo.Extension != "png")
                                {
                                    response.DidError = true;
                                    response.Message = "Only pictures should be uploaded. Only jpg or png files are accepted by the server";
                                    response.Model = updateHouse;
                                    return BadRequest(response);
                                }

                                //We are changing profile pictures... add the past image into the past photo gallery.
                                if (updateHouse.HouseImage != null)
                                {
                                    var profileImage = new HouseProfileImage()
                                    {
                                        HouseID = updateHouse.HouseID,
                                        HouseImage = updateHouse.HouseImage,
                                        HouseProfileImageID = 0,
                                        DateAdded = DateTime.Now.ToString(),
                                        IsDeleted = 0
                                    };
                                    var result = await Task.Run(() =>
                                    {
                                        return profileRepository.AddHouseProfileImage(profileImage);
                                    });

                                    String fileName = imageFile.Name + "." + fileInfo.Extension;
                                    String fileLocation = Path.Combine(houseDirectory, fileName);
                                    houseToUpdate.HouseImage = fileLocation;
                                    using (var stream = new FileStream(fileLocation, FileMode.Create, FileAccess.ReadWrite))
                                    {
                                        await imageFile.CopyToAsync(stream);
                                        int width, height, size = 512;
                                        var copiedFile = new SKManagedStream(stream);
                                        var fileToBeProcessed = SKBitmap.Decode(copiedFile);
                                        if (fileToBeProcessed.Width > fileToBeProcessed.Height)
                                        {
                                            width = size;
                                            height = fileToBeProcessed.Height * size / fileToBeProcessed.Width;
                                        } else
                                        {
                                            width = fileToBeProcessed.Width * size / fileToBeProcessed.Height;
                                            height = size;
                                        }
                                        var resizedFile = fileToBeProcessed.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3);
                                        if (resizedFile != null)
                                        {
                                           //Copy the file to the server
                                        }

                                        houseToUpdate.HouseImage = fileLocation;
                                        var finalUpdateResult = await Task.Run(() => { return houseRepository.UpdateHouse(houseToUpdate); });
                                        if (finalUpdateResult != null)
                                        {
                                            response.DidError = false;
                                            response.Message = "House has been updated successfully! House Image has been updated successfully!";
                                            response.Model = finalUpdateResult;
                                            return Ok(response);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var updateResult = await Task.Run(() => { return houseRepository.UpdateHouse(houseToUpdate); });
                            if (updateResult != null)
                            {
                                response.DidError = false;
                                response.Message = $"House {updateHouse.Name} has been updated successfully!";
                                response.Model = updateResult;
                                return Ok(response);
                            }
                            return Ok(); //Fix this later
                        }
                    }
                    else
                    {
                        response.DidError = true;
                        response.Message = "No matching house was found with the provided details";
                        response.Model = updateHouse;
                        return NotFound(response);
                    }
                }
                else
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
                    response.Model = updateHouse;
                    return BadRequest(response);


                }
             } catch(Exception error)
            {
                response.DidError = true;
                response.Message = error.Message;
                response.Model = null;
                return BadRequest(response);
            }
            return BadRequest();

        }
        [HttpGet]
        public async Task<IActionResult> GetHouses([FromQuery] string emailAddress, [FromQuery] string clientCode)
        {
            ListResponse<House> response = new ListResponse<House>();
            try
            {
                if (clientCode == androidClient)
                {
                    if (emailAddress != null)
                    {
                        var user = await Task.Run(() =>
                        {
                            return userRepository.GetUser(emailAddress);
                        });
                        if (user == null)
                        {
                            response.DidError = true;
                            response.Message = "No user records were found with the given email address";
                            response.Model = null;
                            return NotFound(response);
                        }
                        else
                        {
                            var listHouses = await Task.Run(() =>
                            {
                                return houseRepository.GetHouses(user.Id);
                            });
                            if (listHouses == null)
                            {
                                response.DidError = false;
                                response.Message = "No houses were found for the linked user. Perhaps you need to register a house?";
                                response.Model = null;
                                return Ok(response);
                            }
                            else
                            {
                                response.DidError = false;
                                response.Message = "We have found the following linked houses";
                                response.Model = listHouses;
                                return Ok(response);
                            }
                        }
                    }
                    else
                    {
                        response.DidError = true;
                        response.Message = "Please ensure you are sending through an email adress of the corresponding user";
                        response.Model = null;
                        return BadRequest(response);
                    }
                } else
                {
                    response.DidError = true;
                    response.Message = "Please send a valid client code to the server";
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
        public async Task<IActionResult> GetHouse([FromQuery] int houseID, [FromQuery] String clientCode)
        {
            SingleResponse<House> response = new SingleResponse<House>();
            try
            {
                if (clientCode == null)
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
                if (selectedHouse!=null)
                {
                    response.DidError = false;
                    response.Message = "Here is the selected house";
                    response.Model = selectedHouse;
                    return Ok(selectedHouse);
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
            SingleResponse<Object> response = new SingleResponse<Object>();
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
                    var foundImage = System.IO.File.OpenRead(selectedHouse.HouseImage);
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

                if (selectedHouse.OwnerID == selectedUser.Id)
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
                var administrator = await userManager.FindByIdAsync(Convert.ToString(selectedHouse.OwnerID));
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
                    bool sentToAdmin = mailService.SendMailMessage(administrator.Email, administrator.Name, $"{selectedHouse.Name}: New Join Request", message);
                    bool sentToUser = mailService.SendMailMessage(selectedUser.Email, selectedUser.Name, $"{selectedHouse.Name}: New Join Request Received", toUser);
                    if (administrator.FirebaseMessagingToken != null)
                    {
                        bool sent = await firebaseService.SendFirebaseMessage($"{selectedHouse.Name}: New User Join Request", $"{selectedUser.Name} has requested to join your house. Please attend to this", administrator.FirebaseMessagingToken, firebaseToken);
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

        
    }
}
 