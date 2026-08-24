using day08.Core.Entities;
using day08.Core.Interfaces;
using day08.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace day08.Infractructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext context;
        public InventoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task AddTransactionAsync(InventoryTransaction transaction) 
            => await context.transactions.AddAsync(transaction);


        public async Task<IEnumerable<Product>> GetAllProductsAsync()
            => await context.Products.AsNoTracking().ToListAsync();

        //public async Task<IEnumerable<Product>> GetAllProductsAsync()
        //{ 
        //   return  await context.Products.AsNoTracking().ToListAsync();
        //}
           
        public async Task<Product?> GetProductByIdAsync(int id)
            =>  await context.Products.FindAsync(id);
    }
}
