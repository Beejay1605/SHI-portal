using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Repository.Contexts;

public class ApplicationDBContext : DbContext
{
    public virtual DbSet<UserTokensEntity> UsersTokens { get; set; }
    public virtual DbSet<UserCredentialsEntity> UsersCredentials { get; set; }
    public virtual DbSet<OperationsDetailsEntity> OperationsDetails { get; set; }
    public virtual DbSet<DistributorsDetailsEntity> DistributorsDetails { get; set; }
    public virtual DbSet<ProductsEntity> Products { get; set; }
    public virtual DbSet<ProductImagesEntity> ProductsImages { get; set; }
    public virtual DbSet<PackageProductsEntity> PackageProducts { get; set; }

    public virtual DbSet<PayinCodesEntity> PayinCodes { get; set; }

    public virtual DbSet<TransactionsEntity> Transactions { get; set; }
    public virtual DbSet<PointSaleTransactionsEntity> PointSaleTransactions { get; set; }
    public virtual DbSet<InventoryEntity> InventoryEntities { get; set; }
    public virtual DbSet<VoidInventoryEntity> VoidInventoryEntities { get; set; }

    public virtual DbSet<BinaryTreeEntity> BinaryTree { get; set; }
    
    public virtual DbSet<EarningsUniLevelEntity> EarningsUniLevel { get; set; }
    public virtual DbSet<EarningsReferalEntity> EarningsReferal { get; set; }

    public virtual DbSet<EarningsPairingEntity> EarningsPairing { get; set; }
    public virtual DbSet<PayoutTransactionsEntity> PayoutTransactions { get; set; }
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
}
