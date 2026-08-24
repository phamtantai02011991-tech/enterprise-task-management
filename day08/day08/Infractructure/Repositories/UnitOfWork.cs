using day08.Core.Interfaces;
using day08.Infractructure.Data;

namespace day08.Infractructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        public IInventoryRepository inventory { get; }

        //public IInventoryRepository inventory => throw new NotImplementedException();

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            inventory = new InventoryRepository(context);
        }

        public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
    }
}
