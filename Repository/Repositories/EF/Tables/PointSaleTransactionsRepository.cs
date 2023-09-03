using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class PointSaleTransactionsRepository: Repository<PointSaleTransactionsEntity>, IPointSaleTransactionsRepository
{
    public PointSaleTransactionsRepository(ApplicationDBContext dbContext) : base(dbContext) { }
}