using Domain.DTO.BaseDto;

namespace Domain.DTO.Operations.Financials.output;

public class FinancialOutputDto
{
    public List<EarningsUnilevelBaseDto> unilevel { get; set; } = new List<EarningsUnilevelBaseDto>();
    public List<EarningsReferalBaseDto> referals { get; set; } = new List<EarningsReferalBaseDto>();
    public List<EarningsPairingBaseDto> pairings { get; set; } = new List<EarningsPairingBaseDto>();
    public DistributorsDetailsDto details { get; set; }
    public List<PayoutTransactionsBaseDto> payouts { get; set; } = new List<PayoutTransactionsBaseDto>();
}