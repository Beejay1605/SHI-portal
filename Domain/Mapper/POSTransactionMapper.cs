using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.Mapper;

public class POSTransactionMapper : MapperBase<PointSaleTransactionsEntity, POSTransactionBaseDto>
{
    public POSTransactionBaseDto Map(PointSaleTransactionsEntity source)
    {
        return new POSTransactionBaseDto
        {
            ident = source.ID,
            transaction_ident = source.TRANSACTION_REF,
            srp_price = source.SRP_PRICE,
            distributors_price = source.MEMBERS_PRICE,
            non_dist_price = source.NON_MEMBERS_DISCOUNTED_PRICE,
            company_profit = source.COMPANY_PROFIT,
            total_payout = source.PAYOUT_TOTAL,
            unit_price = source.PER_UNIT_PRICE,
            quantity = source.QUANTITY,
            payment = source.PAYMENT_TYPE,
            branch = source.BRANCH,
            distributor_ident = source.DISTRIBUTOR_REF,
            vat_percent = source.VAT_PERCENTAGE
        };
    }

    public PointSaleTransactionsEntity Reverse(POSTransactionBaseDto source)
    {
        return new PointSaleTransactionsEntity
        {
            ID = source.ident,
            TRANSACTION_REF = source.transaction_ident,
            SRP_PRICE = source.srp_price,
            MEMBERS_PRICE = source.distributors_price,
            NON_MEMBERS_DISCOUNTED_PRICE = source.non_dist_price,
            COMPANY_PROFIT = source.company_profit,
            PAYOUT_TOTAL = source.total_payout,
            PER_UNIT_PRICE = source.unit_price,
            QUANTITY = source.quantity,
            PAYMENT_TYPE = source.payment,
            BRANCH = source.branch,
            DISTRIBUTOR_REF = source.distributor_ident,
            VAT_PERCENTAGE = source.vat_percent
        };
    }
}