using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

[Table("transactions")]
public class TransactionsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string TRANSACTION_NUMBER { get; set; }
    public string TRANSACTION_TYPE { get; set; }
    public bool VOID_STATUS { get; set; }
    public DateTime CREATED_DATE_UTC { get; set; }
    public DateTime UPDATED_DATE_UTC { get; set; }
    
    [ForeignKey("created_by")]
    public int CREATED_BY { get; set; } 

    [ForeignKey("updated_by")]
    public int? UPDATED_BY { get; set; }

    public bool IS_CODE_GENERATED { get; set; } = false;

    public bool IS_ENCODED_UNILEVEL { get; set; } = false;
        
    [Index("IX_CREATED_BY")]
    public virtual OperationsDetailsEntity created_by { get; set; }
    
    [Index("IX_UPDATED_BY")]
    public virtual OperationsDetailsEntity updated_by { get; set; }
}