using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers.Interface;

public interface IUrlHelper
{
    string GenerateUrl();
    string GenerateUrl(string path);
    string GenerateUrl(string apiPath, string path, string query);
    string GenerateConfirmationEmailUrl(int userId, string code);
}
