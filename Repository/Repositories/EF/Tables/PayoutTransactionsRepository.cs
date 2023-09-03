using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class PayoutTransactionsRepository: Repository<PayoutTransactionsEntity>, IPayoutTransactionsRepository
{
    public PayoutTransactionsRepository(ApplicationDBContext dbContext) : base(dbContext)
    {
    }
}