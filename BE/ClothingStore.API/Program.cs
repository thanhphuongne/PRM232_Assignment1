
using Microsoft.EntityFrameworkCore;

namespace ClothingStore.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ClothingStoreContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ClothingStoreContext>();
            db.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
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

        app.MapPost("/api/products", async (Product product, ClothingStoreContext db) =>
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/api/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .WithOpenApi();

        app.MapPut("/api/products/{id}", async (int id, Product inputProduct, ClothingStoreContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            product.Name = inputProduct.Name;
            product.Description = inputProduct.Description;
            product.Price = inputProduct.Price;
            product.ImageUrl = inputProduct.ImageUrl;

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
