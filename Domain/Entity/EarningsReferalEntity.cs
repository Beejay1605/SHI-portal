using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;


[Table("earnings_referal")]
public class EarningsReferalEntity
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    [ForeignKey("benef_binary")]
    public int BENEF_BINARY_REF { get; set; }
    
    [ForeignKey("from_binary")]
    public int FROM_BINARY_REF { get; set; }
    
    [ForeignKey("distributor_details")]
    public int BENEF_DISTRIBUTOR_REF { get; set; }
    
    public string BONUS_TYPE { get; set; }
    public DateTime CREATED_DT { get; set; }
    public bool IS_ENCASH { get; set; }
    
    public int? ENCASH_REQUEST_BY { get; set; }
    
    public decimal AMOUNT { get; set; }
    public int? PAYOUT_TRAN_REF { get; set; }
    
    
    [Index("IX_BENEF_DISTRIBUTOR_REF")]
    public virtual DistributorsDetailsEntity distributor_details { get; set; }
    
    [Index("IX_BENEF_BINARY_REF")]
    public virtual BinaryTreeEntity benef_binary { get; set; }
    
    [Index("IX_FROM_BINARY_REF")]
    public virtual BinaryTreeEntity from_binary { get; set; }

}