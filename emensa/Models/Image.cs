using System.Collections.Generic;

namespace emensa.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string AlternativeText { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public List<Meal> Meals { get; set; }
    }
}