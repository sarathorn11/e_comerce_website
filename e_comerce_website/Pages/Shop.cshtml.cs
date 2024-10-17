using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic; // Ensure this is included for List<Product>

namespace e_commerce_website.Pages
{
    public class ShopModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=e_commerce_db;User ID=sa;Password=123;TrustServerCertificate=True;";
        public List<Product> PagesProducts { get; set; } = new List<Product>();
        public string ErrorMessage { get; set; } = ""; // Changed to a property with a capital letter
        public string SuccessMessage { get; set; } = ""; // Added property for success message

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT ProductID, ProductName, CategoryID, Price, StockQuantity, DiscountID FROM [dbo].[Product]";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    CategoryID = reader.GetInt32(2),
                                    Price = reader.GetDecimal(3),
                                    StockQuantity = reader.GetInt32(4),
                                    DiscountID = reader.GetInt32(5)
                                };
                                PagesProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message; // Capture error messages
            }

            // Debug output
            System.Diagnostics.Debug.WriteLine($"Products loaded: {PagesProducts.Count}");
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int DiscountID { get; set; }
    }
}
