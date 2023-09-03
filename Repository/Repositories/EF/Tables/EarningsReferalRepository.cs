using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class EarningsReferalRepository: Repository<EarningsReferalEntity>, IEarningsReferalRepository
{
    public EarningsReferalRepository(ApplicationDBContext dbContext) : base(dbContext)
    {
    }
}