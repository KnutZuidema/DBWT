namespace emensa.DataModels
{
    public partial class DeclarationMealRelation
    {
        public string DeclarationId { get; set; }
        public int MealId { get; set; }

        public Declaration Declaration { get; set; }
        public Meal Meal { get; set; }
    }
}
