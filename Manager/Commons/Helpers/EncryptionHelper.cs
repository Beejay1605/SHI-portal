using Manager.Commons.Helpers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Helpers;

public class EncryptionHelper : IEncryptionHelper
{
    public string DecodeURL(string parameterToDecode)
    {
        var valueBytes = System.Convert.FromBase64String(parameterToDecode);
        string stringToDecrypt = Encoding.UTF8.GetString(valueBytes);
        return Decrypt(stringToDecrypt);
    }

    public string Decrypt(string string_to_decrypt)
    {
        if (!String.IsNullOrWhiteSpace(string_to_decrypt))
        {
            string removedSaltString = Salt(string_to_decrypt, false);
            var fullCipher = Convert.FromBase64String(removedSaltString);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length]; //changes here

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            // Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length); // changes here
            var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }

            return result;
        }

        return null;
    }

    public string EncodeURL(string parameterToEncode)
    {
        string encryptedParameter = Encrypt(parameterToEncode);
        var valueBytes = Encoding.UTF8.GetBytes(encryptedParameter);
        return Convert.ToBase64String(valueBytes);
    }

    public string Encrypt(string string_to_encrypt)
    {
        var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

        using var aesAlg = Aes.Create();
        using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(string_to_encrypt);
        }

        var iv = aesAlg.IV;

        var decryptedContent = msEncrypt.ToArray();

        var result = new byte[iv.Length + decryptedContent.Length];

        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

        string saltedEncrytion = Salt(Convert.ToBase64String(result), true);

        return saltedEncrytion;
    }


    public string DecodeBase64String(string string_to_decode)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(string_to_decode);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public string EncodeStringToBase64(string string_to_encode)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(string_to_encode);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Adds or Removes the salt in the encrypted string.
    /// THIS SHOULD NEVER CHANGE
    /// </summary>
    /// <param name="stringToChange"></param>
    /// <param name="IsEncrypt"></param>
    /// <returns></returns>
    private  string Salt(string stringToChange, bool IsEncrypt)
    {
        string toAdd = $"{System.DateTime.Now.Day.ToString("00")}{System.DateTime.Now.Month.ToString("00")}";
        if (IsEncrypt)
            return _ = stringToChange.Insert(stringToChange.Length - 2, toAdd);
        else
            return _ = stringToChange.Remove(stringToChange.Length - 6, 4);
    }
}
