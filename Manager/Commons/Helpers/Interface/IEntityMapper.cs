using Domain.DTO.BaseDto;
using Domain.Entity;
using Domain.Mapper;

namespace Manager.Commons.Helpers.Interface;

public interface IEntityMapper
{
    BinaryTreeMapper binaryTreeMap { get; }
    DistributorDetailsMapper distributorDetailsMap { get; }
    
    PackageProductMapper packageProductMapper { get; }
    
    PayinCodeMapper payinCodeMapper { get; }
    ProductImageMapper productImageMapper { get; }
    ProductMapper productMapper { get; }
    
    InventoryMapper inventoryMapper { get; }
    InventoryVoidedMapper inventoryVoidedMapper { get; }
    POSTransactionMapper posTransactionMapper { get; }
    
    TransactionsMapper transactionMapper { get; }
    
    InventoryToVoidMapper inventoryToVoidMapper { get; }
    
    EarningsPairingMapper earningsPairingMapper { get; }
    EarningsReferalMapper earningsReferalMapper { get; }
    EarningsUnilevelMapper earningsUnilevelMapper { get; }
    
    PayoutTransactionMapper payoutTransactionsMapper { get; }
}