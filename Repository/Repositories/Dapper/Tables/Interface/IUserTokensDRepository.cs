using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Dapper.Tables.Interface;

public interface IUserTokensDRepository : IRepositoryBase<UserTokensEntity>
{
  //  Task<IEnumerable<UserTokensEntity>> GetBarangaysByRegionIdAndProvinceIdAndMunicipalIdAsync();
}
