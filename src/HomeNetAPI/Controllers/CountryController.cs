using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Models;
using HomeNetAPI.Repository;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace HomeNetAPI.Controllers
{
    [AllowAnonymous]
    [Route("/[controller]/[action]")]
    public class CountryController : Controller
    {
        private ICountryRepository countryRepository;
        private string androidClient = "bab9baac6fac05ac083c5f42ec25d76d";

        public CountryController(ICountryRepository countryRepository)
        {
            this.countryRepository = countryRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] Country newCountry, [FromQuery] string clientCode)
        {
            SingleResponse<Country> response = new SingleResponse<Country>();
            if (clientCode == androidClient)
            {
                if (ModelState.IsValid)
                {
                    
                    var country = await Task.Run(() =>
                    {
                        return countryRepository.AddCountry(newCountry);
                    });
                    if (country != null)
                    {
                        response.DidError = false;
                        response.Message = "New country inserted successfully";
                        response.Model = newCountry;
                        return Ok(response);
                    }
                    else
                    {
                        response.DidError = true;
                        response.Message = "Unexpected Error";
                        response.Model = newCountry;
                        return BadRequest(response);
                    }
                }
                else
                {
                    response = new SingleResponse<Country>()
                    {
                        DidError = true,
                        Message = ModelState.ToString(),
                        Model = newCountry

                    };
                    return BadRequest(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                response.Model = newCountry;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCountry([FromQuery] int countryId, [FromQuery] string clientCode)
        {
            SingleResponse<Country> response = new SingleResponse<Country>();
            if (clientCode == androidClient)
            {
                if (countryId != 0)
                {
                    var result = await Task.Run(() =>
                    {
                        return countryRepository.GetCountry(countryId);
                    });
                    if (result == null)
                    {
                        response.DidError = true;
                        response.Message = $"Country with ID number {countryId} does not exist";
                        response.Model = null;
                        return NotFound(response);
                    }
                    else
                    {
                        response.DidError = false;
                        response.Model = result;
                        response.Message = "Country has been found!";
                        return Ok(response);
                    }
                }
                else
                {
                    response.DidError = true;
                    response.Message = "Please supply a valid Country ID";
                    response.Model = null;
                    return BadRequest(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCountry([FromQuery] int countryId, [FromQuery]string clientCode)
        {
            SingleResponse<Country> response = new SingleResponse<Country>();
            if (clientCode == androidClient)
            {
                if (countryId != 0)
                {
                    var country = await Task.Run(() =>
                    {
                        return countryRepository.DeleteCountry(countryId);
                    });
                    if (country == null)
                    {
                        response.DidError = true;
                        response.Message = "The country you tried to remove does not exist in system records. Please try again";
                        response.Model = null;
                        return NotFound(response);
                    }
                    else
                    {
                        response.DidError = false;
                        response.Message = "Country has successfully been deleted!";
                        response.Model = country;
                        return Ok(response);
                    }
                }
                else
                {
                    response.DidError = true;
                    response.Message = "Please ensure you pass the correct country id";
                    response.Model = null;
                    return BadRequest(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries([FromQuery] string clientCode)
        {
            if (clientCode == androidClient)
            {
                var countryList = await Task.Run(() =>
                {
                    return countryRepository.GetCountries();

                });
                if (countryList != null)
                {
                    ListResponse<Country> response = new ListResponse<Country>();
                    response.DidError = false;
                    response.Message = "Here are countries found in our system";
                    response.Model = countryList;
                    return Ok(response);
                }
                else
                {
                    SingleResponse<Country> response = new SingleResponse<Country>();
                    response.DidError = true;
                    response.Message = "No countries were returned";
                    response.Model = null;
                    return NotFound(response);
                }
            } else
            {
                SingleResponse<string> response = new SingleResponse<string>();
                response.DidError = true;
                response.Message = "Please provide the server with a valid client code";
                return BadRequest(response);
            }
        }
    }

}
