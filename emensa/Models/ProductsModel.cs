using System;
using System.Collections.Generic;

namespace emensa.Models
{
    public class ProductsModel
    {
        public List<Tuple<Meal, Image>> Meals { get; set; }
        public List<Category> Categories { get; set; }

        public ProductsModel()
        {
            Meals = Meal.GetAllWithImage();
            Categories = Category.GetAll();
        }
    }
}