using Manager.Commons.Helpers.Interface;
using Manager.Commons.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manager.Commons.Services.Interfaces;
using Manager.Commons.Services;
using Manager.Commons.Wrappers.Interfaces;
using Manager.Commons.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Manager;

public static class ServiceRegistration
{
    public static void AddManagers(this IServiceCollection services, IConfiguration configuration)
    {
        var ccmsAuthUrl = configuration["Authentication:CCMSUrl"];
        var Issuer = configuration["Jwt:Issuer"];
        var Audience = configuration["Jwt:Audience"];
        var SymetricKey = configuration["Jwt:Key"];


        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddAuthentication()
                .AddIdentityCookies();
        services.AddTransient<IUrlHelper, UrlHelper>();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IFileHelper, FileHelper>();
        services.AddTransient<IQRCodeHelper, QRCodeHelper>();
        services.AddTransient<IKeyHelper, KeyHelper>();
        services.AddTransient<IEntityMapper, EntityMapper>();
        
        
        services.AddTransient<IPathWrapper, PathWrapper>();
        services.AddTransient<IEncryptionHelper, EncryptionHelper>();
        services.AddTransient<IFileWrapper, FileWrapper>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IMimeTypeWrapper, MimeTypeWrapper>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>(_ => new JwtTokenService(Issuer, Audience, SymetricKey));
        
        
    }
}
