namespace Domain.DTO.BaseDto;

public class EarningsPairingBaseDto
{
    public int ident { get; set; }
    
    public int? left_bin_ident { get; set; }
    
    public int? right_bin_ident { get; set; }
     
    public int ben_bin_ident { get; set; }
     
    public int ben_dist_ident { get; set; }
    
    public decimal earn_amount { get; set; }
    
    public bool is_paid { get; set; } = false;
    
    public int level { get; set; }
    
    public DateTime dt_created { get; set; } 
     
    public virtual BinaryTreeBaseDto benef_binary { get; set; }
     
    public virtual DistributorsDetailsDto ben_dist_details { get; set; }
    
    public virtual DistributorsDetailsDto right_dist_details { get; set; }
    public virtual DistributorsDetailsDto left_dist_details { get; set; }
}