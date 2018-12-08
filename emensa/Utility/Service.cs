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
        public static readonly dynamic Queries;


        static Service()
        {
            dynamic config = JObject.Parse(File.ReadAllText("appsettings.json"));
            Connection = new MySqlConnection(config.ConnectionStrings.Default.ToString());
            Queries = config.SqlQueries;
            Connection.Open();
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