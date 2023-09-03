using Manager.Commons.Wrappers.Interfaces;
using MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Wrappers;

public class MimeTypeWrapper : IMimeTypeWrapper
{
    public string GetMimeType(string input)
    {
        return MimeTypeMap.GetMimeType(input);
    }
}
