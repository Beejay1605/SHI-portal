using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Wrappers.Interfaces;

public interface IPathWrapper
{
    string Combine(string path1, string path2);
    string Combine(string path1, string path2, string path3);
    string Combine(string path1, string path2, string path3, string path4);
    string GetFileNameWithoutExtension(string path);
    string GetExtension(string path);
    string GetFileName(string path);
}
