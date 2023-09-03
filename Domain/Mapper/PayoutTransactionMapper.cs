using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class PayoutTransactionMapper : MapperBase<PayoutTransactionsEntity, PayoutTransactionsBaseDto>
{
    public PayoutTransactionsBaseDto Map(PayoutTransactionsEntity source)
    {
        return new PayoutTransactionsBaseDto
        {
            ident = source.ID,
            distributor_ident = source.DISTRIBUTOR_REF,
            amount = source.TOTAL_AMOUNT,
            created_dt = source.CREATED_DT,
            created_by = source.CREATED_BY
        };
    }

    public PayoutTransactionsEntity Reverse(PayoutTransactionsBaseDto source)
    {
        return new PayoutTransactionsEntity
        {
            ID = source.ident,
            DISTRIBUTOR_REF = source.distributor_ident,
            TOTAL_AMOUNT = source.amount,
            CREATED_DT = source.created_dt,
            CREATED_BY = source.created_by
        };
    }
}