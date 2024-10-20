using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace e_comerce_website.Pages
{
    public class Manage_productModel : PageModel
    {
        private readonly string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";

        [BindProperty]
        public StoreProduct CurrentProduct { get; set; } = new StoreProduct();

        public List<StoreProduct> Products { get; private set; } = new List<StoreProduct>();

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
                                StoreProduct product = new StoreProduct
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
                                Products.Add(product);
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

        public IActionResult OnGetEdit(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [dbo].[Products] WHERE product_id = @Id";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CurrentProduct.product_id = reader.GetInt32(0);
                                CurrentProduct.product_name = reader.GetString(1);
                                CurrentProduct.product_category = reader.GetString(2);
                                CurrentProduct.product_price = reader.GetDecimal(3);
                                CurrentProduct.product_qty = reader.GetInt32(4);
                                CurrentProduct.product_img = reader.GetString(5);
                                CurrentProduct.product_discount = reader.GetDecimal(6);
                                CurrentProduct.product_description = reader.GetString(7);
                                CurrentProduct.product_availability = reader.GetBoolean(8);
                                CurrentProduct.product_favorite = reader.GetBoolean(9);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }

            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE [dbo].[Products] SET product_name = @Name, product_category = @Category, product_price = @Price, product_qty = @Qty, product_img = @Img, product_discount = @Discount, product_description = @Description, product_availability = @Availability, product_favorite = @Favorite WHERE product_id = @Id";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", CurrentProduct.product_id);
                        cmd.Parameters.AddWithValue("@Name", CurrentProduct.product_name);
                        cmd.Parameters.AddWithValue("@Category", CurrentProduct.product_category);
                        cmd.Parameters.AddWithValue("@Price", CurrentProduct.product_price);
                        cmd.Parameters.AddWithValue("@Qty", CurrentProduct.product_qty);
                        cmd.Parameters.AddWithValue("@Img", CurrentProduct.product_img);
                        cmd.Parameters.AddWithValue("@Discount", CurrentProduct.product_discount);
                        cmd.Parameters.AddWithValue("@Description", CurrentProduct.product_description);
                        cmd.Parameters.AddWithValue("@Availability", CurrentProduct.product_availability);
                        cmd.Parameters.AddWithValue("@Favorite", CurrentProduct.product_favorite);
                        cmd.ExecuteNonQuery();
                    }
                }
                successMessage = "Product updated successfully!";
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }

            LoadProducts(); // Refresh the product list
            return Page();
        }


        public IActionResult OnPostDelete(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteSql = "DELETE FROM Products WHERE product_id = @ProductID"; // Actual deletion
                    using (SqlCommand deleteCmd = new SqlCommand(deleteSql, connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@ProductID", id);
                        deleteCmd.ExecuteNonQuery();
                    }

                    successMessage = "Product deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }

            LoadProducts();
            return Page();
        }
    }

    public class StoreProduct
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
