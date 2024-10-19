using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace e_commerce_website.Pages
{
    public class ShopModel : PageModel
    {
        string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";
        public List<Product> PagesProducts = new List<Product>();
        public Product ProductInsert = new Product();
        public string errorMessage = "";
        public string successMessage = "";

        [Key]
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_category { get; set; }
        public decimal product_price { get; set; }
        public int product_qty { get; set; }
        public string product_img { get; set; }
        public decimal product_discount { get; set; }
        public string product_description { get; set; }
        public bool product_availability { get; set; }




        public void OnGet()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string sql = "SELECT * FROM [dbo].[Products]";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.product_id = reader.GetInt32(0);
                            product.product_name = reader.GetString(1);
                            product.product_category = reader.GetString(2);
                            product.product_price = reader.GetDecimal(3);
                            product.product_qty = reader.GetInt32(4);
                            product.product_img = reader.GetString(5);
                            product.product_discount = reader.GetDecimal(6);
                            product.product_description = reader.GetString(7);
                            product.product_availability = reader.GetBoolean(8);
                            PagesProducts.Add(product);
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
        public int product_id;
        public string product_name;
        public string product_category;
        public decimal product_price;
        public int product_qty;
        public string product_img;
        public decimal product_discount;
        public string product_description;
        public bool product_availability;
    }
}
