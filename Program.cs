using I_Attend.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(logging => logging.AddConsole());
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Views/AdminLogin";
        options.LoginPath = "/Views/Login";
        options.LogoutPath = "/Views/Logout";
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        //options.SlidingExpiration = true;
    });

//builder.Services.AddAuthorization(options =>
//{
//    // Policy for authenticated users only
//    options.AddPolicy("AuthenticatedOnly", policy =>
//        policy.RequireAuthenticatedUser());

//});

builder.Services.AddScoped<I_AttendDAO>(provider =>
    new I_AttendDAO(builder.Configuration.GetConnectionString("DefaultConnection"),
        provider.GetService<ILogger<I_AttendDAO>>()));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventSourceLogger();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();