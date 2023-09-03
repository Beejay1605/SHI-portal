using Domain.Entity;
using Repository.Repositories.Dapper.Configs;
using Repository.Repositories.Dapper.Tables.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Dapper.Tables;

public class UserTokensDRepository : RepositoryBase<UserTokensEntity>, IUserTokensDRepository
{
    public UserTokensDRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
}
