using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            IngredientMealRelation = new HashSet<IngredientMealRelation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Organic { get; set; }
        public byte Vegetarian { get; set; }
        public byte Vegan { get; set; }
        public byte GlutenFree { get; set; }

        public ICollection<IngredientMealRelation> IngredientMealRelation { get; set; }
    }
}
