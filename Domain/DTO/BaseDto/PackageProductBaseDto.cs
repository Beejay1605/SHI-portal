using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.BaseDto;

public class PackageProductBaseDto
{ 
    public int ident { get; set; }
    public int p_ident { get; set; }
    public int single_p_ref { get; set; }
    public int quantity { get; set; }

    public ProductBaseDto single_product { get; set; }
}
