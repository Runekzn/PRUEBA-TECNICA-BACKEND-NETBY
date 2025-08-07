using DTOs.Response;
using Encrypt.Entities;

namespace Encrypt.Interface
{
    public interface IEncryption 
    {
        Task<GenericResponse<EncryptorResponse>> Encrypt(EncriptionRequest request);
        Task<GenericResponse<EncryptorResponse>> Decrypt(EncriptionRequest request);
        Task<GenericResponse<bool>> IsEncrypted(EncriptionRequest request);

    }
}
