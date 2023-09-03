using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class TransactionsMapper : MapperBase<TransactionsEntity, TransactionsBaseDto>
{
    public TransactionsBaseDto Map(TransactionsEntity source)
    {
        return new TransactionsBaseDto
        {
            ident = source.ID,
            tran_number = source.TRANSACTION_NUMBER,
            tran_type = source.TRANSACTION_TYPE,
            void_status = source.VOID_STATUS,
            created_dt = source.CREATED_DATE_UTC,
            updated_dt = source.UPDATED_DATE_UTC,
            created_by = source.CREATED_BY,
            updated_by = source.UPDATED_BY,
            is_code_generated = source.IS_CODE_GENERATED
        };
    }

    public TransactionsEntity Reverse(TransactionsBaseDto source)
    {
        return new TransactionsEntity
        {
            ID = source.ident,
            TRANSACTION_NUMBER = source.tran_number,
            TRANSACTION_TYPE = source.tran_type,
            VOID_STATUS = source.void_status,
            CREATED_DATE_UTC = source.created_dt,
            UPDATED_DATE_UTC = source.updated_dt,
            CREATED_BY = source.created_by,
            UPDATED_BY = source.updated_by,
            IS_CODE_GENERATED = source.is_code_generated
        };
    }
}