namespace emensa.DataModels
{
    public partial class Price
    {
        public short ValidYear { get; set; }
        public int MealId { get; set; }
        public decimal GuestPrice { get; set; }
        public decimal? EmployeePrice { get; set; }
        public decimal? StudentPrice { get; set; }

        public Meal Meal { get; set; }
    }
}
