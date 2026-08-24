using day04.Models;
using day04.ViewModels;

namespace day04.Services
{
    public interface IProductService
    {
        Task<ProductIndexViewModel> GetIndexDataAsync(string? keyword);
        Task<Product?> GetByIdAsync(int id);
        Task CreateAsync(Product product, IFormFile? image);
        Task UpdateAsync(Product product, IFormFile? image);
        Task DeleteAsync(int id);
    }
}
