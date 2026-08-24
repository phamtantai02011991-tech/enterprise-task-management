using day08.Core.ViewModels;

namespace day08.Core.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<ProductViewModel>> GetStockListAsync();
        Task<(bool IsSuccess, string Message)> ProcessStockTransactionAsync(StockTransactionViewModel model);
    }
}
