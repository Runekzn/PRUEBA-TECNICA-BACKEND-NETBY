using AccessData.Models;
using DTOs.Request.API_Authentication;
using DTOs.Response;
using DTOs.Response.API_Autentication;

namespace Authentication.Services
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<User>> RegisterAsync(RegisterDto request);
        Task<GenericResponse<Auth>> AuthenticateAsync(LoginDto request);
    }
}
