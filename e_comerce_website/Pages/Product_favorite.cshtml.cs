using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace e_comerce_website.Pages
{
    public class Product_favoriteModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";

        public List<Product> FavoriteProducts { get; private set; } = new List<Product>();

        public void OnGet()
        {
            LoadFavoriteProducts();
        }

        public IActionResult OnPostUnfavorite(int id)
        {
            // Update the product_favorite status in the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE products SET product_favorite = 0 WHERE product_id = @productId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@productId", id);
                    cmd.ExecuteNonQuery();
                }
            }

            // Reload the favorite products after the unfavorite operation
            LoadFavoriteProducts();

            // Optionally redirect to the same page to show the updated list
            return RedirectToPage();
        }

        private void LoadFavoriteProducts()
        {
            FavoriteProducts.Clear(); // Clear existing products
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT product_id, product_name, product_price, product_img FROM products WHERE product_favorite = 1";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    product_id = reader.GetInt32(0),
                                    product_name = reader.GetString(1),
                                    product_price = reader.GetDecimal(2),
                                    product_img = reader.GetString(3)
                                };

                                FavoriteProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log error, etc.
            }
        }
    }

    public class Product
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public decimal product_price { get; set; }
        public string product_img { get; set; }
    }
}
