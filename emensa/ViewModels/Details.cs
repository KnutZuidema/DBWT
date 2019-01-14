using System;
using System.Collections.Generic;
using System.Linq;
using emensa.DataModels;
using emensa.Utility;

namespace emensa.ViewModels
{
    public class Details
    {
        public Details(int id, Role role)
        {
            using (var db = new EmensaContext())
            {
                var relation = db.MealImageRelation.First(r => r.MealId == id);
                Image = db.Image.First(i => i.Id == relation.ImageId);
                Meal = db.Meal.First(m => m.Id == id);
                Ingredients = db.IngredientMealRelation.Where(r => r.MealId == id).ToList()
                    .ConvertAll(r => db.Ingredient.First(i => i.Id == r.IngredientId));
                var price = db.Price.First(p => p.MealId == id);
                switch (role)
                {
                    case Role.Student:
                        Price = price.StudentPrice;
                        break;
                    case Role.Employee:
                        Price = price.EmployeePrice;
                        break;
                    case Role.Guest:
                        Price = price.GuestPrice;
                        break;
                    default:
                        Price = price.GuestPrice;
                        break;
                }
            }
        }

        public Meal Meal { get; }
        public Image Image { get; }
        public List<Ingredient> Ingredients { get; }
        public decimal? Price { get; }
    }
}