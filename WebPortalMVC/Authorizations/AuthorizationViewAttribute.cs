using Manager.Commons.Const;
using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Dapper;
using Domain.Entity;
using Manager.Commons.Helpers;
using Manager.Commons.Helpers.Interface;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.EF.Interfaces;

namespace WebPortalMVC.Authorizations;

public class AuthorizationViewAttribute : TypeFilterAttribute
{
    public AuthorizationViewAttribute(string claimValue) : base(typeof(ClaimAcessFilter))
    {
        Arguments = new object[] { new Claim("ac_level", claimValue) };
    }
}

public class ClaimAcessFilter : IAuthorizationFilter
{
    public ClaimAcessFilter(Claim claim, IHttpContextAccessor contextAccessor, IEncryptionHelper encryptionHelper, IJwtTokenService tokenService)
    {
        _claim = claim;
        _contextAccessor = contextAccessor;
        _encryptionHelper = encryptionHelper;
        _tokenService = tokenService;
    }

    readonly Claim _claim;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEncryptionHelper _encryptionHelper;
    private readonly IJwtTokenService _tokenService;

    
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            string token_to_decrypt = _contextAccessor.HttpContext.Request.Headers["Authorization"];
            token_to_decrypt = _encryptionHelper.Decrypt(token_to_decrypt.Replace("Bearer ",""));
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token_to_decrypt);
            var token_claims = jsonToken as JwtSecurityToken;

            var hasClaim = token_claims.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim)
            {
                context.Result = new RedirectToActionResult("Index", "Login",new {});
            }
            string ac_level = token_claims.Claims.First(claim => claim.Type == "ac_level").Value; 
            
            bool validate_result = _tokenService.TokenValidate(token_to_decrypt);
            
            if (validate_result == false)
            {
                context.Result = new RedirectToActionResult("Index", "Login",new {});
            }
        }
        catch(Exception ex)
        {
            context.Result = new RedirectToActionResult("Index", "Login",new {});
        }
    }
}