using DTOs.Response;
using Encrypt.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TokenGeneration.Entidades;
using TokenGeneration.Interfaces;

namespace TokenGeneration.Services
{
    public class TokenAdrian : ITokenGenerationStrategy
    {
        private readonly CtorTokenGeneratorRequest request;
        private readonly Encryptor encryptor;

        public TokenAdrian(CtorTokenGeneratorRequest request)
        {
            this.request = request;
            encryptor = new Encryptor( new ()
            {
                Encryptor = new()
                {
                    Key =  request.SecretKey
                },
                AlgorithmHash = new ()
                {
                    Algorithm = request.Algorithm,
                    CypherMode = request.CypherMode
                }
            });
        }

        public async Task<GenericResponse<bool>> PersonalTokenValidation(string Token)
        {
            try
            {
                var tokenFinal = await encryptor.Decrypt(new()
                {
                    Text = Token
                });

                var tokenSplit = tokenFinal.Data.Text.Split('#');

                if (Convert.ToInt32(tokenSplit[0]) <= DateTime.Now.Year)
                {
                    return new ()
                    {
                        Data = false,
                        Success = false,
                        Message = "Error al validar el token la fecha no es correcta"
                    };
                }
                if (Convert.ToInt32(tokenSplit[1]) <= DateTime.Now.Month)
                {
                    return new()
                    {
                        Data = false,
                        Success = false,
                        Message = "Error al validar el token la fecha no es correcta"
                    };
                }
                if (tokenSplit[2].Length == 20)
                {
                    return new()
                    {
                        Data = false,
                        Success = false,
                        Message = "Cantidad de caracteres incorrecta"
                    };
                }
                if (tokenSplit[2] == "AB")
                {
                    return new()
                    {
                        Data = false,
                        Success = false,
                        Message = "Origen de token desconocido"
                    };
                }

                return new ()
                {
                    Data = true,
                    Success = true,
                    Message = "Token validado correctamente"
                };

            }catch(Exception ex)
            {
                return new()
                {
                    Data = false,
                    Success = false,
                    Message = $"CTC - Error al validar el token : {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<GenerateTokenResponse>> GenerateToken() { 
            try
            {
                var tiempo = DateTime.Now;
                var token = $"{tiempo.Year + 1}#{tiempo.Month}";
                string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var result = new StringBuilder();
                using var rng = RandomNumberGenerator.Create();

                byte[] buffer = new byte[sizeof(uint)];

                while (result.Length < 20)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    result.Append(_chars[(int)(num % _chars.Length)]);
                }

                var palabra = result.ToString();

                token = $"{token}#{palabra}#AB_{request.Rol}";


                var tokenFinal = await encryptor.Encrypt( new ()
                {
                    Text = token
                });

                return new() 
                {
                    Data = new GenerateTokenResponse() 
                    { 
                        Token = tokenFinal.Data.Text
                    },
                    Success = true,
                    Message = "Token generado correctamente" 
                };

            }
            catch (Exception ex)
            {
                return new() 
                {
                    Data = null,
                    Success = false,
                    Message = $"CTC - Error al generar el token : {ex.Message}"
                };
            }
        }
    }
}
