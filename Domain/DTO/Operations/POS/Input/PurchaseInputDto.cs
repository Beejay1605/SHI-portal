namespace Domain.DTO.Operations.POS.Input;

public class PurchaseInputDto
{
    public int distributor_id { get; set; }
    public List<PurchaseProdInputDto> prod_quantity { get; set; }  // product_id|quantity
}

public class PurchaseProdInputDto
{
    public string productId { get; set; }
    public int quantity { get; set; }
}