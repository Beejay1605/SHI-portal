namespace Domain.DTO.BaseDto;

public class PayoutTransactionsBaseDto
{
    public int ident { get; set; }
    
    public int distributor_ident { get; set; }
    
    public decimal amount { get; set; }
    
    public DateTime created_dt { get; set; }
    
    public int created_by { get; set; }
    
    public virtual DistributorsDetailsDto distributors_details { get; set; }
}