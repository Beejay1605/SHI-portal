using Domain.DTO.BaseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Products.Output;

public class CreatePageOutputDto
{
    public List<ProductBaseDto> products { get; set; } = new List<ProductBaseDto>();
}
