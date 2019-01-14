using System.Collections.Generic;
using System.Linq;
using emensa.DataModels;
using emensa.Utility;

namespace emensa.ViewModels
{
    public class Ingredients
    {
        public Ingredients()
        {
            using (var db = new EmensaContext())
            {
                AllIngredients = (from ingredient in db.Ingredient select ingredient).ToList();
            }
        }
        
        public List<Ingredient> AllIngredients { get; }
    }
}