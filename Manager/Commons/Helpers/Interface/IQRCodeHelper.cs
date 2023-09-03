using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers.Interface;

public interface IQRCodeHelper
{
    Task<string> CreateQRCode(string code);

}
