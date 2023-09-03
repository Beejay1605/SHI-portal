using Domain.Entity;

namespace Domain.Mapper;

public class InventoryToVoidMapper : MapperBase<InventoryEntity, VoidInventoryEntity>
{
    public VoidInventoryEntity Map(InventoryEntity source)
    {
        return new VoidInventoryEntity
        {
            ID = source.ID,
            PRODUCT_REF = source.PRODUCT_REF,
            QUANTITY = source.QUANTITY,
            ACTION = source.ACTION,
            VOID_STATUS = source.VOID_STATUS,
            DOC_PATH = source.DOC_PATH,
            CREATE_DATE_UTC = source.CREATE_DATE_UTC,
            CREATED_BY = source.CREATED_BY,
            TRANSACTION_REF = source.TRANSACTION_REF
        };
    }

    public InventoryEntity Reverse(VoidInventoryEntity source)
    {
        return new InventoryEntity
        {
            ID = source.ID,
            PRODUCT_REF = source.PRODUCT_REF,
            QUANTITY = source.QUANTITY,
            ACTION = source.ACTION,
            VOID_STATUS = source.VOID_STATUS,
            DOC_PATH = source.DOC_PATH,
            CREATE_DATE_UTC = source.CREATE_DATE_UTC,
            CREATED_BY = source.CREATED_BY,
            TRANSACTION_REF = source.TRANSACTION_REF
        };
    }
}