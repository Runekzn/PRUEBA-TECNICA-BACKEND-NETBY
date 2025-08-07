using Encrypt.Entities;
using System.Security.Cryptography;


namespace Encrypt.Services.Algorithm
{
    public class TripleDESService : EncryptionBase
    {
        public TripleDESService(EncryptorBaseRequest encryptorBaseRequest) : base(encryptorBaseRequest)
        {
            encryptorBaseRequest.SymmetricAlgorithm = TripleDES.Create();
        }

        protected override void ConfigureAlgorithm()
        {
            encryptorBaseRequest.SymmetricAlgorithm.Mode = CipherMode.ECB;
            encryptorBaseRequest.SymmetricAlgorithm.Padding = PaddingMode.PKCS7;
            encryptorBaseRequest.SymmetricAlgorithm.IV = new byte[8];
        }
    }
}
