namespace Encrypt.Entities
{
    public class EncryptorRequest
    {
        public AlgorithmHashRequest AlgorithmHash { get; set; }
        public EncryptorBaseRequest Encryptor { get; set; }

    }
}