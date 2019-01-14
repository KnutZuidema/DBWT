using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Order
    {
        public Order()
        {
            OrderMealRelation = new HashSet<OrderMealRelation>();
        }

        public int Id { get; set; }
        public DateTime OrderedAt { get; set; }
        public DateTime? CollectedAt { get; set; }
        public int? UserId { get; set; }

        public User User { get; set; }
        public ICollection<OrderMealRelation> OrderMealRelation { get; set; }
    }
}
