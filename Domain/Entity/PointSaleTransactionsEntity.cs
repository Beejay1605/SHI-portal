using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;


[Table("pos_transactions")]
public class PointSaleTransactionsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    
    [Required]
    [ForeignKey("transaction")]
    public int TRANSACTION_REF { get; set; }
    
    [ForeignKey("product_details")]
    public int PRODUCT_REF { get; set; }
    
    public decimal SRP_PRICE { get; set; }
    public decimal? MEMBERS_PRICE { get; set; }
    public decimal? NON_MEMBERS_DISCOUNTED_PRICE { get; set; }
    
    [Required]
    public decimal COMPANY_PROFIT { get; set; }
    [Required]
    public decimal PAYOUT_TOTAL { get; set; }
    
    [Required]
    public decimal PER_UNIT_PRICE { get; set; }
    
    [Required]
    public int QUANTITY { get; set; }
    
    [Required]
    public string PAYMENT_TYPE { get; set; }
    
    [Required]
    public string BRANCH { get; set; }
    

    public int? DISTRIBUTOR_REF { get; set; }
    
    [Required]
    public decimal VAT_PERCENTAGE { get; set; }
    
    [Index("IX_PRODUCT_REF")]
    public virtual ProductsEntity product_details { get; set; }
    
    [Index("IX_TRANSACTION_REF")]
    public virtual TransactionsEntity transaction { get; set; }
     
}