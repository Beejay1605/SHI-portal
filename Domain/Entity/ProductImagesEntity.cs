using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity;

[Table("product_images")]
public class ProductImagesEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required]
    public string PHOTO_PATH { get; set; }
    
    public int IMG_INDEX { get; set; }

    [Required]
    [ForeignKey("product_details")]
    public int PRODUCT_REF { get; set; }
    
    [Index("IX_PRODUCT_REF")]
    public virtual ProductsEntity product_details { get; set; }
}
