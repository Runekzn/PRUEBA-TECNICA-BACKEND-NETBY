using DTOs.Response;
using TokenGeneration.Entidades;

namespace TokenGeneration.Interfaces
{
    public interface ITokenGenerationStrategy
    {
        Task<GenericResponse<GenerateTokenResponse>> GenerateToken(int id);

        Task<GenericResponse<TokenValid>> PersonalTokenValidation(string Token);
    }
}
