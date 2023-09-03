using Domain.Entity;
using Repository.Contexts;
using Repository.Repositories.EF.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.EF.Tables;

public class OperationsDetailsRepository : Repository<OperationsDetailsEntity>, IOperationsDetailsRepository
{
    public OperationsDetailsRepository(ApplicationDBContext dbContext) : base(dbContext) { }
}
