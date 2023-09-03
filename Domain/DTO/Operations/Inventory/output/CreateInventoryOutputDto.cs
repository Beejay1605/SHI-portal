using Domain.DTO.BaseDto;
using Domain.Entity;

namespace Domain.DTO.Operations.Inventory.output;

public class CreateInventoryOutputDto
{
    public ProductBaseDto product_details { get; set; }
    public List<InventoryBaseDto> inventory { get; set; } = new List<InventoryBaseDto>();
    
}