
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO;

public class UserClaimsBase
{
    public Guid user_id { get; set; }
    public int id { get; set; }
    public string Username { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string email { get; set; } 
    public string ac_level { get; set; }
    public string contact_number { get; set; }
    public DateTime ExpirationDTUTC { get; set; }
    public DateTime exp { get; set; } // 00:05:00 / 5 minutes “Not before” time that identifies the time before which the JWT must not be
    public DateTime iat { get; set; } // “Issued at” time, in Unix time, at which the token was issued
    public Guid jti { get; set; } // JWT ID claim provides a unique identifier for the JWT
}
