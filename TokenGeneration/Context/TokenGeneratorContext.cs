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

        public async Task<GenericResponse<GenerateTokenResponse>> GenerateToken(int id)
        {
            return await tokenGenerationStrategy.GenerateToken(id);
        }

        public async Task<GenericResponse<TokenValid>> ValidateToken(string token)
        {
            return await tokenGenerationStrategy.PersonalTokenValidation(token);
        }
    }
}
