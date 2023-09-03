using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class EarningsPairingRepository: Repository<EarningsPairingEntity>, IEarningsPairingRepository
{
    public EarningsPairingRepository(ApplicationDBContext dbContext) : base(dbContext)
    {
    }
}