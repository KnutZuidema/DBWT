using System;
using System.Data;

namespace emensa.Models
{
    public class User
    {
        public uint Id;
        public string Username;
        public string Email;
        public string salt;
        public string hash;
        public string FirstName;
        public string LastName;
        public DateTime Created;
        public bool Active;
        public DateTime Birthday;
        public DateTime LastLogin;
        public uint Age;
    }
}