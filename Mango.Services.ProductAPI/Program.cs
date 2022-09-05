using AutoMapper;
using Mango.Services.ProductAPI;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conStrBuilder.ConnectionString));

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityServerUrl"];
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    // This is how you add policy and scope
    options.AddPolicy("AdminScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "admin");
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mango.Services.ProductAPI", Version = "v1"});
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description=@"Enter 'Bearer' [space] and your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
