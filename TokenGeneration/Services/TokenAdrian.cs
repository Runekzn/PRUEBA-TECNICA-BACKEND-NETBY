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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<GenericResponse<TokenValid>> PersonalTokenValidation(string Token)
        {
            try
            {
                var tokenFinal = await encryptor.Decrypt(new()
                {
                    Text = Token
                });

                var tokenSplit = tokenFinal.Data.Text.Split('#');//2026#9#RJZYGNAAKxJuNza6cXIE#AB_Admin

                if (Convert.ToInt32(tokenSplit[0]) <= DateTime.Now.Year)
                {
                    return new ()
                    {
                        Data = new TokenValid()
                        {
                            IsValid = false,
                            Rol = string.Empty,
                        },
                        Success = false,
                        Message = "Error al validar el token la fecha no es correcta"
                    };
                }
                if (Convert.ToInt32(tokenSplit[1]) <= DateTime.Now.Month)
                {
                    return new()
                    {
                        Data = new TokenValid()
                        {
                            IsValid = false,
                            Rol = string.Empty,
                        },
                        Success = false,
                        Message = "Error al validar el token la fecha no es correcta"
                    };
                }
                var random = Convert.ToString(tokenSplit[2]);
                if (random.Length != 20)//"erA419757rUC8sLF5qJh
                {
                    return new()
                    {
                        Data = new TokenValid()
                        {
                            IsValid = false,
                            Rol = string.Empty,
                        },
                        Success = false,
                        Message = "Cantidad de caracteres incorrecta"
                    };
                }
                if (tokenSplit[2] == "AB_")
                {
                    return new()
                    {
                        Data = new TokenValid()
                        {
                            IsValid = false,
                            Rol = string.Empty,
                        },
                        Success = false,
                        Message = "Origen de token desconocido"
                    };
                }

                if (tokenSplit[3] == "ID_")
                {
                    return new()
                    {
                        Data = new TokenValid()
                        {
                            IsValid = false,
                            Rol = string.Empty,
                        },
                        Success = false,
                        Message = "Origen de token desconocido"
                    };
                }

                var rol = tokenSplit[3].Split("_"); 

                return new ()
                {
                    Data = new TokenValid()
                    {
                        IsValid = true,
                        Rol = rol[1],
                        UserId = Convert.ToInt32(tokenSplit[4].Split("_")[1])
                    },
                    Success = true,
                    Message = "Token validado correctamente"
                };

            }catch(Exception ex)
            {
                return new()
                {
                    Data = new TokenValid()
                    {
                        IsValid = false,
                        Rol = string.Empty,
                    },
                    Success = false,
                    Message = $"CTC - Error al validar el token : {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<GenerateTokenResponse>> GenerateToken(int id) { 
            try
            {
                var tiempo = DateTime.Now;
                var token = $"{tiempo.Year + 1}#{tiempo.Month + 1}";
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

                token = $"{token}#{palabra}#AB_{request.Rol}#ID_{id}";//2026#8#7aYZymjE5MrA4yr8zhY6#AB_Admin


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
