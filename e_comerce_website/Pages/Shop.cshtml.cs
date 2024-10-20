using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace e_commerce_website.Pages
{
    public class ShopModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";

        public List<Product> PagesProducts { get; private set; } = new List<Product>();
        public string errorMessage { get; private set; } = "";
        public string successMessage { get; private set; } = "";

        public void OnGet()
        {
            LoadProducts();
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
                                Product product = new Product
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

    public class Product
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
