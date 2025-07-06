using etrade_core.application.IRepositories;
using etrade_core.infrastructure.Identity;
using etrade_core.persistence.Context;
using etrade_core.persistence.Identity;
using etrade_core.persistence.Repositories;
using etrade_core.persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "E-Trade Core API",
        Version = "v1",
        Description = "E-Trade Core API Documentation",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "E-Trade Team",
            Email = "support@etrade.com"
        }
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");

// Domain DbContext
builder.Services.AddDbContext<DomainDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(identityConnectionString));

// Identity Services
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Persistence Services
builder.Services.AddPersistenceServices(connectionString!);

// Infrastructure Services
builder.Services.AddScoped<IIdentityUserService, IdentityUserService>();

// Application Services
// builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Trade Core API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "E-Trade Core API Documentation";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();