using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class TransactionsRepository : Repository<TransactionsEntity>, ITransactionsRepository
{
    public TransactionsRepository(ApplicationDBContext dbContext) : base(dbContext) { }
}