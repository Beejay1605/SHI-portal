using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

[Table("earnings_uni_level")]
public class EarningsUniLevelEntity
{ 
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    [Required]
    [ForeignKey("binary_details")]
    public int BINARY_REF { get; set; }  
    
    [Required]
    [ForeignKey("distributor_details")]
    public int DISTRIBUTOR_REF { get; set; }
    
    [Required]
    [ForeignKey("transaction_details")]
    public int TRANSACTION_REF { get; set; } 
    
    public decimal AMOUNT { get; set; }
    
    public DateTime CREATED_DATE { get; set; }
    
    public DateTime AVAILABILITY_DATE { get; set; }
    
    public bool IS_ENCASH { get; set; }
    
    public int? REQUEST_BY { get; set; }
    
    public int? PAYOUT_TRAN_REF { get; set; }
    
    
    [Index("IX_DISTRIBUTOR_REF")]
    public virtual DistributorsDetailsEntity distributor_details { get; set; }
    
    [Index("IX_TRANSACTION_REF")]
    public virtual TransactionsEntity transaction_details { get; set; }
    
    [Index("IX_BINARY_REF")]
    public virtual BinaryTreeEntity binary_details { get; set; }
}