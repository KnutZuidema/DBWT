using System.Collections.Generic;

namespace emensa.Models
{
    public class Image
    {
        public int Id;
        public string AlternativeText;
        public string FilePath;
        public string Title;
        public List<Meal> Meals;
    }
}