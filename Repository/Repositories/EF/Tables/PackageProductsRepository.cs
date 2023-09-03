using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.EF.Tables;

public class PackageProductsRepository : Repository<PackageProductsEntity>, IPackageProductsRepository
{
    public PackageProductsRepository(ApplicationDBContext dbContext) : base(dbContext) { }
}
