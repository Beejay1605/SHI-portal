using Repository.Contexts;
using Repository.Repositories.EF.Interfaces;
using Repository.Repositories.EF.Tables;
using Repository.Repositories.EF.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.EF;

public class UnitOfWork : IUnitOfWork
{
    private bool _isDisposed;
    private readonly ApplicationDBContext _dbContext;

    public UnitOfWork(ApplicationDBContext dbContext) => _dbContext = dbContext;

    public IUserCredentialsRepository UserCredentialsRepository => new UserCredentialsRepository(_dbContext);
    public IUserTokensRepository UserTokensRepository => new UserTokensRepository(_dbContext);
    public IOperationsDetailsRepository OperationsDetailsRepository => new OperationsDetailsRepository(_dbContext);
    public IDistributorsDetailsRepository DistributorsDetailsRepository => new DistributorsDetailsRepository(_dbContext);

    public IProductImageRepository ProductImageRepository =>  new ProductImageRepository(_dbContext);

    public IProductRepository ProductRepository =>  new ProductRepository(_dbContext);

    public IPackageProductsRepository PackageProductsRepository => new PackageProductsRepository(_dbContext);

    public IPayinCodesRepository PayinCodesRepository => new PayinCodesRepository(_dbContext);

    public IPointSaleTransactionsRepository PointSaleTransactionsRepository => new PointSaleTransactionsRepository(_dbContext);
    public ITransactionsRepository TransactionsRepository  => new TransactionsRepository(_dbContext);
    
    public IInventoryRepository InventoryRepository => new InventoryRepository(_dbContext);

    public IVoidInventoryRepository VoidInventoryRepository => new VoidInventoryRepository(_dbContext);
    public IBinaryTreeRepository BinaryTreeRepository => new BinaryTreeRepository(_dbContext);

    public IEarningsUniLevelRepository EarningsUniLevel => new EarningsUniLevelRepository(_dbContext);
    public IEarningsReferalRepository EarningsReferal => new EarningsReferalRepository(_dbContext);
    public IEarningsPairingRepository EarningsPairing => new EarningsPairingRepository(_dbContext);

    public IPayoutTransactionsRepository PayoutTransactions => new PayoutTransactionsRepository(_dbContext);

    public async Task<int> CommitAsync() => await _dbContext.SaveChangesAsync();

    
    
    public async Task RollbackAsync() => await _dbContext.DisposeAsync();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing) _dbContext.Dispose();

        _isDisposed = true;
    }

    ~UnitOfWork() => Dispose(false);
}
