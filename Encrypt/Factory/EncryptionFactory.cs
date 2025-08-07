using Encrypt.Entities;
using Encrypt.Enums;
using Encrypt.Interface;
using Encrypt.Services.Algorithm;
using Encrypt.Services.CypherMode;

namespace Encrypt.Factory
{
    public static class EncryptionFactory
    {
        public static IEncryption CreateEncryption(EncryptorRequest encryptorRequest)
        {
            return encryptorRequest.AlgorithmHash.Algorithm switch
            {
                EncryptAlgorithm.AES => new AESService(encryptorRequest.Encryptor),
                EncryptAlgorithm.TRIPLEDES => new TripleDESService(encryptorRequest.Encryptor),
                _ => throw new ArgumentException("Unsupported encryption type")
            };
        }

        public static IHash CreateHash(EncryptorRequest encryptorRequest)
        {
            return encryptorRequest.AlgorithmHash.CypherMode switch
            {
                EncryptedMode.SHA256 => new SHA256Hash(),
                EncryptedMode.SHA384 => new SHA384Hash(),
                EncryptedMode.SHA512 => new SHA512Hash(),
                EncryptedMode.MD5 => new MD5Hash(),
                _ => throw new ArgumentException("Unsupported hash type")
            };
        }
    }
}
