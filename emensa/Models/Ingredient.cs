using System.Collections.Generic;
using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class Ingredient
    {
        public uint Id;
        public string Name;
        public bool Organic;
        public bool Vegetarian;
        public bool Vegan;
        public bool GlutenFree;


        public static List<Ingredient> GetAll()
        {
            var query = "select * from ingredient";
            var command = new MySqlCommand(query, Service.Connection);
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
    }
    
}