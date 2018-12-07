using System;
using System.Collections.Generic;

namespace emensa.Models
{
    public class ProductsModel
    {
        public List<Tuple<Meal, Image>> Meals;
        public List<Category> Categories;
    }
}