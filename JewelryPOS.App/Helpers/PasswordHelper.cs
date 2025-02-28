using System.Security.Cryptography;
using System.Text;

namespace JewelryPOS.App.Helpers
{
    public static class PasswordHelper
    {
        public static (string passwordHash, string passwordSalt) HashPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                var salt = hmac.Key;
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
            }
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                hmac.Key = Convert.FromBase64String(storedSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return storedHash == Convert.ToBase64String(computedHash);
            }
        }
    }
}
