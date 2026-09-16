using System.Security.Cryptography;
using System.Text;

namespace Obsidian.Infrastructure.Security
{
    public static class SecurityService
    {
        public static string GenerateToken(int sizeInBytes)
        {
            return Convert.ToHexString(
               RandomNumberGenerator.GetBytes(sizeInBytes)
            );
        }

        public static string HashToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Argument 'token' cannot be empty.", nameof(token));

            return Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(token)
                )
            );
        }

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Argument 'password' cannot be empty.", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Argument 'password' and/or 'passwordHash' cannot be empty.");

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
