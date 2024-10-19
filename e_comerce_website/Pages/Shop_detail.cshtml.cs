using e_comerce_website.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace e_comerce_website.Pages
{
    public class Shop_detailModel : PageModel
    {
        string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";
        public List<ProductDetail> ProductDetails = new List<ProductDetail>();
        public ProductDetail ProductInsert = new ProductDetail();
        public string errorMessage = "";
        public string successMessage = "";

        // Accept the ID parameter passed via the URL
        public void OnGet(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                // Join the Product table with the ProductDetail table to get product_name
                string sql = @"
                            SELECT pd.product_detail_id, pd.ProductID, pd.detail_type, pd.detail_value,
                            p.product_name, p.product_price, p.product_description, p.product_availability
                            FROM [dbo].[product_detail] pd
                            INNER JOIN [dbo].[products] p ON pd.ProductID = p.product_id
                            WHERE pd.ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductDetail product_detail = new ProductDetail();
                            product_detail.product_detail_id = reader.GetInt32(0);
                            product_detail.ProductID = reader.GetInt32(1);
                            product_detail.detail_type = reader.GetString(2);
                            product_detail.detail_value = reader.GetString(3);
                            product_detail.product_name = reader.GetString(4);
                            product_detail.product_price = reader.GetDecimal(5);
                            product_detail.product_description = reader.GetString(6);
                            product_detail.product_availability = reader.GetBoolean(7);
                            ProductDetails.Add(product_detail);
                        }
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;
            }
        }

    }

    public class ProductDetail
    {
        public int product_detail_id;
        public int ProductID;
        public string detail_type;
        public string detail_value;
        public string product_name;
        public decimal product_price;
        public string product_description;
        public bool product_availability;
    }
}
