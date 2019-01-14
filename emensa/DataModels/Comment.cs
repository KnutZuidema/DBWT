namespace emensa.DataModels
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int MealId { get; set; }

        public Meal Meal { get; set; }
        public Student User { get; set; }
    }
}
