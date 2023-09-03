
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;

[Table("user_credentials")]
public class UserCredentialsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID { get; set; }
    [Required]
    public string USERNAME { get; set; }
    [Required]
    public string PASSWORD { get; set; }
    [Required]
    [MaxLength(100)]
    public string EMAIL { get;set; }
    public DateTime CREATED_AT_UTC { get; set; } = DateTime.UtcNow;
    [Required]
    [MaxLength(50)]
    public string ACCESS_LEVEL { get; set; }
    public Nullable<DateTime> UPDATED_AT_UTC { get; set; }
    public Nullable<DateTime> DELETE_AT_UTC { get; set; }

    [MaxLength(50)]
    public string STATUS { get; set; }

    [MaxLength(200)]
    public string REMARKS { get; set; }
    public int TOTAL_ATTEMPTS { get; set; } = 0;
}
