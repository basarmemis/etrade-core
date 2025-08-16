using etrade_core.infrastructure.Identity;
using etrade_core.persistence.Context;
using etrade_core.persistence.Identity;
using etrade_core.persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MediatR;
using etrade_core.application.Common.Behaviors;

using FluentValidation;
using etrade_core.application.OrderModule.Commands.CreateOrder;
using System.Reflection;
using etrade_core.infrastructure.CustomMessageQueue.DIExtensions;
using etrade_core.infrastructure.CustomMessageQueue.Options;
using etrade_core.infrastructure.CustomMessageQueue.IServices;
using etrade_core.infrastructure.CustomMessageQueue.Services;

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

// MediatR Configuration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

// Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommandValidator).Assembly);

// Distributed Cache for Idempotency
builder.Services.AddDistributedMemoryCache();

// Application Services
// builder.Services.AddScoped<IUserService, UserService>();



builder.Services.AddRabbitMqMessaging(
    scanAssemblies: new[] { Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(MessagingOptions))! },
    configure: opt =>
    {
        opt.Host = "localhost";
        opt.Username = "guest";
        opt.Password = "guest";
        opt.UseDelayedMessageScheduler = true;
        opt.UseQuorumQueues = true;          // opsiyonel
        opt.EnablePriority = false;          // opsiyonel
        opt.PrefetchCount = 64;
        opt.RetryAttempts = 5;
        opt.RedeliveryIntervals = new[]
        {
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(45),
            TimeSpan.FromMinutes(2)
        };
        opt.EnableIdempotency = true;
        opt.IdempotencyWindow = TimeSpan.FromMinutes(10);
    });

builder.Services.AddScoped<IMessageSenderService, MessageSenderService>();

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