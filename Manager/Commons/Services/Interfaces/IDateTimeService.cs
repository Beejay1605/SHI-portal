using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Services.Interfaces;

public interface IDateTimeService
{
    DateTime Now { get; }
}
