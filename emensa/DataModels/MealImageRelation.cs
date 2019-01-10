using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class MealImageRelation
    {
        public int MealId { get; set; }
        public int ImageId { get; set; }

        public Image Image { get; set; }
        public Meal Meal { get; set; }
    }
}
