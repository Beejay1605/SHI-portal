using Domain.DTO.BaseDto;

namespace Domain.DTO.Operations.Products.Output;

public class GetAllProductsOutputDto
{
    public List<ProductBaseDto> Products { get; set; } = new List<ProductBaseDto>();
}