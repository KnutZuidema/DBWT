using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Image
    {
        public Image()
        {
            Category = new HashSet<Category>();
            MealImageRelation = new HashSet<MealImageRelation>();
        }

        public int Id { get; set; }
        public string AlternativeText { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }

        public ICollection<Category> Category { get; set; }
        public ICollection<MealImageRelation> MealImageRelation { get; set; }
    }
}
