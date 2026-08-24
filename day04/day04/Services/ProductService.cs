using day04.Data;
using day04.Models;
using day04.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace day04.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly string[] fileExtensions = { ".gif",".png",".jpg"};
        public ProductService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        private void DeleteImage(string? imagePath)
        {
            if(string.IsNullOrEmpty(imagePath)) return;
            var fileName = Path.GetFileName(imagePath);
            var filePath = Path.Combine(environment.WebRootPath,"uploads","products",fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!fileExtensions.Contains(extension))
            {
                throw new Exception("Only file extension .gif, .png, .jpg");
            }
            if (image.Length > 2 * 1024 * 1024) {
                throw new Exception("Size only 2MB");
            }
            var uploadFolder = Path.Combine(environment.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadFolder,fileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return "/uploads/products/" + fileName;
        }

        public async Task CreateAsync(Product product, IFormFile? image)
        {
            if(image != null)
            {
                product.ImagePath = await SaveImageAsync(image);
            }
            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product =await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return;
            }
            DeleteImage(product.ImagePath);
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id ==id);//.SingleOrDefaultAsync()
        }

        //Thong ke, hien thi, tim kiem
        public async Task<ProductIndexViewModel> GetIndexDataAsync(string? keyword)
        {
            var query= context.Products.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Name!.Contains(keyword.Trim()) 
                || p.Category!.Contains(keyword.Trim()));
            }
            var products = await query.OrderBy(p => p.Name).ToListAsync();
            var statistics = new ProductStatisticsViewModel
            {
                TotalProducts =await context.Products.CountAsync(),
                TotalQuantity =await context.Products.SumAsync(p => (int?)p.Quantity) ?? 0,
                TotalInventoryValue =await context.Products.SumAsync(p => (decimal?)p.Price * p.Quantity) ?? 0,
                AveragePrice = await context.Products.AverageAsync(p => (decimal?)p.Price) ?? 0,
                TotalCategories =await context.Products.Select(p => p.Category).Distinct().CountAsync()
            };
            return new ProductIndexViewModel
            {
                Keyword = keyword,
                Products = products,
                Statistics = statistics
            };
        }

        public async Task UpdateAsync(Product editProduct, IFormFile? image)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == editProduct.Id);
            if (product == null)
            {
                throw new Exception("Product not found...");
            }
            product.Name = editProduct.Name;
            product.Category = editProduct.Category;
            product.Price = editProduct.Price;
            product.Quantity = editProduct.Quantity;
            if (image != null)
            {
                DeleteImage(product.ImagePath);
                product.ImagePath = await SaveImageAsync(image);
            }
            await context.SaveChangesAsync();
        }
    }
}
