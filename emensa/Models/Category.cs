using System;
using System.Collections.Generic;
using emensa.Controllers;
using MySql.Data.MySqlClient;

namespace emensa.Models
{
    public class Category
    {
        public uint Id;
        public string Name;
        public Image Image;
        public Category Parent;

        public static List<Category> GetAll()
        {
            var categories = new List<Category>();

            var query = "select * from categories_with_parent";
            var command = new MySqlCommand(query, Service.Connection);
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

            return categories;
        }
    }
}