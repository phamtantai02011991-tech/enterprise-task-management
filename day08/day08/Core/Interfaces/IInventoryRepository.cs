using day08.Core.Entities;

namespace day08.Core.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddTransactionAsync(InventoryTransaction transaction);
    }
}
