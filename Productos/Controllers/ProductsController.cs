using DTOs.Request.API_Productos;
using DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Productos.Services.Interfaces;
using System.Security.Claims;
using TokenGeneration.Resources;

namespace Productos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductosService _productosService;
        public ProductsController(IProductosService productosService)
        {
            _productosService = productosService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            if(User.FindFirstValue(SecurityResources.Claims.CL_Type) != SecurityResources.Claims.CL_Value)
            {
                return BadRequest(new GenericResponse<bool>()
                {
                    Data = false,
                    Success = false,
                    Message = "El metodo no esta siendo invocado desde el Gateway"
                });
            }
            var idUsuario = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);//OBTENEMOS EL ID DEL USUARIO DEL TOKEN

            var productos = await _productosService.GetAllAsync();

            if (productos == null )
            {
                return NotFound(productos);
            }
            return Ok(productos);
        }

        [HttpPost]
        public async Task<IActionResult> PostProductos(ProductoRequest request)
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
            if (!User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                return Forbid("Solo los administradores pueden crear productoss"); // O lo que necesites retornar
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
            var productos = await _productosService.CreateAsync(request, Convert.ToInt32(userIdClaim));

            if (productos == null)
            {
                return NotFound(productos);
            }
            return Ok(productos);
        }
    }
}
