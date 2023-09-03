using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class EarningsUniLevelRepository : Repository<EarningsUniLevelEntity>, IEarningsUniLevelRepository
{
    public EarningsUniLevelRepository(ApplicationDBContext dbContext) : base(dbContext)
    {
    }
}