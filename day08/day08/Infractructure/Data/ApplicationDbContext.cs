using day08.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace day08.Infractructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<InventoryTransaction> transactions => Set<InventoryTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(
                entity =>
                {
                    entity.HasKey(p => p.Id);
                    entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                    entity.Property(p => p.Price).HasPrecision(15,2);
                });
            modelBuilder.Entity<InventoryTransaction>(
               entity =>
               {
                   entity.HasKey(t => t.Id);
                   entity.HasOne(t => t.Product)
                   .WithMany(p => p.Transactions)
                   .HasForeignKey(t => t.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);         
               });

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name ="Laptop Lenovo X1",StockQuantity = 20, Price = 1000},
                new Product { Id = 2, Name = "Màn hình Dell", StockQuantity = 4, Price = 700 },
                new Product { Id = 3, Name = "Iphone 17 Pro Max", StockQuantity = 11, Price = 1200 },
                new Product { Id = 4, Name = "Bàn phím Cơ Keychtron K1", StockQuantity = 2, Price = 40 }
            );
        }
    }
}
