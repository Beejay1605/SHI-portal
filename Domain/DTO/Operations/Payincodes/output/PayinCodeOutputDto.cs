using Domain.DTO.BaseDto;

namespace Domain.DTO.Operations.Payincodes.output;

public class PayinCodeOutputDto
{
    public List<PayinCodeBaseDto> payins { get; set; } = new List<PayinCodeBaseDto>();
}
