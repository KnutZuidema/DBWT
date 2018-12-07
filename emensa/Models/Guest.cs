using System;

namespace emensa.Models
{
    public class Guest : User
    {
        public string Reason;
        public DateTime ValidUntil;
    }
}