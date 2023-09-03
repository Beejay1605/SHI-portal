using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Dapper.Configs;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync();
}
