using Domain.DTO.BaseDto;
using Domain.Entity;
using Domain.Mapper;
using Manager.Commons.Helpers.Interface;

namespace Manager.Commons.Helpers;

public class EntityMapper : IEntityMapper
{

    public BinaryTreeMapper binaryTreeMap => new BinaryTreeMapper();
    public DistributorDetailsMapper distributorDetailsMap => new DistributorDetailsMapper();

    public PackageProductMapper packageProductMapper => new PackageProductMapper();
    
    public PayinCodeMapper payinCodeMapper => new PayinCodeMapper();
    
    public ProductImageMapper productImageMapper => new ProductImageMapper();
    
    public ProductMapper productMapper => new ProductMapper();

    public InventoryMapper inventoryMapper => new InventoryMapper();
    public InventoryVoidedMapper inventoryVoidedMapper => new InventoryVoidedMapper();
    public POSTransactionMapper posTransactionMapper => new POSTransactionMapper();
    public TransactionsMapper transactionMapper => new TransactionsMapper();
    public InventoryToVoidMapper inventoryToVoidMapper => new InventoryToVoidMapper();
    public EarningsPairingMapper earningsPairingMapper => new EarningsPairingMapper();
    public EarningsReferalMapper earningsReferalMapper => new EarningsReferalMapper();
    public EarningsUnilevelMapper earningsUnilevelMapper => new EarningsUnilevelMapper();
    public PayoutTransactionMapper payoutTransactionsMapper => new PayoutTransactionMapper();
}