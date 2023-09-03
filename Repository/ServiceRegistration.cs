using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories.EF.Interfaces;
using Repository.Repositories.EF;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Contexts;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.Dapper;
using IDbConnectionFactory = Repository.Repositories.Dapper.Configs.IDbConnectionFactory;
using MySql.EntityFrameworkCore;

namespace Repository;

public static class ServiceRegistration
{   
    public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
        services.AddDbContext<ApplicationDBContext>(options => options.UseMySQL(connectionString)); 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDapperRepositories, DapperRepositories>();
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(connectionString));
    }
}
