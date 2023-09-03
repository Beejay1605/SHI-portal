
using Manager;
using Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using WebPortalMVC.GlobalErrorHandling;
using Repository.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WebPortalMVC.Extensions;

[ExcludeFromCodeCoverage]
public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = false
        };

        _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParameters;
            options.SaveToken = true;
        });

        _ = builder.Services.AddAuthorization();

        builder.Services.AddManagers(builder.Configuration);
        builder.Services.AddRepository(builder.Configuration);
        
        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        var ti = CultureInfo.CurrentCulture.TextInfo;

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();
        _ = app.UseHttpsRedirection();
        _ = app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        using (var scope = app.Services.CreateScope())
        {
            var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            //dataContext.Database.Migrate();
            //var dataDefaultcontext = scope.ServiceProvider.GetService<>
        }

        return app;
    }   
}
