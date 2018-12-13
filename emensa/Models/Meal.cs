using System;
using System.Collections.Generic;
using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class Meal
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public uint Stock { get; set; }
        public Category Category { get; set; }
        public bool Available { get; set; }
        public bool Vegetarian { get; set; }
        public bool Vegan { get; set; }
        public bool Organic { get; set; }
        public bool GlutenFree { get; set; }

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