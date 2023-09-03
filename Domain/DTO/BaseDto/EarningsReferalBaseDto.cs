namespace Domain.DTO.BaseDto;

public class EarningsReferalBaseDto
{
    public int ident { get; set; }
    
    public int ben_binary_ident { get; set; }
    
    public int from_binary_ident { get; set; }
    
    public int ben_dist_ident { get; set; }
    
    public string type { get; set; }
    public DateTime created_dt { get; set; }
    public bool is_paid { get; set; }
    
    public int? cashout_req_by { get; set; }
    
    public decimal amount { get; set; }
    
    
    public virtual DistributorsDetailsDto dist_details { get; set; }
    public virtual DistributorsDetailsDto from_dist_details { get; set; }
    
    public virtual BinaryTreeBaseDto binary_details { get; set; }
    
    public virtual BinaryTreeBaseDto from_bin_details { get; set; }
}