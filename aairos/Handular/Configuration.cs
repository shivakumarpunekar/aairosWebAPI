using System;
using System.Security.Cryptography;



namespace aairos.Handular
{
    public class Configuration
    {
        public static string GenerateSecretKey(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                char[] chars = new char[length];
                for (int i = 0; i < length; i++)
                {
                    chars[i] = validChars[bytes[i] % validChars.Length];
                }
                return new string(chars);
            }
        }
    }
}
