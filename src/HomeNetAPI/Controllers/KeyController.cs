using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeNetAPI.Models;
using HomeNetAPI.Repository;

namespace HomeNetAPI.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class KeyController : Controller
    {
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private IKeyRepository keyRepository;

        public KeyController(IKeyRepository keyRepository)
        {
            this.keyRepository = keyRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddKey([FromBody] Key newKey, [FromQuery] string clientCode)
        {
            SingleResponse<Key> response = new SingleResponse<Key>();
            try
            {
                if (clientCode == androidClient)
                {
                    if (ModelState.IsValid)
                    {
                        var existingKey = await Task.Run(() =>
                        {
                            return keyRepository.GetKey(newKey.Name);

                        });
                        if (existingKey == null)
                        {
                            var result = await Task.Run(() =>
                            {
                                return keyRepository.AddKey(newKey);
                            });
                            if (result != null)
                            {
                                response.DidError = false;
                                response.Message = $"You key ({newKey.Name}) has been added successfully!";
                                response.Model = newKey;
                                return Ok(response);
                            }
                            else
                            {
                                response.DidError = true;
                                response.Message = "Error adding the key";
                                response.Model = newKey;
                                return BadRequest(response);
                            }
                        }
                        else
                        {
                            response.DidError = true;
                            response.Message = "The key you tried adding to the system already exists. Please try adding another key";
                            response.Model = newKey;
                            return BadRequest(response);

                        }
                    }
                    else
                    {
                        response.DidError = true;
                        response.Message = "Please ensure all fields have valid data";
                        response.Model = newKey;
                        return BadRequest(response);
                    }
                }
                else
                {
                    response.DidError = true;
                    response.Message = "Please provide a valid client code to the server";
                    response.Model = newKey;
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
        public async Task<IActionResult> GetKey([FromQuery] string keyName, [FromQuery] string clientCode)
        {
            SingleResponse<Key> response = new SingleResponse<Key>();
            try
            {
                if (clientCode == androidClient)
                {
                    var resultKey = await Task.Run(() =>
                    {
                        return keyRepository.GetKey(keyName);
                    });
                    if (resultKey == null)
                    {
                        response.DidError = true;
                        response.Message = "No key was found with the provided key name. Please try again";
                        response.Model = null;
                        return NotFound(response);
                    } else
                    {
                        response.DidError = false;
                        response.Message = "The following key was returned: ";
                        response.Model = resultKey;
                        return Ok(resultKey);
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
        public async Task<IActionResult> GetKeys([FromQuery] string clientCode)
        {
            ListResponse<Key> response = new ListResponse<Key>();
            try
            {
                if (clientCode == androidClient)
                {
                    var keyList = await Task.Run(() =>
                    {
                        return keyRepository.GetKeys();
                    });
                    if (keyList == null)
                    {
                        response.DidError = true;
                        response.Message = "No keys were found on the system";
                        response.Model = null;
                        return NotFound(response);
                    } else
                    {
                        response.DidError = false;
                        response.Message = "Here are keys found on the system";
                        response.Model = keyList;
                        return Ok(response);
                    }
                } else
                {
                    response.DidError = true;
                    response.Message = "PLease send a valid client code to the server";
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

        [HttpDelete]
        public async Task<IActionResult> DeleteKey([FromQuery] string keyName, [FromQuery] string clientCode)
        {
            SingleResponse<Key> response = new SingleResponse<Key>();
            try
            {
                var selectedKey = await Task.Run(() =>
                {
                    return keyRepository.GetKey(keyName);
                });
                if (selectedKey == null)
                {
                    response.DidError = true;
                    response.Message = "No key was found with the given key name";
                    response.Model = null;
                    return NotFound(response);
                } else
                {
                    var deleteKey = await Task.Run(() =>
                    {
                        return keyRepository.RemoveKey(keyName);
                    });
                    if (deleteKey == null)
                    {
                        response.DidError = true;
                        response.Message = "The key you tried deleting could not be deleted";
                        response.Model = null;
                        return NotFound(deleteKey);
                    } else
                    {
                        if (deleteKey.IsDeleted == 1)
                        {
                            response.DidError = false;
                            response.Message = $"Key {deleteKey.Name} has been deleted successfully";
                            response.Model = deleteKey;
                            return Ok(response);

                        } else
                        {
                            response.DidError = true;
                            response.Message = "Unexpected error while attempting to delete the key";
                            response.Model = deleteKey;
                            return BadRequest(response);
                        }
                    }
                }
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
