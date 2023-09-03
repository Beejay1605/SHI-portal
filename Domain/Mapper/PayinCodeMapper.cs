using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class PayinCodeMapper : MapperBase<PayinCodesEntity, PayinCodeBaseDto>
{
    public PayinCodeBaseDto Map(PayinCodesEntity source)
    {
        return new PayinCodeBaseDto
        {
            ident = source.ID,
            code = source.PAYIN_CODE,
            distributor_ident = source.DISTRIBUTOR_REF,
            tran_ident = source.TRANSACTION_REF,
            date_created = source.CREATED_AT,
            date_updated = source.UPDATED_AT,
            expiration_date = source.EXPIRATION_DT,
            created_by = source.CREATED_BY,
            updated_by = source.UPDATED_BY,
            is_used = source.IS_USED
        };
    }

    public PayinCodesEntity Reverse(PayinCodeBaseDto source)
    {
        return new PayinCodesEntity
        {
            ID = source.ident,
            PAYIN_CODE = source.code,
            DISTRIBUTOR_REF = source.distributor_ident,
            TRANSACTION_REF = source.tran_ident,
            CREATED_AT = source.date_created,
            UPDATED_AT = source.date_updated,
            EXPIRATION_DT = source.expiration_date,
            CREATED_BY = source.created_by,
            UPDATED_BY = source.updated_by,
            IS_USED = source.is_used
        };
    }
}