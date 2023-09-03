using Domain.DTO.BaseDto;

namespace Domain.DTO.Operations.POS.Output;

public class ReceiptPurchaseOutputDto
{
    public List<POSTransactionBaseDto> posTransactions { get; set; } = new List<POSTransactionBaseDto>();
    public TransactionsBaseDto transaction { get; set; }
    public DistributorsDetailsDto distributor_details { get; set; }
}