using System;

namespace emensa.Models
{
    public class Order
    {
        public uint Id;
        public DateTime OrderedAt;
        public DateTime CollectedAt;
        public User User;
    }
}