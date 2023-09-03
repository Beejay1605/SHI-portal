using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;

[Table("products")]
public class ProductsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required]
    [MaxLength(20)]
    public string PRODUCT_CODE { get; set; }
    [Required]
    [MaxLength(100)]
    public string PRODUCT_NAME { get; set; }
    [Required]
    public decimal SRP_PRICE { get; set; }

    public decimal? MEMBERS_PRICE { get; set; }

    public decimal? NON_MEMBERS_DISCOUNTED_PRICE { get; set; }

    [Required]
    public decimal COMPANY_PROFIT { get; set; }

    [Required]
    public decimal PAYOUT_TOTAL { get; set; }

    [Required]
    [MaxLength(15)]
    public string CATEGORY { get; set; } 
    public string? COVER_PHOTO_PATH { get; set; }
    public string? DESCCRIPTION_FILE_PATH { get; set; }
    [MaxLength(200)]
    public string  MINI_DESCRIPTION { get; set; }
    public DateTime CREATED_AT_UTC { get; set; }
    public DateTime UPDATED_AT_UTC { get; set; }
    public DateTime DELETED_AT_UTC { get; set; }
    [Required, MaxLength(10)]
    public string STATUS { get; set; }
    public string? REMARKS  { get; set; }
    public bool IS_PACKAGE { get; set; } 

    [ForeignKey("created_by")]
    public int CREATED_BY { get; set; } 

    [ForeignKey("updated_by")]
    public int? UPDATED_BY { get; set; }
    
    [Index("IX_CREATED_BY")]
    public virtual OperationsDetailsEntity created_by { get; set; }
    
    [Index("IX_UPDATED_BY")]
    public virtual OperationsDetailsEntity updated_by { get; set; }
}
