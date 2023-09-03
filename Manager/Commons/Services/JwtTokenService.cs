using Manager.Commons.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Manager.Commons.Helpers;
using Manager.Commons.Helpers.Interface;
using System.Security.Principal;

namespace Manager.Commons.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;

    public JwtTokenService(string issuer, string audience, string key)
    {
        _issuer = issuer;
        _audience = audience;
        _key = key;
    }

    public string GenerateJwtToken(int id, string username, string firstname, string lastname, 
        string access_level, 
        string email, 
        string contact_number, string refreshToken, Guid user_id)
    {

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("user_id", user_id.ToString()),
                new Claim("Username", username),
                new Claim("id", id.ToString()),
                new Claim("firstname", firstname),
                new Claim("lastname", lastname),
                new Claim("ac_level", access_level),
                new Claim("email", email),
                new Claim("contact_number", contact_number),
                new Claim("jti", Guid.NewGuid().ToString()),
                new Claim("iat_custom", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")),
                new Claim("exp_custom", DateTime.UtcNow.AddMinutes(5).ToString("yyyy-MM-dd HH:mm")),
                new Claim("rtid", refreshToken)
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Audience = _audience,
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return jwtToken;
    }
    
    public bool TokenValidate(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.FromMinutes(5),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)) // The same key as the one that generate the token
            };

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

            // if (jwtToken.ValidTo < DateTime.UtcNow)
            // {
            //     return false;
            // }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidateExpiredToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)) // The same key as the one that generate the token
            };

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            
            return true;
        }
        catch
        {
            return false;
        }
        
    }
}
