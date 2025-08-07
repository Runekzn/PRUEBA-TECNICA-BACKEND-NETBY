using AccessData.Models;
using Azure.Core;
using DTOs.Request.API_Productos;
using DTOs.Response;
using DTOs.Response.API_Productos;
using Microsoft.EntityFrameworkCore;
using Productos.Services.Interfaces;
using Repository;

namespace Productos.Services.Implementation
{
    public class ProductosService : IProductosService
    {
        private readonly InventarioContext inventarioContext;
        private readonly IRepository<Product, InventarioContext> repository;
        public ProductosService(InventarioContext inventarioContext, IRepository<Product, InventarioContext> repository)
        {
            this.inventarioContext = inventarioContext;
            this.repository  = repository;
        }
        public async Task<GenericResponse<bool>> CreateAsync(ProductoRequest request, int u)
        {
            try
            {
                var producto = new Product
                {
                    ProCategoria = request.ProCategoria,
                    ProDescripcion = request.ProDescripcion,
                    ProCreatedBy = u,
                    ProImagen = request.ProImagen,
                    ProNombre = request.ProNombre,
                    ProPrecio = request.ProPrecio,
                    ProStock = request.ProStock,
                };

                var insert = await repository.InsertAsync(producto);

                if (insert.Success)
                {
                    return new GenericResponse<bool>
                    {
                        Data = true,
                        Success = true,
                        Message = "Producto creado exitosamente"
                    };
                }

                return new GenericResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Error al crear el producto : " + insert.Message
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error al crear el producto: {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var getProducto = await repository.DeleteAsync(id);

                return new GenericResponse<bool>
                {
                    Data = getProducto.Success,
                    Success = getProducto.Success,
                    Message = getProducto.Success ? "Producto eliminado exitosamente" : "Error al eliminar el producto"
                };

            }
            catch (Exception ex)
            {
                return new()
                {
                    Data = false,
                    Success = false,
                    Message = $"Error al eliminar el producto: {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<List<ProductosResponse>>> GetAllAsync()
        {
            try
            {
                var getProducto = await repository.GetAllAsync();
                
                List<ProductosResponse> list = new List<ProductosResponse>();

                foreach (var item in getProducto.Data)
                {
                    var userName = await inventarioContext.Set<User>().Where(x => x.Id == item.ProCreatedBy).Select(x => x.UserName).FirstOrDefaultAsync();
                    var data = new ProductosResponse
                    {
                        ProCategoria = item.ProCategoria,
                        ProDescripcion = item.ProDescripcion,
                        ProImagen = item.ProImagen,
                        ProNombre = item.ProNombre,
                        ProPrecio = item.ProPrecio,
                        ProStock = item.ProStock,
                        ProCreatedBy = userName
                    };
                    list.Add(data);
                }
                

                return new GenericResponse<List<ProductosResponse>>()
                {
                    Data = list,
                    Success = false,
                    Message = "Se obtivieron  los datos adecuadamente"
                };

            }
            catch (Exception ex)
            {
                return new()
                {
                    Data = null,
                    Success = false,
                    Message = $"Error al obtener el producto: {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<ProductosResponse>> GetByIdAsync(int id)
        {
            try
            {
                var getProducto = await repository.GetByIdAsync(id);
                var userName = await inventarioContext.Set<User>().Where(x => x.Id == getProducto.Data.ProCreatedBy).Select(x => x.UserName).FirstOrDefaultAsync();

                var data = new ProductosResponse
                {
                    ProCategoria = getProducto.Data.ProCategoria,
                    ProDescripcion = getProducto.Data.ProDescripcion,
                    ProImagen = getProducto.Data.ProImagen,
                    ProNombre = getProducto.Data.ProNombre,
                    ProPrecio = getProducto.Data.ProPrecio,
                    ProStock = getProducto.Data.ProStock,
                    ProCreatedBy = userName
                };

                return new GenericResponse<ProductosResponse>()
                {
                    Data = data,
                    Success = true,
                    Message = "Se obtivieron  los datos adecuadamente"
                };

            }
            catch (Exception ex)
            {
                return new ()
                {
                    Data = null,
                    Success = false,
                    Message = $"Error al obtener el producto: {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<bool>> UpdateAsync(int id, ProductoRequest request)
        {
            try
            {
                var getProducto = await repository.GetByIdAsync(id);


                getProducto.Data.ProCategoria = request.ProCategoria;
                getProducto.Data.ProDescripcion = request.ProDescripcion;
                getProducto.Data.ProImagen = request.ProImagen;
                getProducto.Data.ProNombre = request.ProNombre;
                getProducto.Data.ProPrecio = request.ProPrecio;
                getProducto.Data.ProStock = request.ProStock;

                var update = await repository.UpdateAsync(getProducto.Data);

                if (update.Success)
                {
                    return new GenericResponse<bool>
                    {
                        Data = true,
                        Success = true,
                        Message = "Producto actualilzado exitosamente"
                    };
                }

                return new GenericResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Error al actualilzar el producto"
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error al actulizar el producto: {ex.Message}"
                };
            }
        }
    }
}
