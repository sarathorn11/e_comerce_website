using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace e_comerce_website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";

        public List<Products> PagesProducts { get; private set; } = new List<Products>();
        public string errorMessage { get; private set; } = "";
        public string successMessage { get; private set; } = "";

        public void OnGet()
        {
            LoadProducts();
        }

        // Method to update product_favorite status
        public JsonResult OnPostEditFavorite([FromBody] FavoriteRequest request)
        {
            if (request == null || request.ProductId <= 0)
            {
                return new JsonResult(new { success = false, message = "Invalid product ID" });
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get the current favorite status
                    string getFavStatusQuery = "SELECT product_favorite FROM [dbo].[Products] WHERE product_id = @ProductId";
                    using (SqlCommand getFavCmd = new SqlCommand(getFavStatusQuery, connection))
                    {
                        getFavCmd.Parameters.AddWithValue("@ProductId", request.ProductId);
                        bool currentFavorite = (bool)getFavCmd.ExecuteScalar(); // Get the current favorite status

                        // Toggle the favorite status
                        string updateQuery = "UPDATE [dbo].[Products] SET product_favorite = @NewFavorite WHERE product_id = @ProductId";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@ProductId", request.ProductId);
                            updateCmd.Parameters.AddWithValue("@NewFavorite", !currentFavorite); // Toggle favorite status
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }

                return new JsonResult(new { success = true, message = "Favorite status updated" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error: " + ex.Message });
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [dbo].[Products]";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products product = new Products
                                {
                                    product_id = reader.GetInt32(0),
                                    product_name = reader.GetString(1),
                                    product_category = reader.GetString(2),
                                    product_price = reader.GetDecimal(3),
                                    product_qty = reader.GetInt32(4),
                                    product_img = reader.GetString(5),
                                    product_discount = reader.GetDecimal(6),
                                    product_description = reader.GetString(7),
                                    product_availability = reader.GetBoolean(8),
                                    product_favorite = reader.GetBoolean(9)
                                };
                                PagesProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }
        }
    }

    public class FavoriteRequest
    {
        public int ProductId { get; set; } // Represents the product ID
    }

    public class Products
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_category { get; set; }
        public decimal product_price { get; set; }
        public int product_qty { get; set; }
        public string product_img { get; set; }
        public decimal product_discount { get; set; }
        public string product_description { get; set; }
        public bool product_availability { get; set; }
        public bool product_favorite { get; set; }
    }
}
