using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(int id, string username, string firstname, string lastname, string access_level, string email, string contact_number, string refreshToken, Guid user_id);

    bool TokenValidate(string token);

    bool ValidateExpiredToken(string token);
}
