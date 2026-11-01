using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Movie Management dependency registrations
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

// Configure Entity Framework Core with SQL Server
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register OpenAPI services
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/health/database", async (ApplicationDbContext db) =>
{
    bool canConnect = await db.Database.CanConnectAsync();

    if (canConnect)
    {
        return Results.Ok(new
        {
            message = "Successfully connected to MovieTicketBookingDB",
            databaseConnected = true
        });
    }

    return Results.Problem(
        title: "Database connection failed",
        statusCode: 503
    );
});

app.MapControllers();

app.Run();