namespace emensa.Models
{
    public class Price
    {
        public uint ValidYear { get; set; }
        public Meal Meal { get; set; }
        public float GuestPrice { get; set; }
        public float EmployeePrice { get; set; }
        public float StudentPrice { get; set; }
    }
}