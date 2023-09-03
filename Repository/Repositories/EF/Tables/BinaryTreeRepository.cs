using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;

namespace Repository.Repositories.EF.Tables;

public class BinaryTreeRepository :  Repository<BinaryTreeEntity>, IBinaryTreeRepository
{
    public BinaryTreeRepository(ApplicationDBContext dbContext) : base(dbContext) { }
}