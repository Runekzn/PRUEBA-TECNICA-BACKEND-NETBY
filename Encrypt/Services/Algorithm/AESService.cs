using Encrypt.Entities;
using System.Security.Cryptography;


namespace Encrypt.Services.Algorithm
{
    public class AESService : EncryptionBase
    {
        public AESService(EncryptorBaseRequest encryptorBaseRequest) : base(encryptorBaseRequest)
        {
            encryptorBaseRequest.SymmetricAlgorithm = Aes.Create();
        }

        protected override void ConfigureAlgorithm()
        {
            encryptorBaseRequest.SymmetricAlgorithm.Mode = CipherMode.CBC;
            encryptorBaseRequest.SymmetricAlgorithm.Padding = PaddingMode.PKCS7;
            encryptorBaseRequest.SymmetricAlgorithm.IV = new byte[16]; // IV específico para AES
        }
    }
}
