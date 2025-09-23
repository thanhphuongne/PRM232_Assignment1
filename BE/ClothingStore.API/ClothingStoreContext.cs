using Microsoft.EntityFrameworkCore;

namespace ClothingStore.API;

public class ClothingStoreContext : DbContext
{
    public ClothingStoreContext(DbContextOptions<ClothingStoreContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}