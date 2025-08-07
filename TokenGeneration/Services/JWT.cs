using DTOs.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TokenGeneration.Entidades;
using TokenGeneration.Interfaces;
using TokenGeneration.Resources;

namespace TokenGeneration.Services
{
    public class JWT : ITokenGenerationStrategy
    {
        private readonly CtorTokenGeneratorRequest request;

        public JWT(CtorTokenGeneratorRequest request)
        {
            this.request = request;
        }

        public Task<GenericResponse<bool>> PersonalTokenValidation(string Token)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<GenerateTokenResponse>> GenerateToken()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(request.SecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] 
                    { new Claim(SecurityResources.Claims.CL_Type, SecurityResources.Claims.CL_Value)  , new Claim(ClaimTypes.NameIdentifier, request.UserId.ToString())}),// Claim para indicar que proviene del gateway
                    Expires = DateTime.UtcNow.AddMinutes(30),// Duración del token
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                if (!string.IsNullOrEmpty(request.Rol)) 
                {

                    tokenDescriptor.Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, request.Rol) });

                }
                //Creación del Token
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                //Validación del Token
                if (token == null)
                {
                    throw new ArgumentNullException(nameof(token));
                }

                return new()
                {
                    Success = true,
                    Message = string.Empty,
                    Data = new()
                    {
                        Token = token
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,

                    Data = null!,
                    Message = ex.Message
                };
            }
        }
    }
}
