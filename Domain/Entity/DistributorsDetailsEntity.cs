using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Http;

namespace Domain.Entity;

[Table("distributors_details")]
public class DistributorsDetailsEntity
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DISTRIBUTOR_ID { get; set; }
    
    [ForeignKey("user_credentials")]
    public Guid USER_CRED_REF { get; set; }

    [Required]
    [MaxLength(100)]
    public string FIRSTNAME { get; set; }
    [Required]
    [MaxLength(100)]
    public string LASTNAME { get; set; }
    [MaxLength(100)]
    public string MIDDLENAME { get; set; }
    [MaxLength(30)]
    public string SUFFIX { get; set; }
    [Required]
    [MaxLength(250)]
    public string ADDRESS { get; set; }
    [Required]
    [MaxLength(11)]
    public string CONTACT_NUMBER { get; set; }

    [Required]
    public DateTime BIRTH_DATE { get; set; }

    [MaxLength(200)]
    public string MESSENGER_ACCOUNT { get; set; }

    [MaxLength(30)]
    public string TIN_NUMBER { get; set; }
    [Required]
    public string ACCOUNT_TYPE { get; set; }
    [Required]
    public int NUMBER_OF_ACCOUNTS { get; set; }

    [Required]
    [MaxLength(20)]
    public string GENDER { get; set; }
    
    [ForeignKey("upline_details")]
    public Nullable<int> UPLINE_REF_ID { get; set; }
    public string? PICTURE_PATH { get; set; } 

    [ForeignKey("created_by")]
    public int CREATED_BY { get; set; }

    [ForeignKey("updated_by")]
    public int? UPDATED_BY { get; set; }
    
    [Index("IX_UPLINE_REF_ID")]
    public virtual DistributorsDetailsEntity upline_details { get; set; }
    
    [Index("IX_USER_CRED_REF")]
    public virtual UserCredentialsEntity user_credentials { get; set; }
    
    [Index("IX_CREATED_BY")]
    public virtual OperationsDetailsEntity created_by { get; set; }
    
    [Index("IX_UPDATED_BY")]
    public virtual OperationsDetailsEntity updated_by { get; set; }
}
