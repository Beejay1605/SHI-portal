using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers;

public static class EncoderHelper
{
    public static string WebEncodersBase64UrlEncode(string input)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(input));

    public static string DefaultEncoder(string input)
        => HtmlEncoder.Default.Encode(input);
}
