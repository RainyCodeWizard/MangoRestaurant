using Mango.Services.Identity;
using Mango.Services.Identity.DbContext;
using Mango.Services.Identity.Initializer;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// To add user secrets from secrets.json file. (Actually not needed as user secrets are included by default) can type the following command: `dotnet user-secrets init`
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true); // Pass "true" for optional as if users have not set up the user secret file on their machine yet, the following error will not occur: System.IO.FileNotFoundException: The configuration file 'secrets.json' was not found and is not optional

// Below block of code is to use local user-secrets to store account details. Refer to: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows
var conStrBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
conStrBuilder.UserID = builder.Configuration["userid"];
conStrBuilder.Password = builder.Configuration["password"];
//-----------------------------------------------------------

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conStrBuilder.ConnectionString, 
    options => options.EnableRetryOnFailure()));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // IdentityRole is default
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
}).AddInMemoryIdentityResources(Constants.IdentityResources)
.AddInMemoryApiScopes(Constants.ApiScopes)
.AddInMemoryClients(Constants.Clients)
.AddAspNetIdentity<ApplicationUser>()
.AddDeveloperSigningCredential();  //This is for dev only scenarios when you don’t have a certificate to use.

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

IServiceScope scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IDbInitializer>().Initialize();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
