using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;

[Table("package_products")]
public class PackageProductsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required]
    [ForeignKey("package_details")]
    public int PACKAGE_ID { get; set; }

    [Required]
    [ForeignKey("product_details")]
    public int PRODUCT_REF { get; set; }

    public int QUANTITY { get; set; }

    public DateTime CREATE_UPDATE_DT { get; set; } 

    
    [Index("IX_PACKAGE_ID")]
    public virtual ProductsEntity package_details { get; set; }
    
    [Index("IX_PRODUCT_REF")]
    public virtual ProductsEntity product_details { get; set; }
    
}
