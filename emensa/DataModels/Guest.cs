using System;

namespace emensa.DataModels
{
    public partial class Guest
    {
        public int UserId { get; set; }
        public string Reason { get; set; }
        public DateTime ValidUntil { get; set; }

        public User User { get; set; }
    }
}
