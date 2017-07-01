using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Models;
using HomeNetAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace HomeNetAPI.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class OrganizationController : Controller 
    {
        private IOrganizationRepository organizationRepository;
        private String androidClient = "bab9baac6fac05ac083c5f42ec25d76d";
        private UserManager<User> userManager;
        

        public OrganizationController(IOrganizationRepository organizationRepository, UserManager<User> userManager)
        {
            this.organizationRepository = organizationRepository;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganization([FromBody] Organization NewOne, [FromQuery] String clientCode)
        {
            SingleResponse<Organization> response = new SingleResponse<Organization>();
            try
            {
                if (clientCode == androidClient)
                {
                    if (ModelState.IsValid)
                    {
                        var addResult = await Task.Run(() =>
                        {
                            return organizationRepository.AddOrganization(NewOne);
                        });
                        if (addResult != null)
                        {
                            response.DidError = false;
                            response.Message = $"New organization {addResult.Name} created successfully!";
                            response.Model = addResult;
                            return Ok(response);
                        } else
                        {
                            response.DidError = false;
                            response.Message = "Error adding new organization to the system";
                            response.Model = NewOne;
                            return BadRequest(response);
                        }
                    } else
                    {
                        response.DidError = true;
                        response.Message = "Please ensure data sent to the server is complete";
                        response.Model = NewOne;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetOrganizations([FromQuery] String clientCode)
        {
            ListResponse<Organization> response = new ListResponse<Organization>();
            try
            {
                if (clientCode == androidClient)
                {
                    var list = await Task.Run(() =>
                    {
                        return organizationRepository.GetOrganizations();

                    });
                    if (list != null)
                    {
                        response.DidError = false;
                        response.Message = "Here are the organizations on the system";
                        response.Model = list;
                        return Ok(response);
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No organizations were found on the system";
                        response.Model = null;
                        return NotFound(response);
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

        [HttpDelete]
        public async Task<IActionResult> DeleteOrganization([FromQuery] int organizationID, [FromQuery] String clientCode)
        {
            SingleResponse<Organization> response = new SingleResponse<Organization>();
            try
            {
                if (clientCode == androidClient)
                {
                    var foundOrganization = await Task.Run(() =>
                    {
                        return organizationRepository.GetOrganization(organizationID);
                    });
                    if (foundOrganization != null)
                    {
                        
                        var update = await Task.Run(() =>
                        {
                            return organizationRepository.DeleteOrganization(foundOrganization.OrganizationID);
                        });
                        if (update != null)
                        {
                            response.DidError = false;
                            response.Message = "Organization deleted successfully!";
                            response.Model = update;
                            return Ok(response);
                        } else
                        {
                            response.DidError = true;
                            response.Message = "Something went wrong with deleting the organization";
                            response.Model = foundOrganization;
                            return BadRequest(response);
                        }
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No organization was found with the linked code";
                        response.Model = null;
                        return NotFound(response);
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
        public async Task<IActionResult> GetUserOrganizations([FromQuery] String emailAddress, [FromQuery] int clientCode)
        {
            ListResponse<Organization> response = new ListResponse<Organization>();
            try
            {
                var foundUser = await userManager.FindByEmailAsync(emailAddress);
                if (foundUser != null)
                {
                    var organization = await Task.Run(() =>
                    {
                        return organizationRepository.GetUserOrganizations(foundUser.Id);
                    });
                    if (organization != null)
                    {
                        response.DidError = false;
                        response.Message = "Here are a list of the users organizations";
                        response.Model = organization;
                        return Ok(response);
                    } else
                    {
                        response.DidError = true;
                        response.Message = "No organizations were found for the selected user";
                        response.Model = null;
                        return NotFound(response);
                    }

                } else
                {
                    response.DidError = true;
                    response.Message = "No user was found with the given email addess. Please try again";
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
    }
}
