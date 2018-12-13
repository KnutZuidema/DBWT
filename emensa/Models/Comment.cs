namespace emensa.Models
{
    public class Comment
    {
        public uint Id { get; set; }
        public string Note { get; set; }
        public uint Rating { get; set; }
        public User User { get; set; }
        public Meal Meal { get; set; }
    }
}