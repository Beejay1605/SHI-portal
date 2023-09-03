using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class InventoryRepository : Repository<InventoryEntity>, IInventoryRepository
{
    public InventoryRepository( ApplicationDBContext dbContext): base(dbContext)
    {
        
    }
}