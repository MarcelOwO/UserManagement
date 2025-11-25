
using System.Security.Cryptography;
using Isopoh.Cryptography.Argon2;

namespace UserManagement.Api.Utility;

public  static class PasswordHasher
{
    
    public static string HashPassword(string password)
    {
        var config = new Argon2Config()
        {
            Type = Argon2Type.DataIndependentAddressing, 
            Version = Argon2Version.Nineteen,
            TimeCost = 4, 
            MemoryCost = 65536, 
            Lanes = 4,
            Threads = Environment.ProcessorCount,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
            Salt = RandomNumberGenerator.GetBytes(16),
            HashLength = 16 
        };
        
        var argon2A = new Argon2(config);
        return argon2A.Hash().ToString();

    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return Argon2.Verify( hashedPassword,password);
    }
    
}