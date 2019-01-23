using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using emensa.DataModels;
using emensa.Utility;

namespace emensa.ViewModels
{
    public class Order
    {
        public string PickupTime { get; set; }
        public Dictionary<string, int> Meals { get; set; }

        public Dictionary<Meal, int> GetMeals()
        {
            if (Meals is null)
            {
                return new Dictionary<Meal, int>();
            }

            using (var db = new EmensaContext())
            {
                var meals = new Dictionary<Meal, int>();
                foreach (var (mealId, amount) in Meals)
                {
                    var meal = db.Meal.FirstOrDefault(m => m.Id == Convert.ToInt32(mealId));
                    if (meal is null)
                    {
                        continue;
                    }

                    meals[meal] = Math.Min(meal.Stock, amount);
                }

                return meals;
            }
        }

        public static decimal GetPrice(Meal meal, string username)
        {
            using (var db = new EmensaContext())
            {
                var price = db.Price.First(p => p.MealId == meal.Id);
                switch (EmensaContext.GetRole(username))
                {
                    case Role.Student:
                        Debug.Assert(price.StudentPrice != null, "price.StudentPrice != null");
                        return (decimal) price.StudentPrice;
                    case Role.Employee:
                        Debug.Assert(price.EmployeePrice != null, "price.EmployeePrice != null");
                        return (decimal) price.EmployeePrice;
                    case Role.Guest:
                        return price.GuestPrice;
                    default:
                        return price.GuestPrice;
                }
            }
        }

        public static bool PlaceOrder(Order orders, string username)
        {
            if (orders.PickupTime is null)
            {
                return false;
            }

            var order = new DataModels.Order
            {
                CollectedAt = DateTime.Parse(orders.PickupTime),
                OrderedAt = DateTime.Now,
                UserId = EmensaContext.GetUser(username).Id
            };
            using (var db = new EmensaContext())
            {
                db.Order.Add(order);
                db.SaveChanges();
                var relations = orders.Meals.ToList().ConvertAll(pair => new OrderMealRelation
                {
                    Amount = pair.Value,
                    MealId = db.Meal.First(meal => meal.Id == Convert.ToInt32(pair.Key)).Id,
                    OrderId = order.Id
                });
                db.OrderMealRelation.AddRange(relations);
                db.SaveChanges();
            }

            return true;
        }
    }
}