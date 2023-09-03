using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class PackageProductMapper : MapperBase<PackageProductsEntity, PackageProductBaseDto>
{
    public PackageProductBaseDto Map(PackageProductsEntity source)
    {
        return new PackageProductBaseDto
        {
            ident = source.ID,
            p_ident = source.PACKAGE_ID,
            single_p_ref = source.PRODUCT_REF,
            quantity = source.QUANTITY
        };
    }

    public PackageProductsEntity Reverse(PackageProductBaseDto source)
    {
        return new PackageProductsEntity
        {
            ID = source.ident,
            PACKAGE_ID = source.p_ident,
            PRODUCT_REF = source.single_p_ref,
            QUANTITY = source.quantity
        };
    }
}