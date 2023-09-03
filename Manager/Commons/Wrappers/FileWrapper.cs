using Manager.Commons.Wrappers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Wrappers;

public class FileWrapper : IFileWrapper
{
    public void CreateDirectory(string input)
    {
        Directory.CreateDirectory(input);
    }

    public bool Exists(string input)
    {
        return File.Exists(input);
    }
}
