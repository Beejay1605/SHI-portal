using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

[Table("binary_tree")]
public class BinaryTreeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    [Required]
    [ForeignKey("distributors_details")]
    public int DISTRIBUTOR_REF { get; set; }
    
    [ForeignKey("upline_details")]
    public Nullable<int> UPLINE_DETAILS_REF { get; set; }
    
    [ForeignKey("grand_upline_details")]
    public Nullable<int> GRAND_UPLINE_DETAILS_REF { get; set; }
    
    [ForeignKey("parent_binary")]
    public Nullable<int> PARENT_BINARY_REF { get; set; }
    

    [ForeignKey("child_left_binary")]
    public Nullable<int> CHILD_LEFT_BINARY_REF { get; set; }
    

    [ForeignKey("child_right_binary")]
    public Nullable<int> CHILD_RIGHT_BINARY_REF { get; set; }
    

    public string? POSITION { get; set; }
    
    [Required]
    public int LEVELS { get; set; }
    
    public DateTime CREATED_AT_UTC { get; set; }
    
    public DateTime? UPDATED_AT_UTC { get; set; }
    
    
    [ForeignKey("payin_code")]
    public int PAYIN_CODE_REF { get; set; }
    
    
    public int? IMAGINARY_UPLINE_BIN_REF { get; set; }
    
    
    [Index("IX_PAYIN_CODE_REF")]
    public virtual PayinCodesEntity payin_code { get; set; }
    
    [Index("IX_DISTRIBUTOR_REF")]
    public virtual DistributorsDetailsEntity distributors_details { get; set; }
    
    [Index("IX_UPLINE_DETAILS_REF")]
    public virtual DistributorsDetailsEntity upline_details { get; set; }
    
    [Index("IX_GRAND_UPLINE_DETAILS_REF")]
    public virtual DistributorsDetailsEntity grand_upline_details { get; set; }
    
    [Index("IX_PARENT_BINARY_REF")]
    public virtual BinaryTreeEntity parent_binary { get; set; }
    
    [Index("IX_CHILD_LEFT_BINARY_REF")]
    public virtual BinaryTreeEntity child_left_binary { get; set; }
    
    [Index("IX_CHILD_RIGHT_BINARY_REF")]
    public virtual BinaryTreeEntity child_right_binary { get; set; }
     
    
}