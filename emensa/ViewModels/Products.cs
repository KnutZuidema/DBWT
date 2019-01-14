using System;
using System.Collections.Generic;
using System.Linq;
using emensa.DataModels;
using emensa.Utility;

namespace emensa.ViewModels
{
    public class Products
    {
        public Products()
        {
            using (var db = new EmensaContext())
            {
                Categories = (from category in db.Category select category).ToList();
                MealsAndImages = (from meal in db.Meal
                    join relation in db.MealImageRelation on meal.Id equals relation.MealId into leftJoin
                    from joined in leftJoin.DefaultIfEmpty()
                    select new Tuple<Meal, Image>(joined.Meal, joined.Image)).ToList();
            }
        }

        public List<Category> Categories { get; }
        public List<Tuple<Meal, Image>> MealsAndImages { get; }

        public bool IsVegetarian(Meal meal)
        {
            using (var db = new EmensaContext())
            {
                return (from relation in db.IngredientMealRelation
                    where relation.MealId == meal.Id
                    select relation.Ingredient)
                    .ToList()
                    .All(ingredient => ingredient.Vegetarian != 0);
            }
        }

        public bool IsVegan(Meal meal)
        {
            using (var db = new EmensaContext())
            {
                return (from relation in db.IngredientMealRelation
                        where relation.MealId == meal.Id
                        select relation.Ingredient)
                    .ToList()
                    .All(ingredient => ingredient.Vegan != 0);
            }
        }

        public bool IsOrganic(Meal meal)
        {
            using (var db = new EmensaContext())
            {
                return (from relation in db.IngredientMealRelation
                        where relation.MealId == meal.Id
                        select relation.Ingredient)
                    .ToList()
                    .All(ingredient => ingredient.Organic != 0);
            }
        }

        public bool IsGlutenFree(Meal meal)
        {
            using (var db = new EmensaContext())
            {
                return (from relation in db.IngredientMealRelation
                        where relation.MealId == meal.Id
                        select relation.Ingredient)
                    .ToList()
                    .All(ingredient => ingredient.GlutenFree != 0);
            }
        }
    }
}