using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class EarningsUnilevelMapper : MapperBase<EarningsUniLevelEntity, EarningsUnilevelBaseDto>
{
    public EarningsUnilevelBaseDto Map(EarningsUniLevelEntity source)
    {
        return new EarningsUnilevelBaseDto
        {
            ident = source.ID,
            bin_ident = source.BINARY_REF,
            dist_ident = source.DISTRIBUTOR_REF,
            transact_ident = source.TRANSACTION_REF,
            amount = source.AMOUNT,
            created_dt = source.CREATED_DATE,
            available_dt = source.AVAILABILITY_DATE,
            is_paid = source.IS_ENCASH,
            req_by = source.REQUEST_BY
        };
    }

    public EarningsUniLevelEntity Reverse(EarningsUnilevelBaseDto source)
    {
        return new EarningsUniLevelEntity
        {
            ID = source.ident,
            BINARY_REF = source.bin_ident,
            DISTRIBUTOR_REF = source.dist_ident,
            TRANSACTION_REF = source.transact_ident,
            AMOUNT = source.amount,
            CREATED_DATE = source.created_dt,
            AVAILABILITY_DATE = source.available_dt,
            IS_ENCASH = source.is_paid,
            REQUEST_BY = source.req_by
        };
    }
}