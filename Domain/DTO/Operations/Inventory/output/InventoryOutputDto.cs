using Domain.DTO.BaseDto;

namespace Domain.DTO.Operations.Inventory.output;

public class InventoryOutputDto
{
    public ProductBaseDto product_details { get; set; }
    public List<InventoryBaseDto> inventory { get; set; }

    public int total_stocks
    {
        get
        {
            return inventory?.Sum(x => x.quantity_stock) ?? 0;
        }
    }
}