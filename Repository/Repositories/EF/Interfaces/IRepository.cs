using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.EF.Interfaces;

public interface IRepository<in TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Patch(IEnumerable<TEntity> entity);
    void Delete(TEntity entity);
    void DeleteList(IEnumerable<TEntity> entity);
    
    Task AddRangeAsync(IEnumerable<TEntity> entities);
}
