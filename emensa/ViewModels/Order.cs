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
                return Meals.ToDictionary(meal1 => db.Meal.First(m => m.Id == Convert.ToInt32(meal1.Key)),
                    meal2 => meal2.Value);
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