using DTOs.Response;
using Encrypt.Entities;
using Encrypt.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Encrypt.Services
{
    public abstract class EncryptionBase : IEncryption
    {
        protected EncryptorBaseRequest encryptorBaseRequest;

        protected EncryptionBase(EncryptorBaseRequest encryptorBaseRequest)
        {
            this.encryptorBaseRequest = encryptorBaseRequest;
        }

        public async Task<GenericResponse<EncryptorResponse>> Encrypt(EncriptionRequest request)
        {
            try
            {
                encryptorBaseRequest.SymmetricAlgorithm.Key = encryptorBaseRequest.HashedKey;
                ConfigureAlgorithm(); // Configuración específica del algoritmo
                using MemoryStream msEncrypt = new();
                using (ICryptoTransform encryptor = encryptorBaseRequest.SymmetricAlgorithm.CreateEncryptor(encryptorBaseRequest.SymmetricAlgorithm.Key, encryptorBaseRequest.SymmetricAlgorithm.IV))
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(request.Text);
                    await csEncrypt.WriteAsync(inputBytes);
                    await csEncrypt.FlushFinalBlockAsync();
                }
                string encryptedText = Convert.ToBase64String(msEncrypt.ToArray());
                return new()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = new()
                    {
                        Text = encryptedText
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = string.Empty,
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<EncryptorResponse>> Decrypt(EncriptionRequest request)
        {
            try
            {
                encryptorBaseRequest.SymmetricAlgorithm.Key = encryptorBaseRequest.HashedKey;
                ConfigureAlgorithm(); // Configuración específica del algoritmo
                using MemoryStream msDecrypt = new(Convert.FromBase64String(request.Text));
                using ICryptoTransform decryptor = encryptorBaseRequest.SymmetricAlgorithm.CreateDecryptor(encryptorBaseRequest.SymmetricAlgorithm.Key, encryptorBaseRequest.SymmetricAlgorithm.IV);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt, Encoding.UTF8);
                string decryptedText = (await srDecrypt.ReadToEndAsync()).Trim();
                return new()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = new()
                    {
                        Text = decryptedText
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = string.Empty,
                    Data = null
                };
            }
        }

        // Método abstracto para configuraciones específicas
        protected abstract void ConfigureAlgorithm();

        public async Task<GenericResponse<bool>> IsEncrypted(EncriptionRequest request)
        {
            try
            {
                var decryptText = await Decrypt(request);
                bool IsEncrypted = decryptText.Success;

                return new()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = IsEncrypted
                };

            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = string.Empty,
                    Data = false
                };
            }
        }
    }
}
