using System;
using System.Collections.Generic;
using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class DetailsModel
    {
        public Meal Meal;
        public List<Ingredient> Ingredients;
        public Image Image;
        public decimal Price;
        public User User;

        public DetailsModel(uint id, User user)
        {
            User = user;
            Ingredients = new List<Ingredient>();
            var query = "call get_meal(@id)";
            var command = new MySqlCommand(query, Service.Connection);
            command.Parameters.AddWithValue("id", id);
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Meal = new Meal
                {
                    Name = reader["name"] as string,
                    Description = reader["description"] as string
                };
                Image = new Image
                {
                    FilePath = reader["file_path"] as string
                };
                object price;
                if (User is Employee)
                {
                    price = reader["employee_price"];
                }
                else if (User is Student)
                {
                    price = reader["student_price"];
                }
                else
                {
                    price = reader["guest_price"];
                }

                Price = price is DBNull ? (decimal) 3.5 : (decimal) price;
                do
                {
                    Ingredients.Add(new Ingredient
                    {
                        Name = reader["ingredient_name"] as string,
                        GlutenFree = (bool) reader["gluten_free"],
                        Organic = (bool) reader["organic"],
                        Vegan = (bool) reader["vegan"],
                        Vegetarian = (bool) reader["vegetarian"]
                    });
                } while (reader.Read());
            }
        }
    }
}