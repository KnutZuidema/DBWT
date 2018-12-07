namespace emensa.Models
{
    public class Comment
    {
        public uint Id;
        public string Note;
        public uint Rating;
        public User User;
        public Meal Meal;
    }
}