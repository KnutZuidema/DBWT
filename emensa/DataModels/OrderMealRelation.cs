using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class OrderMealRelation
    {
        public int Amount { get; set; }
        public int OrderId { get; set; }
        public int MealId { get; set; }

        public Meal Meal { get; set; }
        public Order Order { get; set; }
    }
}
