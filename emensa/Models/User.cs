using System;
using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class User
    {
        public uint Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string salt { get; set; }
        public string hash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime LastLogin { get; set; }
        public uint Age { get; set; }


        public static string GetRole(string username)
        {
            var query = "select get_role(@username)";
            var command = new MySqlCommand(query, Service.Connection);
            command.Parameters.AddWithValue("username", username);
            return command.ExecuteScalar() as string;
        }
        
        public static bool RegisterUser(User user, string password)
        {
            var transaction = Service.Connection.BeginTransaction();
            try
            {
                var (hash, salt) = Service.CreateHash(password);
                var query = "insert into user (username, email, salt, hash, first_name, last_name, birthday)" + 
                            "values (@username, @email, @salt, @hash, @firstName, @lastName, @birthday);";
                var command = new MySqlCommand(query, Service.Connection, transaction);
                command.Parameters.AddWithValue("username", user.Username);
                command.Parameters.AddWithValue("firstName", user.FirstName);
                command.Parameters.AddWithValue("lastName", user.LastName);
                command.Parameters.AddWithValue("email", user.Email);
                command.Parameters.AddWithValue("hash", hash);
                command.Parameters.AddWithValue("salt", salt);
                command.Parameters.AddWithValue("birthday", user.Birthday.ToString("yyyy-MM-dd"));
                command.ExecuteNonQuery();

                command.Parameters.Clear();

                var id = command.LastInsertedId;
                command.Parameters.AddWithValue("id", id);
                
                if (user is Guest guest)
                {
                    command.CommandText = "call add_guest(@id, @reason, @validUntil)";
                    command.Parameters.AddWithValue("reason", guest.Reason);
                    command.Parameters.AddWithValue("validUntil", guest.ValidUntil.ToString("yyyy-MM-dd"));
                }
                else if (user is Student student)
                {
                    command.CommandText = "call add_student(@id, @matriculationNumber, @major)";
                    command.Parameters.AddWithValue("matriculationNumber", student.MatriculationNumber);
                    command.Parameters.AddWithValue("major", student.Major.ToString());
                }
                else if (user is Employee employee)
                {
                    command.CommandText = "call add_employee(@id, @office, @phoneNumber)";
                    command.Parameters.AddWithValue("office", employee.Office);
                    command.Parameters.AddWithValue("phoneNumber", employee.PhoneNumber);
                }
                else if (user is Member)
                {
                    command.CommandText = "call add_member(@id)";
                }

                command.ExecuteNonQuery();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}