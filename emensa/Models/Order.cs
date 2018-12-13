using System;

namespace emensa.Models
{
    public class Order
    {
        public uint Id { get; set; }
        public DateTime OrderedAt { get; set; }
        public DateTime CollectedAt { get; set; }
        public User User { get; set; }
    }
}