using Domain.Entity;

namespace Domain.DTO.Operations.Inventory.input;

public class CreateInventoryInputDto
{ 
    public int quantity { get; set; }
    public string doc_body { get; set; }
    public int product_id { get; set; }
}