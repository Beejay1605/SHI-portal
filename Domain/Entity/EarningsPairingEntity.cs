using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

[Table("earnings_pairing")]
public class EarningsPairingEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    public int? LEFT_BIN_ID { get; set; }
    
    public int? RIGHT_BIN_ID { get; set; }
    
    [ForeignKey("benef_binary")]
    public int BENEF_BIN_ID { get; set; }
    
    [ForeignKey("distributor_details")]
    public int BENEF_DIST_ID { get; set; }
    
    public decimal AMOUNT { get; set; }
    
    public bool IS_ENCASH { get; set; } = false;
    public int LEVEL { get; set; }
    public DateTime CREATED_DT { get; set; } = DateTime.UtcNow;
    
    public int? PAYOUT_TRAN_REF { get; set; }
    
    [Index("IX_BENEF_BIN_ID")]
    public virtual BinaryTreeEntity benef_binary { get; set; }
    
    [Index("IX_BENEF_DIST_ID")]
    public virtual DistributorsDetailsEntity distributor_details { get; set; }
}