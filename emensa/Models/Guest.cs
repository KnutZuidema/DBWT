using System;

namespace emensa.Models
{
    public class Guest : User
    {
        public string Reason;
        public DateTime ValidUntil;

        public Guest()
        {
        }

        public Guest(User user)
        {
            Username = user.Username;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Birthday = user.Birthday;
        }
    }
}