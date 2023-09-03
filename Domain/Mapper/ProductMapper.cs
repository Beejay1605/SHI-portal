using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;



public class ProductMapper : MapperBase<ProductsEntity, ProductBaseDto>
{
    public ProductBaseDto Map(ProductsEntity source)
    {
        return new ProductBaseDto
        {
            ident = source.ID,
            p_code = source.PRODUCT_CODE,
            p_name = source.PRODUCT_NAME, 
            p_price = source.SRP_PRICE,
            membership_price = source.MEMBERS_PRICE,
            non_membership_discounted_price = source.NON_MEMBERS_DISCOUNTED_PRICE,
            profit = source.COMPANY_PROFIT,
            total_payout = source.PAYOUT_TOTAL,
            p_category = source.CATEGORY,
            p_cover_photo = source.COVER_PHOTO_PATH,
            p_description = source.DESCCRIPTION_FILE_PATH,
            p_mini_desc = source.MINI_DESCRIPTION,
            p_created_dt = source.CREATED_AT_UTC,
            p_updated_dt = source.UPDATED_AT_UTC,
            p_status = source.STATUS,
            p_remarks = source.REMARKS,
            p_is_package = source.IS_PACKAGE
        };
    }

    public ProductsEntity Reverse(ProductBaseDto source)
    {
        return new ProductsEntity
        {
            ID = source.ident,
            PRODUCT_CODE = source.p_code,
            PRODUCT_NAME = source.p_name,
            SRP_PRICE = source.p_price,
            MEMBERS_PRICE = source.membership_price,
            NON_MEMBERS_DISCOUNTED_PRICE = source.non_membership_discounted_price,
            COMPANY_PROFIT = source.profit,
            PAYOUT_TOTAL = source.total_payout,
            CATEGORY = source.p_category,
            COVER_PHOTO_PATH = source.p_cover_photo,
            DESCCRIPTION_FILE_PATH = source.p_description,
            MINI_DESCRIPTION = source.p_mini_desc,
            CREATED_AT_UTC = source.p_created_dt,
            UPDATED_AT_UTC = source.p_updated_dt,
            STATUS = source.p_status,
            REMARKS = source.p_remarks,
            IS_PACKAGE = source.p_is_package
        };
    }
}