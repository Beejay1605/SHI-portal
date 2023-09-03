using Domain.DTO.BaseDto;

namespace Domain.DTO.Distributor.Dashboard.output;

public class DashboardOutputDto
{
    
    public decimal total_earn { get; set; }
    public decimal balance { get; set; }
    public decimal cashout_request { get; set; }
    public List<POSTransactionBaseDto> purchase_history { get; set; } = new List<POSTransactionBaseDto>();
    public List<TransachtionHistoryOutputDto> transaction_history { get; set; } = new List<TransachtionHistoryOutputDto>();
    public List<PayoutTransactionsBaseDto> payout_history { get; set; } = new List<PayoutTransactionsBaseDto>();
}

public class TransachtionHistoryOutputDto
{
    public string created_dt { get; set; }
    public decimal amount { get; set; }
    public string type { get; set; }
}