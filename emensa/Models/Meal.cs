using System;
using System.Collections.Generic;
using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class Meal
    {
        public uint Id;
        public string Description;
        public string Name;
        public uint Stock;
        public Category Category;
        public bool Available;
        public bool Vegetarian;
        public bool Vegan;
        public bool Organic;
        public bool GlutenFree;

        public static List<Tuple<Meal, Image>> GetAllWithImage()
        {
            var query = "select * from products";
            var command = new MySqlCommand(query, Service.Connection);
            var meals = new List<Tuple<Meal, Image>>();
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
                            GlutenFree = !(reader["gluten_free"] is DBNull) && (bool) reader["gluten_free"],
                            Organic = !(reader["organic"] is DBNull) && (bool) reader["organic"],
                            Vegan = !(reader["vegan"] is DBNull) && (bool) reader["vegan"],
                            Vegetarian = !(reader["vegetarian"] is DBNull) && (bool) reader["vegetarian"]
                        },
                        new Image
                        {
                            FilePath = reader["file_path"] as string
                        }));
                }
            }

            return meals;
        }
    }
}