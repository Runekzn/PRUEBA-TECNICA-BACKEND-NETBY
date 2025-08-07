using DTOs.Request.API_Productos;
using DTOs.Response;
using DTOs.Response.API_Productos;

namespace Productos.Services.Interfaces
{
    public interface IProductosService
    {
        Task<GenericResponse<List<ProductosResponse>>> GetAllAsync();
        Task<GenericResponse<ProductosResponse>> GetByIdAsync(int id);
        Task<GenericResponse<bool>> CreateAsync(ProductoRequest request, int userId);
        Task<GenericResponse<bool>> UpdateAsync(int id, ProductoRequest request);
        Task<GenericResponse<bool>> DeleteAsync(int id);
    }
}
