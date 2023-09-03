using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Operations.Payincodes.output;

public class PayinCodeGenerateOutputDto
{
    public string qr_code_base64 { get; set; }
    public string payin_code { get; set; }
    public DateTime expiration_date { get; set; }
}
