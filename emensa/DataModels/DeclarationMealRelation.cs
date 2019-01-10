using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class DeclarationMealRelation
    {
        public string DeclarationSymbol { get; set; }
        public int MealId { get; set; }

        public Declaration DeclarationSymbolNavigation { get; set; }
        public Meal Meal { get; set; }
    }
}
