using Microsoft.EntityFrameworkCore;
using ClothingStore.API.Models;

namespace ClothingStore.API;

public class ClothingStoreContext : DbContext
{
    public ClothingStoreContext(DbContextOptions<ClothingStoreContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}