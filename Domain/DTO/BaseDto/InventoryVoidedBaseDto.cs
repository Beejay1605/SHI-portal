using Domain.Entity;

namespace Domain.DTO.BaseDto;

public class InventoryVoidedBaseDto
{
    public int ident { get; set; }

    public int product_ident { get; set; }

    public int quantity_stock { get; set; }

    public string actions { get; set; }

    public bool status { get; set; }

    public string document_path  { get; set; }

    public DateTime created_dt { get; set; }

    public int? transaction_ident { get; set; }

    public int created_ident { get; set; }
    
    public virtual TransactionsEntity transactions { get; set; }
        
    public virtual ProductsEntity products { get; set; }
        
    public virtual OperationsDetailsEntity created_by { get; set; }
}