using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); // Adds Razor Pages services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Helps mitigate XSS attacks
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

// Register IHttpContextAccessor to access HttpContext
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Shows detailed errors in development
}
else
{
    app.UseExceptionHandler("/Error"); // Custom error handling
    app.UseHsts(); // Enables HTTP Strict Transport Security
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS
app.UseStaticFiles(); // Enables serving static files

app.UseRouting(); // Enables routing

app.UseSession(); // Enables session management
app.UseAuthorization(); // Enables authorization middleware

app.MapRazorPages(); // Maps Razor pages to the app

app.Run(); // Runs the application
