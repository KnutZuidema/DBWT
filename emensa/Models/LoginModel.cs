using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class LoginModel
    {
        public string Username;
        public string Hash;
        public string Salt;
        public bool UserError;
        public bool PasswordError;
        public bool ActiveError;

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
                    if (!(bool) reader["active"])
                    {
                        ActiveError = true;
                    }
                    else
                    {
                        Username = reader["username"] as string;
                        Hash = reader["hash"] as string;
                        Salt = reader["salt"] as string;
                    }
                }
                else
                {
                    UserError = true;
                }
            }
        }
    }
}