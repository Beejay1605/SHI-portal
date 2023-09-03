using Repository.Repositories.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.Contexts;

namespace Repository.Repositories.EF;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _entities;
    public Repository(ApplicationDBContext dbContext)
    {
        _entities = dbContext.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity) => await _entities.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await _entities.AddRangeAsync(entities);



    public void Delete(TEntity entity) => _entities.Remove(entity);
    public void DeleteList(IEnumerable<TEntity> entity) => _entities.RemoveRange(entity);
    public void Update(TEntity entity) => _entities.Update(entity);

    public void Patch(IEnumerable<TEntity> entity) => _entities.UpdateRange(entity);
}
