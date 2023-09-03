using Repository.Repositories.EF.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.EF.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserCredentialsRepository UserCredentialsRepository { get; }
    IUserTokensRepository UserTokensRepository { get; }
    IOperationsDetailsRepository OperationsDetailsRepository { get; }
    IDistributorsDetailsRepository DistributorsDetailsRepository { get; }
    
    IProductImageRepository ProductImageRepository { get; } 
    IProductRepository ProductRepository { get; }

    IPackageProductsRepository PackageProductsRepository { get; }

    IPayinCodesRepository PayinCodesRepository { get; }
    
    IPointSaleTransactionsRepository PointSaleTransactionsRepository { get; }
    
    ITransactionsRepository TransactionsRepository { get; }

    
    IInventoryRepository InventoryRepository { get; }
    IVoidInventoryRepository VoidInventoryRepository { get; }


    IBinaryTreeRepository BinaryTreeRepository { get; }

    IEarningsUniLevelRepository EarningsUniLevel { get; }
    
    IEarningsReferalRepository EarningsReferal { get; }
    
    IEarningsPairingRepository EarningsPairing { get; }
    
    IPayoutTransactionsRepository PayoutTransactions { get; }

    Task<int> CommitAsync();
    Task RollbackAsync();
}
