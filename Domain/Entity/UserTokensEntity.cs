using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;


[Table("user_tokens")]
public class UserTokensEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }


    [Required]
    [ForeignKey("user_credentials")]
    public Guid USER_REF { get; set; }

    [Required]
    [MaxLength(100)]
    public string TOKEN { get; set; }
    
    public DateTime EXPIRATION_UTC { get; set; } = DateTime.UtcNow;
    public bool IS_USED { get; set; } = false;
    
    [Index("IX_USER_REF")]
    public virtual UserCredentialsEntity user_credentials { get; set; }
}
