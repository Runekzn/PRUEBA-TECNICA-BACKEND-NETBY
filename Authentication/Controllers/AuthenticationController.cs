using API_GATEWAY.Models;
using Authentication.Services;
using DTOs.Request.API_Authentication;
using DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TokenGeneration.Context;
using TokenGeneration.Entidades;
using TokenGeneration.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService _authenticationService)
        {
            this._authenticationService = _authenticationService;
        }

        // POST api/<AuthenticationController>
        [HttpPost("Login")]
        public async Task<IActionResult> Authentication([FromBody] LoginDto value)
        {
            var authenticate = await _authenticationService.AuthenticateAsync(value);
            if (!authenticate.Success)
            {
                return BadRequest(new GenericResponse<bool>() { Data = false, Success = false,
                Message = authenticate.Message});
            }

            //generacion del token
            var tokenValidator = new TokenGeneratorContext(new TokenAdrian(new CtorTokenGeneratorRequest() { Algorithm = Encrypt.Enums.EncryptAlgorithm.AES, CypherMode = Encrypt.Enums.EncryptedMode.SHA256, SecretKey = Key.key , Rol = authenticate.Data.Rol, UserId = authenticate.Data.userId}));

            var generateToken = await tokenValidator.GenerateToken(authenticate.Data.userId);

            return Ok(generateToken);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto value)
        {
            var authenticate = await _authenticationService.RegisterAsync(value);
            if (!authenticate.Success)
            {
                return BadRequest(authenticate);
            }
            return Ok(authenticate);
        }


    }
}
