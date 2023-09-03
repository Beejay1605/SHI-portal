using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class InventoryMapper :  MapperBase<InventoryEntity, InventoryBaseDto>
{
    public InventoryBaseDto Map(InventoryEntity source)
    {
        return new InventoryBaseDto
        {
            ident = source.ID,
            product_ident = source.PRODUCT_REF,
            quantity_stock = source.QUANTITY,
            actions = source.ACTION,
            status = source.VOID_STATUS,
            document_path = source.DOC_PATH,
            created_dt = source.CREATE_DATE_UTC,
            transaction_ident = source.TRANSACTION_REF,
            created_ident = source.CREATED_BY
        };
    }

    public InventoryEntity Reverse(InventoryBaseDto source)
    {
        return new InventoryEntity
        {
            ID = source.ident,
            PRODUCT_REF = source.product_ident,
            QUANTITY = source.quantity_stock,
            ACTION = source.actions,
            VOID_STATUS = source.status,
            DOC_PATH = source.document_path,
            CREATE_DATE_UTC = source.created_dt,
            TRANSACTION_REF = source.transaction_ident,
            CREATED_BY = source.created_ident
        };
    }
}