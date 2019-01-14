using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Meal
    {
        public Meal()
        {
            Comment = new HashSet<Comment>();
            DeclarationMealRelation = new HashSet<DeclarationMealRelation>();
            IngredientMealRelation = new HashSet<IngredientMealRelation>();
            MealImageRelation = new HashSet<MealImageRelation>();
            OrderMealRelation = new HashSet<OrderMealRelation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public byte Available { get; set; }

        public Category Category { get; set; }
        public Price Price { get; set; }
        public ICollection<Comment> Comment { get; set; }
        public ICollection<DeclarationMealRelation> DeclarationMealRelation { get; set; }
        public ICollection<IngredientMealRelation> IngredientMealRelation { get; set; }
        public ICollection<MealImageRelation> MealImageRelation { get; set; }
        public ICollection<OrderMealRelation> OrderMealRelation { get; set; }
    }
}
