
namespace day08.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IInventoryRepository inventory {  get; }
        Task<bool> SaveChangesAsync();
    }
}
