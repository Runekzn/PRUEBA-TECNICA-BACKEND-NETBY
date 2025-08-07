using DTOs.Response;
using TokenGeneration.Entidades;
using TokenGeneration.Interfaces;

namespace TokenGeneration.Context
{
    public class TokenGeneratorContext
    {
        private readonly ITokenGenerationStrategy tokenGenerationStrategy;

        public TokenGeneratorContext(ITokenGenerationStrategy tokenGenerationStrategy)
        {
            this.tokenGenerationStrategy = tokenGenerationStrategy;
        }

        public async Task<GenericResponse<GenerateTokenResponse>> GenerateToken()
        {
            return await tokenGenerationStrategy.GenerateToken();
        }

        public async Task<GenericResponse<bool>> ValidateToken(string token)
        {
            return await tokenGenerationStrategy.PersonalTokenValidation(token);
        }
    }
}
