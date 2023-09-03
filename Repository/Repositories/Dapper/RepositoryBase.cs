using Dapper;
using Repository.Repositories.Dapper.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Repository.Repositories.Dapper;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
{
    protected readonly IDbConnectionFactory _connectionFactory;

    protected RepositoryBase(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QueryAsync<T>(sql, parameters);
    }

    public virtual async Task<TEntity?> GetSingleAsync(string where, object? parameters = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var tableName = GetTableAttributeName();
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException($"Table Attribute has not been set with this entity: {typeof(TEntity)}");

        return await connection.QuerySingleOrDefaultAsync<TEntity>($"SELECT TOP 1 * FROM {tableName} {@where}", parameters);
    }

    protected string GetTableAttributeName()
    {
        if (typeof(TEntity).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute tableAttribute)
        {
            return tableAttribute.Name;
        }

        return string.Empty;
    }

    public async Task<SqlMapper.GridReader?> QueryMultipleAsync(string sql, object? parameters = null)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        return await connection.QueryMultipleAsync(sql, parameters);
    }
}
