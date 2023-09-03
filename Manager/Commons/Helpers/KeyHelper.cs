using System.Text;
using Manager.Commons.Helpers.Interface;

namespace Manager.Commons.Helpers;

public class KeyHelper : IKeyHelper
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    
    public string GenerateAlphaNumeric(int length)
    {
        var random = new Random();
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int randomIndex = random.Next(Characters.Length);
            char randomChar = Characters[randomIndex];
            stringBuilder.Append(randomChar);
        }

        return stringBuilder.ToString();
    }
}