using OrderManagerCLI.ContextModels;
using Microsoft.EntityFrameworkCore;

namespace OrderManagerCLI.Contexts
{
    public class ECommerce2Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"User Id=usr;Password=pass;Server=host.docker.internal;Port=5432;Database=ECommerce2;");
        }


        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = "b4b5a9c9-70c5-4e91-b0c6-e38c94475659", Price = 10m, ProductName = "Ürün 1", Quantity = 100 });
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = "f5ed9460-f063-4b0d-92fb-d0e605c65457", Price = 10m, ProductName = "Ürün 2", Quantity = 100 });
            base.OnModelCreating(modelBuilder);
        }
    }
}
