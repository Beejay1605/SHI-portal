using Domain.DTO;
using Manager.Commons.Const;
using Manager.Commons.Helpers.Interface;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEncryptionHelper encryptionHelper;

    public CurrentUserService(IHttpContextAccessor contextAccessor, IEncryptionHelper encryptionHelper)
    {
        _contextAccessor = contextAccessor;
        this.encryptionHelper = encryptionHelper;
    }

    public async Task<UserClaimsBase> CurrentUser()
    {
        string token = _contextAccessor.HttpContext.Request.Headers["Authorization"];
        token = token.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            return new UserClaimsBase();
        }
        token = encryptionHelper.Decrypt(token);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var jwt_obj = jsonToken as JwtSecurityToken;

        UserClaimsBase result = new UserClaimsBase();

        result.user_id = Guid.Parse(jwt_obj.Claims.First(claim => claim.Type == "user_id").Value);
        result.id = Int32.Parse(jwt_obj.Claims.First(claim => claim.Type == "id").Value);
        result.Username = jwt_obj.Claims.First(claim => claim.Type == "Username").Value;
        result.firstname = jwt_obj.Claims.First(claim => claim.Type == "firstname").Value;
        result.lastname = jwt_obj.Claims.First(claim => claim.Type == "lastname").Value;
        result.ac_level = jwt_obj.Claims.First(claim => claim.Type == "ac_level").Value;
        result.email = jwt_obj.Claims.First(claim => claim.Type == "email").Value;
        result.contact_number = jwt_obj.Claims.First(claim => claim.Type == "contact_number").Value;
        result.jti = Guid.Parse(jwt_obj.Claims.First(claim => claim.Type == "jti").Value);
        result.iat = DateTime.ParseExact(jwt_obj.Claims.First(claim => claim.Type == "iat_custom").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        result.exp = DateTime.ParseExact(jwt_obj.Claims.First(claim => claim.Type == "exp_custom").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        return result;
    }
}
