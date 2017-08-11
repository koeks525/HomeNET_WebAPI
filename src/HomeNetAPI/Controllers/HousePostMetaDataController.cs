using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HomeNetAPI.ViewModels;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class HousePostMetaDataController : Controller
    {
        private IHousePostMetaDataRepository metaDataRepository;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private UserManager<User> userManager;
        private IHousePostRepository housePostRepository;
        private ICommentRepository commentRepository;

        public HousePostMetaDataController(IHousePostMetaDataRepository metaDataRepository, UserManager<User> userManager, IHousePostRepository housePostRepository, ICommentRepository commentRepository)
        {
            this.metaDataRepository = metaDataRepository;
            this.userManager = userManager;
            this.housePostRepository = housePostRepository;
            this.commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterLike([FromQuery] int housePostID, [FromBody] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<HousePostMetaData> response = new SingleResponse<HousePostMetaData>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
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
                    response.Message = "No user was found with the given credentials. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                //If a user likes a post, they cannot dislike the post. 
                var houseMetaData = await Task.Run(() =>
                {
                    return metaDataRepository.GetHousePostMetaData(housePostID);
                });

                if (houseMetaData == null)
                {//No house post data exist - continue registering the like
                    HousePostMetaData data = new HousePostMetaData()
                    {
                        DateAdded = DateTime.Now.ToString(),
                        HousePostID = housePostID,
                        IsDeleted = 0,
                        HousePostMetaDataID = 0,
                        Liked = 1,
                        Disliked = 0,
                        UserID = selectedUser.Id,

                    };
                    var likeReg = await Task.Run(() =>
                    {
                        return metaDataRepository.RegisterLike(data);
                    });
                    if (likeReg == null)
                    {
                        response.DidError = true;
                        response.Message = "Something went wrong with registering your like. Please try again later";
                        response.Model = null;
                        return BadRequest(response);
                    } else
                    {
                        response.DidError = false;
                        response.Message = "Like registered successfully!";
                        response.Model = likeReg;
                        return Ok(response);
                    }
                } else
                {
                    foreach (HousePostMetaData houseData in houseMetaData)
                    {
                        if (houseData.UserID == selectedUser.Id && houseData.HousePostID == housePostID)
                        {
                            //A like record / or dislike record exists. If it is a dislike, turn it into a like. 
                            houseData.Liked = 1;
                            houseData.Disliked = 0;
                            var update = await Task.Run(() =>
                            {
                                return metaDataRepository.UpdateMetaData(houseData);
                            });

                            response.DidError = true;
                            response.Message = "Post Meta record updated successfully";
                            response.Model = update;
                            return Ok(response);
                        }
                    }

                    response.DidError = false;
                    response.Model = null;
                    response.Message = "There was an error with creating the like record. Please try again";
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
        public async Task<IActionResult> RegisterDislike([FromQuery] int housePostID, [FromBody] String emailAddress, [FromQuery] String clientCode)
        {
            SingleResponse<HousePostMetaData> response = new SingleResponse<HousePostMetaData>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
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
                    response.Message = "No user was found with the given credentials. Please try again";
                    response.Model = null;
                    return NotFound(response);
                }
                //If a user likes a post, they cannot dislike the post. 
                var houseMetaData = await Task.Run(() =>
                {
                    return metaDataRepository.GetHousePostMetaData(housePostID);
                });

                if (houseMetaData == null)
                {//No house post data exist - continue registering the like
                    HousePostMetaData data = new HousePostMetaData()
                    {
                        DateAdded = DateTime.Now.ToString(),
                        HousePostID = housePostID,
                        IsDeleted = 0,
                        HousePostMetaDataID = 0,
                        Liked = 0,
                        Disliked = 1,
                        UserID = selectedUser.Id,

                    };
                    var likeReg = await Task.Run(() =>
                    {
                        return metaDataRepository.RegisterLike(data);
                    });
                    if (likeReg == null)
                    {
                        response.DidError = true;
                        response.Message = "Something went wrong with registering your like. Please try again later";
                        response.Model = null;
                        return BadRequest(response);
                    }
                    else
                    {
                        response.DidError = false;
                        response.Message = "Like registered successfully!";
                        response.Model = likeReg;
                        return Ok(response);
                    }
                }
                else
                {
                    foreach (HousePostMetaData houseData in houseMetaData)
                    {
                        if (houseData.UserID == selectedUser.Id && houseData.HousePostID == housePostID)
                        {
                            //A like record / or dislike record exists. If it is a dislike, turn it into a like. 
                            houseData.Liked = 0;
                            houseData.Disliked = 1;
                            var update = await Task.Run(() =>
                            {
                                return metaDataRepository.UpdateMetaData(houseData);
                            });

                            response.DidError = true;
                            response.Message = "Post Meta record updated successfully";
                            response.Model = update;
                            return Ok(response);
                        }
                    }

                    response.DidError = false;
                    response.Model = null;
                    response.Message = "There was an error with creating the like record. Please try again";
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
        public async Task<IActionResult> GetPostData([FromQuery] int housePostID, [FromQuery] String clientCode)
        {
            SingleResponse<HousePostMetaDataViewModel> response = new SingleResponse<HousePostMetaDataViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client details to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var housePostMetaData = await Task.Run(() =>
                {
                    return metaDataRepository.GetHousePostMetaData(housePostID);
                });

                if (housePostMetaData == null)
                {
                    response.DidError = true;
                    response.Message = "No meta data was found for the selected house post. It could be that no one has liked or disliked a post";
                    response.Model = null;
                    return NotFound(response);
                }
                HousePostMetaDataViewModel model = new HousePostMetaDataViewModel()
                {
                    HousePostID = housePostID

                };
                foreach (HousePostMetaData metaData in housePostMetaData)
                {
                    if (metaData.Liked == 1)
                    {
                        model.TotalLikes++;
                    }
                    if (metaData.Disliked == 1)
                    {
                        model.TotalDislikes++;
                    }
                }
                response.DidError = false;
                response.Message = "Here are the metrics for the selected house";
                response.Model = model;
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
        public async Task<IActionResult> GetHousePostMetrics([FromQuery] int housePostID, [FromQuery] String clientCode)
        {
            SingleResponse<HousePostMetaDataViewModel> response = new SingleResponse<HousePostMetaDataViewModel>();
            try
            {
                if (clientCode != androidClient)
                {
                    response.DidError = true;
                    response.Message = "Please send valid client credentials to the server";
                    response.Model = null;
                    return BadRequest(response);
                }

                var housePost = await Task.Run(() =>
                {
                    return housePostRepository.GetHousePost(housePostID);
                });
                if (housePost == null)
                {
                    response.DidError = true;
                    response.Message = "No house posts were found";
                    response.Model = null;
                    return NotFound(response);
                }

                var metaData = await Task.Run(() =>
                {
                    return metaDataRepository.GetHousePostMetaData(housePost.HousePostID);
                });
                var comments = await Task.Run(() =>
                {
                    return commentRepository.GetComments(housePost.HousePostID);
                });

                if (metaData == null)
                {
                    HousePostMetaDataViewModel model = new HousePostMetaDataViewModel();
                    model.HousePostID = housePost.HousePostID;
                    model.TotalDislikes = 0;
                    model.TotalLikes = 0;
                    model.TotalComments = 0;
                    response.DidError = false;
                    response.Message = "No metric data has been found";
                    response.Model = model;
                    if (comments != null)
                    {
                        model.TotalComments = comments.Count;
                    }
                    return Ok(response);

                } else
                {
                    HousePostMetaDataViewModel model = new HousePostMetaDataViewModel();
                    int totalLikes = 0, totalDislikes = 0;
                    foreach (HousePostMetaData data in metaData)
                    {
                        if (data.Liked == 1)
                        {
                            totalLikes++;
                        } else
                        {
                            totalDislikes++;
                        }
                    }
                    model.TotalLikes = totalLikes;
                    model.TotalDislikes = totalDislikes;
                    model.TotalComments = 0;
                    if (comments != null)
                    {
                        model.TotalComments = comments.Count;
                    }
                    response.DidError = false;
                    response.Model = model;
                    response.Message = "Here are metric data";
                    return Ok(response);
                }

            } catch (Exception error)
            {
                response.DidError = true;
                response.Message = error.Message + " " + error.StackTrace;
                response.Model = null;
                return BadRequest(response);
            }
        }
    }
}
