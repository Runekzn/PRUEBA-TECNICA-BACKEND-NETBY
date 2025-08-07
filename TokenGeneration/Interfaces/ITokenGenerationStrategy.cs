using DTOs.Response;
using TokenGeneration.Entidades;

namespace TokenGeneration.Interfaces
{
    public interface ITokenGenerationStrategy
    {
        Task<GenericResponse<GenerateTokenResponse>> GenerateToken();

        Task<GenericResponse<bool>> PersonalTokenValidation(string Token);
    }
}
