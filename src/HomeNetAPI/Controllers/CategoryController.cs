using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Repository;
using HomeNetAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace HomeNetAPI.Controllers
{
    [AllowAnonymous]
    [Route("/[controller]/[action]")]
    public class CategoryController : Controller
    {
        private ICategoryRepository categoryRepository;
        private string androidClient = "bab9baac6fac05ac083c5f42ec25d76d";

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCategory([FromBody] Category newCategory, [FromQuery] string clientCode)
        {
            SingleResponse<Category> response = new SingleResponse<Category>();
            if (clientCode == androidClient)
            {
                if (ModelState.IsValid)
                {
                    var cat = new Category()
                    {
                        CategoryID = 0,
                        Name = newCategory.Name,
                        IsDeleted = 0
                    };
                    var result = await Task.Run(() =>
                    {
                        return categoryRepository.CreateCategory(cat);
                    });
                    if (result != null)
                    {
                        response.DidError = false;
                        response.Message = "New Category has been created!";
                        response.Model = result;
                        return Ok(response);
                    }
                    else
                    {
                        response.DidError = true;
                        response.Model = newCategory;
                        response.Message = "Error submitting new category. Please try again";
                        return BadRequest(response);
                    }
                }
                else
                {
                    response.DidError = true;
                    response.Message = "Error occurred with category. Please ensure all data fields have valid data";
                    response.Model = newCategory;
                    return BadRequest(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                response.Model = newCategory;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory([FromQuery] int categoryID, [FromQuery] string clientCode)
        {
            SingleResponse<Category> response = new SingleResponse<Category>();
            if (clientCode == androidClient)
            {
                if (ModelState.IsValid)
                {
                    var result = await Task.Run(() =>
                    {
                        return categoryRepository.GetCategory(categoryID);
                    });
                    if (result == null)
                    {
                        response.DidError = true;
                        response.Message = "No category data was found with the provided ID";
                        response.Model = null;
                        return NotFound(response);
                    }
                    else
                    {
                        response.DidError = false;
                        response.Message = "We found the following category given the supplied ID";
                        response.Model = result;
                        return Ok(response);
                    }

                }
                else
                {
                    response.DidError = true;
                    response.Message = "Error occurred with category. Please ensure all data fields have valid data";
                    response.Model = null;
                    return BadRequest(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] string clientCode)
        {
            ListResponse<Category> response = new ListResponse<Category>();
            if (clientCode == androidClient)
            {
                var list = await Task.Run(() =>
                {
                    return categoryRepository.GetAllCategories();
                });
                if (list == null)
                {
                    response.DidError = true;
                    response.Message = "No categories were found";
                    response.Model = null;
                    return NotFound(response);
                }
                else
                {
                    response.DidError = false;
                    response.Message = "Here are categories found on our system";
                    response.Model = list;
                    return Ok(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                response.Model = null;
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory([FromQuery] int categoryID, [FromQuery] string clientCode)
        {
            SingleResponse<Category> response = new SingleResponse<Category>();
            if (clientCode == androidClient)
            {
                var category = await Task.Run(() =>
                {
                    return categoryRepository.DeleteCategory(categoryID);
                });
                if (category == null)
                {
                    response.DidError = true;
                    response.Message = "No categories were found given the supplied ID";
                    response.Model = null;
                    return NotFound(response);
                }
                else
                {
                    response.DidError = false;
                    response.Message = "We have successfully deleted category with ID " + categoryID;
                    response.Model = category;
                    return Ok(response);
                }
            } else
            {
                response.DidError = true;
                response.Message = "Please provide a valid client code to the server";
                return BadRequest(response);
            }
        }

    }
}
