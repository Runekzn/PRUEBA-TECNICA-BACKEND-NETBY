using Encrypt.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Encrypt.Services.CypherMode
{
    public class SHA384Hash : IHash
    {
        public byte[] HashKey(string key)
        {
            var sha384Key = SHA384.HashData(Encoding.UTF8.GetBytes(key));
            var sha384compatibleToAES32 = DeriveAesKeyFromSha384(sha384Key,32, key);
            return sha384compatibleToAES32;
        }

        private byte[] DeriveAesKeyFromSha384(byte[] sha384Hash, int keySizeInBytes,string keySalt)
        {
            using var hkdf = new HMACSHA384(sha384Hash); // Utiliza el hash como clave HMAC
            var key = new byte[keySizeInBytes];
            var salt = Encoding.UTF8.GetBytes(keySalt); // Salt opcional para mayor seguridad
            using var rfc2898 = new Rfc2898DeriveBytes(sha384Hash, salt, iterations: 1000, HashAlgorithmName.SHA384);
            return rfc2898.GetBytes(keySizeInBytes);
        }
    }
}
