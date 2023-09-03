using Repository.Repositories.Dapper.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Dapper;

public class DapperRepositories : IDapperRepositories
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperRepositories(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
}
