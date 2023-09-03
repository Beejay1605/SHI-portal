namespace Domain.DTO.BaseDto;

public class BinaryTreeBaseDto
{
    public int ident { get; set; }
    public int distibutor_ident { get; set; }
    public int? upline_ident { get; set; }
    public int? grand_upline_ident { get; set; }
    public int? parent_bin_ident { get; set; }
    public int? child_left_bin_ident { get; set; }
    public int? child_right_bin_ident { get; set; }
    public string? position { get; set; }
    public int level { get; set; }
    public DateTime created_dt { get; set; }
    public DateTime? updated_dt { get; set; }

    public virtual DistributorsDetailsDto? distributors_details { get; set; }
    public virtual DistributorsDetailsDto? upline_details { get; set; }
    public virtual DistributorsDetailsDto? grand_upline_details { get; set; }
    public virtual BinaryTreeBaseDto? parent_bin { get; set; } 
    public virtual BinaryTreeBaseDto? child_left_bin { get; set; }
    public virtual BinaryTreeBaseDto? child_right_bin { get; set; }
}