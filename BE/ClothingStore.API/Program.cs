
using System;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using ClothingStore.API.Models;

namespace ClothingStore.API;

public class Program
{
    public static void Main(string[] args)
    {
        if (File.Exists(".env"))
        {
            DotNetEnv.Env.Load();
        }
        var builder = WebApplication.CreateBuilder(args);

        // Configure the port for deployment platforms like Render
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        // Add services to the container.
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ClothingStoreContext>(options =>
            options.UseNpgsql(connectionString));
        builder.Services.AddAuthorization();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Apply migrations on startup
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ClothingStoreContext>();
                db.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration failed: {ex.Message}");
            // Continue without crashing
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");
        app.UseHttpsRedirection();
        app.UseAuthorization();

        // Product CRUD endpoints
        app.MapGet("/api/products", async (ClothingStoreContext db) =>
            await db.Products.ToListAsync())
            .WithName("GetProducts")
            .WithOpenApi();

        app.MapGet("/api/products/{id}", async (int id, ClothingStoreContext db) =>
            await db.Products.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound())
            .WithName("GetProduct")
            .WithOpenApi();

        app.MapPost("/api/products", async (ProductCreateDto dto, ClothingStoreContext db) =>
        {
            var product = new Product { Name = dto.Name, Description = dto.Description, Price = dto.Price, ImageUrl = dto.ImageUrl };
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/api/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .WithOpenApi();

        app.MapPut("/api/products/{id}", async (int id, ProductCreateDto dto, ClothingStoreContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        app.MapDelete("/api/products/{id}", async (int id, ClothingStoreContext db) =>
        {
            if (await db.Products.FindAsync(id) is Product product)
            {
                db.Products.Remove(product);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName("DeleteProduct")
        .WithOpenApi();

        app.Run();
    }
}
