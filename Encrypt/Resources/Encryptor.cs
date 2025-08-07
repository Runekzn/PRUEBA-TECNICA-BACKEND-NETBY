using Encrypt.Factory;
using Encrypt.Entities;
using Encrypt.Enums;
using Encrypt.Interface;
using DTOs.Response;

namespace Encrypt.Resources
{
    public class Encryptor
    {
        private readonly IEncryption encryption;
        private readonly IHash hash;
        private readonly EncryptorRequest request;

        public Encryptor(EncryptorRequest request)
        {
            this.request = request;

            if (string.IsNullOrEmpty(request.Encryptor.Key))
            {
                throw new InvalidOperationException("Key cannot be empty.");
            }
            else if (request.AlgorithmHash.Algorithm == EncryptAlgorithm.TRIPLEDES && request.AlgorithmHash.CypherMode != EncryptedMode.MD5)
            {
                throw new InvalidOperationException("Triple DES only supports MD5 as the hash algorithm.");
            }

            hash = EncryptionFactory.CreateHash(request);
            request.Encryptor.HashedKey = hash.HashKey(request.Encryptor.Key);

            encryption = EncryptionFactory.CreateEncryption(request);
        }

        public async Task<GenericResponse<EncryptorResponse>> Encrypt(EncriptionRequest request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                throw new InvalidOperationException("Text cannot be empty.");
            }

            var encryptedText = await encryption.Encrypt(request);
            return encryptedText;
        }

        public async Task<GenericResponse<EncryptorResponse>> Decrypt(EncriptionRequest request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                throw new InvalidOperationException("Text cannot be empty.");
            }

            var decryptedText = await encryption.Decrypt(request);
            return decryptedText;
        }

        public async Task<GenericResponse<bool>> IsEncrypted(EncriptionRequest request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                throw new InvalidOperationException("Text cannot be empty.");
            }

            var textIsEncrypted = await encryption.IsEncrypted(request);
            return textIsEncrypted;
        }
    }
}
