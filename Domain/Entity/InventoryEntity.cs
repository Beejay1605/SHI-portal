using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    [Table("inventory")]
    public class InventoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        [Required]
        [ForeignKey("products")]
        public int PRODUCT_REF { get; set; }
        
        [Required]
        public int QUANTITY { get; set; }
        
        [Required]
        public string ACTION { get; set; }
        
        [Required]
        public bool VOID_STATUS { get; set; }
        
        [Required]
        public string DOC_PATH  { get; set; }
        
        [Required]
        public DateTime CREATE_DATE_UTC { get; set; }
        
        public int? TRANSACTION_REF { get; set; }
        
        [Required] 
        [ForeignKey("created_by")]
        public int CREATED_BY { get; set; }
          
        [Index("IX_PRODUCT_REF")]
        public virtual ProductsEntity products { get; set; }
        
        [Index("IX_CREATED_BY")]
        public virtual OperationsDetailsEntity created_by { get; set; }
    }
}
