using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Repository.Repositories.Dapper;

public interface IRepositoryBase<TEntity>
{
    Task<TEntity?> GetSingleAsync(string where, object? parameters = null);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<GridReader?> QueryMultipleAsync(string sql, object? parameters = null);
}
