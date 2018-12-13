using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public bool UserError { get; set; }
        public bool PasswordError { get; set; }

        public LoginModel()
        {
        }

        public LoginModel(string username)
        {
            var query = "call get_user_hash(@username)";
            var command = new MySqlCommand(query, Service.Connection);
            command.Parameters.AddWithValue("username", username);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Username = reader["username"] as string;
                    Hash = reader["hash"] as string;
                    Salt = reader["salt"] as string;
                }
                else
                {
                    UserError = true;
                }
            }
        }
    }
}