using System;
using emensa.Controllers;
using MySql.Data.MySqlClient;

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


        public static string GetRole(string username)
        {
            var query = "select get_role(@username)";
            var command = new MySqlCommand(query, Service.Connection);
            command.Parameters.AddWithValue("username", username);
            return command.ExecuteScalar() as string;
        }
        
//        public static bool RegisterUser()
    }
}