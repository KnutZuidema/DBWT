using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using emensa.Models;
using emensa.Utility;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Internal;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace emensa.Controllers
{
    public static class Service
    {
        public static readonly MySqlConnection Connection;
        private static readonly dynamic Config;
        public static readonly dynamic Queries;


        static Service()
        {
            Config = JObject.Parse(File.ReadAllText("appsettings.json"));
            Connection = new MySqlConnection(Config.ConnectionStrings.Default.ToString());
            Queries = Config.SqlQueries;
            Connection.Open();
        }

        public static bool RegisterUser(User user, string password)
        {
            var transaction = Connection.BeginTransaction();
            try
            {
                var (hash, salt) = CreateHash(password);
                var query = "call add_user(@username, @email, @salt, @hash, @firstName, @lastName, @birthday)";
                var command = new MySqlCommand(query, Connection);
                command.Transaction = transaction;
                command.Parameters.AddWithValue("username", user.Username);
                command.Parameters.AddWithValue("firstName", user.FirstName);
                command.Parameters.AddWithValue("lastName", user.LastName);
                command.Parameters.AddWithValue("email", user.Email);
                command.Parameters.AddWithValue("hash", hash);
                command.Parameters.AddWithValue("salt", salt);
                command.Parameters.AddWithValue("birthday", user.Birthday.ToString("yyyy-MM-dd"));
                command.ExecuteNonQuery();
                command.Parameters.AddWithValue("id", user.Id);
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
                else if (user is Member member)
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

        public static bool VerifyPassword(string password, string salt, string hash)
        {
            return PasswordStorage.VerifyPassword(password, $"sha1:64000:18:{salt}:{hash}");
        }

        public static (string, string) CreateHash(string password)
        {
            var tuple = PasswordStorage.CreateHash(password).Split(':');
            return (tuple[tuple.Length - 1], tuple[tuple.Length - 2]);
        }
    }
}