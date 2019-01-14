using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Category
    {
        public Category()
        {
            ChildCategories = new HashSet<Category>();
            Meal = new HashSet<Meal>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ImageId { get; set; }
        public int? ParentCategoryId { get; set; }

        public Image Image { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<Category> ChildCategories { get; set; }
        public ICollection<Meal> Meal { get; set; }
    }
}
