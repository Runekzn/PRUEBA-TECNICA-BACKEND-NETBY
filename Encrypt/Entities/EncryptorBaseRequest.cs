using System.Security.Cryptography;

namespace Encrypt.Entities
{
    public class EncryptorBaseRequest
    {

        public string Key { get; set; }
        public byte[] HashedKey { get; set; }
        public SymmetricAlgorithm SymmetricAlgorithm { get; set; }
    }
}
