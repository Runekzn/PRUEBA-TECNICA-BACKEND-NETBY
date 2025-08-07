using Encrypt.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Encrypt.Services.CypherMode
{
    public class MD5Hash : IHash
    {
        public byte[] HashKey(string key)
        {
            return MD5.HashData(Encoding.UTF8.GetBytes(key));
        }
    }
}
