using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class VoidInventoryRepository : Repository<VoidInventoryEntity>, IVoidInventoryRepository
{
    public VoidInventoryRepository( ApplicationDBContext dbContext): base(dbContext)
    {
            
    }
}