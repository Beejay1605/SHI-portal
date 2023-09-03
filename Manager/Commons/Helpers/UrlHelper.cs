using Manager.Commons.Helpers.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers;

public class UrlHelper : IUrlHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GenerateUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        return string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent(), request?.Path, request?.QueryString);
    }
    public string GenerateUrl(string path)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        return string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent(), path);
    }

    public string GenerateUrl(string apiPath, string path, string query)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        return string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent(), apiPath, path, query);
    }

    public string GenerateConfirmationEmailUrl(int userId, string code)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        return string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent(), $"/Account/ConfirmEmail?userId={userId}&code={code}");
    }
}
