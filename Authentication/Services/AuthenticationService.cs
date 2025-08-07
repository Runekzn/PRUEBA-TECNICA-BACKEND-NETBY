using AccessData.Models;
using API_GATEWAY.Models;
using DTOs.Request.API_Authentication;
using DTOs.Response;
using DTOs.Response.API_Autentication;
using Microsoft.EntityFrameworkCore;
using TokenGeneration.Context;
using TokenGeneration.Entidades;
using TokenGeneration.Services;

namespace Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly InventarioContext _context;

        private readonly TokenGeneratorContext _tokenGeneratorContext;
        public AuthenticationService(InventarioContext contexto)
        {
            _context = contexto;
            _tokenGeneratorContext = new TokenGeneratorContext(new TokenAdrian(new CtorTokenGeneratorRequest() { Algorithm = Encrypt.Enums.EncryptAlgorithm.AES, CypherMode = Encrypt.Enums.EncryptedMode.SHA256, SecretKey = Key.key }));
        }
        public async Task<GenericResponse<Auth>> AuthenticateAsync(LoginDto request)
        {
            var userExist = await _context.Set<User>().Where(x => x.UserEmail.Equals(request.Email)).FirstOrDefaultAsync();

            if (userExist == null )
                return new()
                {
                    Data = null,
                    Success = false,
                    Message = $"El correo con el que intentas ingresar no esta registrado en este aplicativo por favor registrate para continuar"
                };


            var userRoles = await _context.Set<UserRole>().Where(x => x.UserId == userExist.Id).ToListAsync();

            if (!userRoles.Any())
            {
                return new()
                {
                    Data = null,
                    Success = false,
                    Message = $"El usuario no posee rol asignado"
                };
            }
           
            var nombreRol =  await _context.Set<Role>().Where(x => x.Id== userRoles[0].RoleId).FirstOrDefaultAsync();


            using var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(userExist.PasswordSalt));
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));

            if (Convert.ToBase64String(computedHash) != userExist.PasswordHash)
                return new()
                {
                    Data = null,
                    Success = false,
                    Message ="La contraseña no es correcta"
                };


            return new()
            {
                Data = new Auth()
                {
                    Rol = nombreRol.RolName
                },
                Success = true,
                Message = "Autenticacion correcta"
            };
        }

        public async Task<GenericResponse<User>> RegisterAsync(RegisterDto request)
        {
            try
            {
                var user = new User
                {
                    UserEmail = request.Email,
                    CreatedAt = DateTime.UtcNow,
                    UserName = request.Username,
                    Status = true
                };

                var filter = _context.Set<User>().Select(x => x.UserEmail.Equals(request.Email)).FirstOrDefault();

                if (filter)
                {
                    return new()
                    {
                        Data = null,
                        Success = false,
                        Message = "Ya existe una cuenta con ese correo, ingresa con un correo nuevo "
                    };
                }

                using var hmac = new System.Security.Cryptography.HMACSHA512();
                user.PasswordSalt = Convert.ToBase64String(hmac.Key);
                user.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password)));
                
                _context.Set<User>().Add(user);
                _context.SaveChanges();

                var getRolId = await _context.Set<Role>().Where(x => x.RolName.Equals(request.Rol)).FirstOrDefaultAsync();

                if (getRolId != null) 
                {
                    var roleUser = new UserRole
                    {
                        RoleId = getRolId.Id,
                        Status = true,
                        UserId = user.Id
                    };

                    _context.Set<UserRole>().Add(roleUser);
                    _context.SaveChanges();
                }

                return new()
                {
                    Data = user,
                    Success = true,
                    Message = "Usuario registrado correctamente en la aplicacion"
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Data = null,
                    Success = false,
                    Message = $"No se pudo registrar el usuario : {ex.Message}"
                };
            }
        }
    }
}
