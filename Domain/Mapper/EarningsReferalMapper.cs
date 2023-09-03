using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class EarningsReferalMapper : MapperBase<EarningsReferalEntity, EarningsReferalBaseDto>
{
    public EarningsReferalBaseDto Map(EarningsReferalEntity source)
    {
        return new EarningsReferalBaseDto
        {
            ident = source.ID,
            ben_binary_ident = source.BENEF_BINARY_REF,
            from_binary_ident = source.FROM_BINARY_REF,
            ben_dist_ident = source.BENEF_DISTRIBUTOR_REF,
            type = source.BONUS_TYPE,
            created_dt = source.CREATED_DT,
            is_paid = source.IS_ENCASH,
            cashout_req_by = source.ENCASH_REQUEST_BY,
            amount = source.AMOUNT
        };
    }

    public EarningsReferalEntity Reverse(EarningsReferalBaseDto source)
    {
        return new EarningsReferalEntity
        {
            ID = source.ident,
            BENEF_BINARY_REF = source.ben_binary_ident,
            FROM_BINARY_REF = source.from_binary_ident,
            BENEF_DISTRIBUTOR_REF = source.ben_dist_ident,
            BONUS_TYPE = source.type,
            CREATED_DT = source.created_dt,
            IS_ENCASH = source.is_paid,
            ENCASH_REQUEST_BY = source.cashout_req_by,
            AMOUNT = source.amount
        };
    }
}