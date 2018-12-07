using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using emensa.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Internal;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace emensa.Controllers
{
    public static class Service
    {
        private static readonly MySqlConnection Connection;
        private static readonly dynamic Config;

        static Service()
        {
            Config = JObject.Parse(File.ReadAllText("appsettings.json"));
            Connection = new MySqlConnection(Config.ConnectionStrings.Default.ToString());
            Connection.Open();
        }

        public static ProductsModel GetProductsModel()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            var query = ((JArray) Config.SqlQueries.ProductsModel.Meals).ToList().Join("\n");
            var command = new MySqlCommand(query, Connection);
            var meals = new List<Tuple<Meal, Image>>();
            var categories = new List<Category>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    meals.Add(new Tuple<Meal, Image>(
                        new Meal
                        {
                            Id = (uint) reader["meal_id"],
                            Name = reader["meal_name"] as string,
                            Available = (bool) reader["available"],
                            Category = new Category
                            {
                                Id = (uint) reader["category_id"]
                            },
                            GlutenFree = !(reader["gluten_free"] is DBNull) && (sbyte) reader["gluten_free"] > 0,
                            Organic = !(reader["organic"] is DBNull) && (sbyte) reader["organic"] > 0,
                            Vegan = !(reader["vegan"] is DBNull) && (sbyte) reader["vegan"] > 0,
                            Vegetarian = !(reader["vegetarian"] is DBNull) && (sbyte) reader["vegetarian"] > 0
                        },
                        new Image
                        {
                            FilePath = reader["file_path"] as string
                        }));
                }
            }

            query = ((JArray) Config.SqlQueries.ProductsModel.Categories).ToList().Join("\n");
            command.CommandText = query;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = (uint) reader["id"],
                        Name = reader["name"] as string,
                        Parent = reader["parent_id"] is DBNull
                            ? null
                            : new Category
                            {
                                Id = (uint) reader["parent_id"],
                                Name = reader["parent_name"] as string
                            }
                    });
                }
            }

            return new ProductsModel
            {
                Categories = categories,
                Meals = meals
            };
        }

        public static List<Ingredient> GetIngredients()
        {
            var query = ((JArray) Config.SqlQueries.IngredientsModel).ToList().Join("\n");
            var command = new MySqlCommand(query, Connection);
            var ingredients = new List<Ingredient>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ingredients.Add(new Ingredient
                    {
                        Id = (uint) reader["id"],
                        Name = reader["name"] as string,
                        Organic = (bool) reader["organic"],
                        Vegetarian = (bool) reader["vegetarian"],
                        Vegan = (bool) reader["vegan"],
                        GlutenFree = (bool) reader["gluten_free"]
                    });
                }
            }

            return ingredients;
        }

        public static DetailsModel GetDetailsModel(uint id)
        {
            var query = ((JArray) Config.SqlQueries.DetailsModel.Meal).ToList().Join("\n");
            var command = new MySqlCommand(query);
            command.Parameters.AddWithValue("id", id);
            var detailsModel = new DetailsModel();
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                detailsModel.Meal = new Meal
                {
                    Name = reader["name"] as string,
                    Description = reader["description"] as string
                };
            }

            return detailsModel;
        }

        public static LoginModel GetUserLogin(string username)
        {
            var query = ((JArray) Config.SqlQueries.LoginModel).ToList().Join("\n");
            var command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("username", username);
            var loginModel = new LoginModel();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    loginModel.Username = reader["username"] as string;
                    loginModel.Hash = reader["hash"] as string;
                    loginModel.Salt = reader["salt"] as string;
                }
                else
                {
                    loginModel.UserError = true;
                }
            }

            return loginModel;
        }

        public static bool AddUser(string username, string firstName, string lastName, string email, string password,
            DateTime birthday)
        {
            byte[] saltBytes = new byte[24];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 16
            ));
            var salt = Convert.ToBase64String(saltBytes);
            var query = ((JArray) Config.SqlQueries.LoginModel.AddUser).ToList().Join("\n");
            var command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("firstName", firstName);
            command.Parameters.AddWithValue("lastName", lastName);
            command.Parameters.AddWithValue("email", email);
            command.Parameters.AddWithValue("hash", hash);
            command.Parameters.AddWithValue("salt", salt);
            command.Parameters.AddWithValue("birthday", birthday.ToString("yyyy-MM-dd"));
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool VerifyPassword(string password, string salt, string hash)
        {
            string hashCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 16
            ));

            return hash == hashCheck;
        }
    }
}