using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Declaration
    {
        public Declaration()
        {
            DeclarationMealRelation = new HashSet<DeclarationMealRelation>();
        }

        public string Symbol { get; set; }
        public string Label { get; set; }

        public ICollection<DeclarationMealRelation> DeclarationMealRelation { get; set; }
    }
}
