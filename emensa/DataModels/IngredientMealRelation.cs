using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class IngredientMealRelation
    {
        public int IngredientId { get; set; }
        public int MealId { get; set; }

        public Ingredient Ingredient { get; set; }
        public Meal Meal { get; set; }
    }
}
