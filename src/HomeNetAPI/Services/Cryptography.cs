using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace HomeNetAPI.Services
{
    //Source: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing 
    public class Cryptography : ICryptography
    {
        public string GenerateHashedString(byte [] passwordSalt, string unHashed)
        {
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: unHashed, salt: passwordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
            return hashedPassword;
        }

        public byte[] GenerateSalt()
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(passwordSalt);
            }
            return passwordSalt;
        }
    }
}
