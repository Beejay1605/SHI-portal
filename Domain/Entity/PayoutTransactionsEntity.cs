using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

[Table("payout_transaction")]
public class PayoutTransactionsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    [Required]
    [ForeignKey("distributors_details")]
    public int DISTRIBUTOR_REF { get; set; }
    
    public decimal TOTAL_AMOUNT { get; set; }
    
    public DateTime CREATED_DT { get; set; }
    
    
    [ForeignKey("created_by")]
    public int CREATED_BY { get; set; }
    
    
    [Index("IX_DISTRIBUTOR_REF")]
    public virtual DistributorsDetailsEntity distributors_details { get; set; }
    
    [Index("IX_CREATED_BY")]
    public virtual OperationsDetailsEntity created_by { get; set; }
}