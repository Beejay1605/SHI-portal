using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;

[Table("operations_details")]
public class OperationsDetailsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [ForeignKey("user_credentials")]
    [Required]
    public Guid CRED_REF { get; set; }

    [Required]
    [MaxLength(100)]
    public string FIRST_NAME { get; set; }
    [Required]
    [MaxLength(100)]
    public string LAST_NAME { get; set;}
    public string MIDDLE_NAME { get; set; }
    public string POSITION { get; set; }
    [Required]
    public DateTime BIRTH_DATE { get; set; }
    [Required, MaxLength(220)]
    public string ADDRESS { get; set; }
    [Required]
    public int MOBILE_NUMBER { get; set; }
    [Required]
    public string MOBILE_COUNTRY_CODE { get; set; }
    public DateTime UPDATED_AT_UTC { get; set; }
    [Required]
    public string NATIONALITY { get; set; }
    
    
    [Index("IX_CRED_REF")]
    public virtual UserCredentialsEntity user_credentials { get; set; }
}
