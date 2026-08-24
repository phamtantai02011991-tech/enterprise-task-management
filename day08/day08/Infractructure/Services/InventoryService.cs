using AutoMapper;
using day08.Core.Entities;
using day08.Core.Interfaces;
using day08.Core.ViewModels;

namespace day08.Infractructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<ProductViewModel>> GetStockListAsync()
        {
            var products = await unitOfWork.inventory.GetAllProductsAsync();
            return mapper.Map<IEnumerable<ProductViewModel>>(products);
        }

        public async Task<(bool IsSuccess, string Message)> ProcessStockTransactionAsync(StockTransactionViewModel model)
        {
            if (model.Quantity <= 0)
            {
                return (false, "Số lượng giao dịch phải lớn hơn 0");
            }

            var product = await unitOfWork.inventory.GetProductByIdAsync(model.ProductId);
            if (product == null) return (false, "Sản phẩm không tồn tại");
            if(model.Type == Core.Entities.TransactionType.Export)
            {
                if (product.StockQuantity < model.Quantity) return (false, $"Không đủ hàng trong kho (Tồn kho hiện tại: {product.StockQuantity})");
                product.StockQuantity -= model.Quantity;
            }
            else
            {
                product.StockQuantity += model.Quantity;
            }
            var transaction = mapper.Map<InventoryTransaction>(model);
            await unitOfWork.inventory.AddTransactionAsync(transaction);
            await unitOfWork.SaveChangesAsync();
            return (true, "Giao dịch thành công");
        }
    }
}
