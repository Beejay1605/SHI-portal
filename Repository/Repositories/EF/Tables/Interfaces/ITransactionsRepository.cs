using Domain.Entity;
using Repository.Repositories.EF.Interfaces;

namespace Repository.Repositories.EF.Tables.Interfaces;

public interface ITransactionsRepository: IRepository<TransactionsEntity>
{
    
}