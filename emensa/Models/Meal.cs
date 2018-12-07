namespace emensa.Models
{
    public class Meal
    {
        public uint Id;
        public string Description;
        public string Name;
        public uint Stock;
        public Category Category;
        public bool Available;
        public bool Vegetarian;
        public bool Vegan;
        public bool Organic;
        public bool GlutenFree;
    }
}