using System;
using System.Collections.Generic;
using System.Linq;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private HomeNetContext homeContext;

        public CategoryRepository(HomeNetContext homeContext)
        {
            this.homeContext = homeContext;
        }
        public Category CreateCategory(Category newCategory)
        {
            var result = homeContext.Categories.Add(newCategory);
            homeContext.SaveChanges();
            newCategory = result.Entity;
            return newCategory;
        }

        public Category DeleteCategory(int categoryID)
        {
            var category = homeContext.Categories.FirstOrDefault(c => c.CategoryID == categoryID);
            category.IsDeleted = 1;
            homeContext.SaveChanges();
            return category;
        }

        public List<Category> GetAllCategories()
        {
            return homeContext.Categories.ToList();
        }

        public Category GetCategory(int categoryID)
        {
            var category = homeContext.Categories.FirstOrDefault(c => c.CategoryID == categoryID);
            return category;
        }
    }
}
