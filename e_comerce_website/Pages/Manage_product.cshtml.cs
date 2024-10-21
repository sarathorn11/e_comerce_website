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

        public void LoadProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT product_id, product_name, product_category, product_price, product_qty, product_discount, product_description FROM [dbo].[Products]";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new StoreProduct
                                {
                                    product_id = reader.GetInt32(0),
                                    product_name = reader.GetString(1),
                                    product_category = reader.GetString(2),
                                    product_price = reader.GetDecimal(3),
                                    product_qty = reader.GetInt32(4),
                                    product_discount = reader.GetDecimal(5),
                                    product_description = reader.GetString(6)
                                };
                                Products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                errorMessage = "Error loading products: " + ex.Message;
            }
        }

        public IActionResult OnPostSave()
        {
            if (CurrentProduct == null)
            {
                errorMessage = "Product details are required.";
                return Page();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        if (CurrentProduct.product_id > 0) // Update existing product
                        {
                            string sql = "UPDATE [dbo].[Products] SET product_name = @product_name, product_category = @product_category, product_price = @product_price, product_qty = @product_qty, product_discount = @product_discount, product_description = @product_description WHERE product_id = @product_id";
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                cmd.Parameters.AddWithValue("@product_id", CurrentProduct.product_id);
                                cmd.Parameters.AddWithValue("@product_name", CurrentProduct.product_name);
                                cmd.Parameters.AddWithValue("@product_category", CurrentProduct.product_category);
                                cmd.Parameters.AddWithValue("@product_price", CurrentProduct.product_price);
                                cmd.Parameters.AddWithValue("@product_qty", CurrentProduct.product_qty);
                                cmd.Parameters.AddWithValue("@product_discount", CurrentProduct.product_discount);
                                cmd.Parameters.AddWithValue("@product_description", CurrentProduct.product_description);
                                cmd.ExecuteNonQuery();
                            }
                            successMessage = "Product updated successfully!";
                        }
                        else // Create new product
                        {
                            string sql = "INSERT INTO [dbo].[Products] (product_name, product_category, product_price, product_qty, product_discount, product_description) VALUES (@product_name, @product_category, @product_price, @product_qty, @product_discount, @product_description)";
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                cmd.Parameters.AddWithValue("@product_name", CurrentProduct.product_name);
                                cmd.Parameters.AddWithValue("@product_category", CurrentProduct.product_category);
                                cmd.Parameters.AddWithValue("@product_price", CurrentProduct.product_price);
                                cmd.Parameters.AddWithValue("@product_qty", CurrentProduct.product_qty);
                                cmd.Parameters.AddWithValue("@product_discount", CurrentProduct.product_discount);
                                cmd.Parameters.AddWithValue("@product_description", CurrentProduct.product_description);
                                cmd.ExecuteNonQuery();
                            }
                            successMessage = "Product created successfully!";
                        }

                        LoadProducts(); // Refresh the product list
                    }
                }
                catch (SqlException ex)
                {
                    errorMessage = "Error saving product: " + ex.Message;
                }
            }
            return Page();
        }


        public IActionResult OnPostDelete(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM [dbo].[Products] WHERE product_id = @product_id";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@product_id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                successMessage = "Product deleted successfully!";
            }
            catch (SqlException ex)
            {
                errorMessage = "Error deleting product: " + ex.Message;
            }
            LoadProducts(); // Refresh the product list
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
        public decimal product_discount { get; set; }
        public string product_description { get; set; }
    }
}
