namespace Domain.DTO.BaseDto;

public class EarningsUnilevelBaseDto
{
    public int ident { get; set; }
     
    public int bin_ident { get; set; }  
     
    public int dist_ident { get; set; }
     
    public int transact_ident { get; set; } 
    
    public decimal amount { get; set; }
    
    public DateTime created_dt { get; set; }
    
    public DateTime available_dt { get; set; }
    
    public bool is_paid { get; set; }
    
    public int? req_by { get; set; }
    
     
    public virtual DistributorsDetailsDto dist_details { get; set; }
     
    public virtual TransactionsBaseDto transaction_details { get; set; }
     
    public virtual BinaryTreeBaseDto binary_details { get; set; }
}