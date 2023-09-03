using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;

[Table("payin_codes")]
public class PayinCodesEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required] 
    public string PAYIN_CODE { get; set; }

    [Required]
    [ForeignKey("distributor_details")]
    public int DISTRIBUTOR_REF { get; set; }
    
    [Required]
    public int TRANSACTION_REF { get; set; }
    
    public DateTime CREATED_AT { get; set; }
    public DateTime UPDATED_AT { get; set; }
    public DateTime EXPIRATION_DT { get; set; }
    
    [Required]
    [ForeignKey("created_by")]
    public int CREATED_BY { get; set; }

    [Required]
    [ForeignKey("updated_by")]
    public int? UPDATED_BY { get; set; }

    public bool IS_USED { get; set; } = false;
    
    
    
    [Index("IX_DISTRIBUTOR_REF")]
    public virtual DistributorsDetailsEntity distributor_details { get; set; }
    
    [Index("IX_CREATED_BY")]
    public virtual OperationsDetailsEntity created_by { get; set; }
    
    [Index("IX_UPDATED_BY")]
    public virtual OperationsDetailsEntity updated_by { get; set; }
}
