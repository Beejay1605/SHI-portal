using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.DTO.Operations.POS.Output;

public class VoidOutputDto
{
    public DistributorsDetailsDto distributors_details { get; set; }
    public TransactionsBaseDto transaction { get; set; }
    public List<POSTransactionBaseDto> pos_tran { get; set; } = new List<POSTransactionBaseDto>();
}
