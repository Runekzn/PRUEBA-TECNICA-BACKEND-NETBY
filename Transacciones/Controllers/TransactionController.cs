using Azure.Core;
using DTOs.Request.API_Productos;
using DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TokenGeneration.Resources;
using Transacciones.Request;
using Transacciones.Services;

namespace Transacciones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }
        [HttpPost("compra")]
        public async Task<IActionResult> PostCompra(TransactionRequest request)
        {
            if (User.FindFirstValue(SecurityResources.Claims.CL_Type) != SecurityResources.Claims.CL_Value)
            {
                return BadRequest(new GenericResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Message = "El metodo no esta siendo invocado desde el Gateway"
                });
            }

            var idUsuario = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);//OBTENEMOS EL ID DEL USUARIO DEL TOKEN

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim.Equals("0"))
            {
                return BadRequest(new GenericResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Message = "El usuario no tiene un ID valido"
                });
            }
            var productos = await transactionService.PostCompraAsync(request);

            if (productos == null)
            {
                return NotFound(productos);
            }
            return Ok(productos);
        }
        [HttpPost("venta")]
        public async Task<IActionResult> PostVenta(TransactionRequest request)
        {
            if (User.FindFirstValue(SecurityResources.Claims.CL_Type) != SecurityResources.Claims.CL_Value)
            {
                return BadRequest(new GenericResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Message = "El metodo no esta siendo invocado desde el Gateway"
                });
            }

            var idUsuario = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);//OBTENEMOS EL ID DEL USUARIO DEL TOKEN

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim.Equals("0"))
            {
                return BadRequest(new GenericResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Message = "El usuario no tiene un ID valido"
                });
            }
            var productos = await transactionService.PostVentaAsync(request);

            if (productos == null)
            {
                return NotFound(productos);
            }
            return Ok(productos);
        }
        [HttpGet("FiltroVentas")]
        public async Task<IActionResult> GetFilterProducts(TransactionHistoryRequest req)
        {
            if (User.FindFirstValue(SecurityResources.Claims.CL_Type) != SecurityResources.Claims.CL_Value)
            {
                return BadRequest(new GenericResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Message = "El metodo no esta siendo invocado desde el Gateway"
                });
            }
            var idUsuario = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);//OBTENEMOS EL ID DEL USUARIO DEL TOKEN

            var productos = await transactionService.GetAllTrasactions(req);

            if (productos == null)
            {
                return NotFound(productos);
            }
            return Ok(productos);
        }
    }
}
