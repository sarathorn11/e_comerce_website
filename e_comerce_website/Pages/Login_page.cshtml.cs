using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace e_comerce_website.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        private readonly string connectionString = "Server=DESKTOP-MHAUOVO\\SQLEXPRESS1;Database=shops_db;User ID=sa;Password=123;TrustServerCertificate=True;";

        public void OnGet()
        {
            // Any logic to handle before showing the page
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the user role and email if credentials are correct
            var userData = GetUserRoleAndEmail(Input.Username, Input.Password);

            // Check if the tuple contains valid data
            if (!string.IsNullOrEmpty(userData.Role) && !string.IsNullOrEmpty(userData.Email))
            {
                // Store the role and email in session
                HttpContext.Session.SetString("UserRole", userData.Role);
                HttpContext.Session.SetString("UserEmail", userData.Email);

                // Redirect to some dashboard or homepage on successful login
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }

        // Method to get the user role and email based on username and password
        private (string Role, string Email) GetUserRoleAndEmail(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Hash the entered password to compare with stored hash
                string hashedPassword = HashPassword(password);

                // Query to get the user's role and email
                string sql = "SELECT role, email FROM users WHERE username = @Username AND password_hash = @PasswordHash";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string role = reader["role"].ToString();
                            string email = reader["email"].ToString();
                            return (role, email); // Return both role and email
                        }
                    }
                }
            }

            return (null, null); // Return nulls if no user found
        }

        // Method to hash the password
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert to hex
                }
                return builder.ToString();
            }
        }

        // Input model for user credentials
        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        // Method to handle user logout
        public IActionResult OnGetLogout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Set UserEmail session to empty
            HttpContext.Session.SetString("UserEmail", string.Empty);

            return Page();
        }
    }
}
