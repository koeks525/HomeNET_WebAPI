using HomeNetAPI.Models;
using System.Collections.Generic;

namespace HomeNetAPI.Repository
{
    public interface ICategoryRepository
    {
        Category CreateCategory(Category newCategory);
        Category DeleteCategory(int categoryID);
        Category GetCategory(int categoryID);
        List<Category> GetAllCategories();
    }
}
