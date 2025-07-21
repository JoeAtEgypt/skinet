using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// AddScpoed means that the service is going to live for as long as the HTTP request is being processed
// AddTransient means that a new instance of the service is created every time it is requested
// AddSingleton means that a single instance of the service is created and shared throughout the application
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Buiding the application
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

// Seed the database with initial data
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    // Log the exception (not implemented here for brevity)
    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
}

app.Run();
