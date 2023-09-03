using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Services.Interfaces;

public interface ICurrentUserService
{
    Task<UserClaimsBase> CurrentUser();
}
