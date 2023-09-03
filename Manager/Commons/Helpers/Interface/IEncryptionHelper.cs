using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers.Interface;

public interface IEncryptionHelper
{
    string Decrypt(string string_to_decrypt);
    string Encrypt(string string_to_encrypt);
    string DecodeBase64String(string string_to_decode);
    string EncodeStringToBase64(string string_to_encode); 
}
