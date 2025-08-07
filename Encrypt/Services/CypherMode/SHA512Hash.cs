using Encrypt.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Encrypt.Services.CypherMode
{
    public class SHA512Hash : IHash
    {
        public byte[] HashKey(string key)
        {
            return SHA512.HashData(Encoding.UTF8.GetBytes(key));
        }
    }
}
