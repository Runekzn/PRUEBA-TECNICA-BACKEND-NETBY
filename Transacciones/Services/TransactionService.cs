using AccessData.Models;
using DTOs.Response;
using Microsoft.EntityFrameworkCore;
using Repository;
using Transacciones.Request;
using Transacciones.Response;

namespace Transacciones.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction, InventarioContext> repository;
        private readonly IRepository<Product, InventarioContext> productRepository;
        private readonly InventarioContext inventarioContext;
        public TransactionService(IRepository<Transaction, InventarioContext> repository, InventarioContext inventarioContext, IRepository<Product, InventarioContext> productRepository)
        {
            this.repository = repository;
            this.inventarioContext = inventarioContext;
            this.productRepository = productRepository;
        }

        public async Task<GenericResponse<TransactionHistoryResponse>> GetAllTrasactions(TransactionHistoryRequest request)
        {
            try
            {
                // Normaliza parámetros
                var page = request.page <= 0 ? 1 : request.page;
                var pageSize = (request.pageSize <= 0 || request.pageSize > 200) ? 20 : request.pageSize;
                var sort = string.IsNullOrWhiteSpace(request.sort) ? "dateDesc" : request.sort;

                // Base query: por producto y activos
                var q = new List<Transaction>();
                if(request.productId != 0)
                {
                    q = await (inventarioContext.Transactions.AsNoTracking()
                    .Where(
                    t => t.TraProducto == request.productId //busco el producto
                    && t.TraFechaRealizada >= request.from //valido la fecha desde
                    && t.TraFechaRealizada <= request.to //valido la fecha hasta
                    && t.TraTipoTransaccion == request.type //valido el tipo de transacción (Compra o Venta)
                    )).ToListAsync();
                }
                else
                {
                    q = await (inventarioContext.Transactions.AsNoTracking()
                    .Where(
                    t => t.TraFechaRealizada >= request.from //valido la fecha desde
                    && t.TraFechaRealizada <= request.to //valido la fecha hasta
                    && t.TraTipoTransaccion == request.type //valido el tipo de transacción (Compra o Venta)
                    )).ToListAsync();

                }



                // Total para paginación
                var total = q.Count();

                // Page
                var items = (q
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TransactionHistoryItem
                    {
                        TransactionId = t.Id,
                        Date = t.TraFechaRealizada,
                        Type = t.TraTipoTransaccion,
                        Quantity = t.TraTipoTransaccion == "Venta" ? -t.TraCantidad : t.TraCantidad,
                        UnitPrice = t.TraPrecioUnitario,
                        TotalPrice = t.TraPrecioTotal,
                        Description = t.TraDescripcion,
                        UserExecutedId = t.TraUserExecuted
                    }))
                    .ToList();

                // Producto + stock actual
                var product = await inventarioContext.Products.AsNoTracking()
                    .Where(p => p.Id == request.productId)
                    .Select(p => new
                    {
                        p.Id,
                        Name = EF.Property<string>(p, "ProNombre"), 
                        Unit = EF.Property<int?>(p, "ProStock") 
                    })
                    .FirstOrDefaultAsync();

                if (product is null)
                    throw new KeyNotFoundException("El producto no existe.");

                var currentStock = await inventarioContext.Transactions.AsNoTracking()
                    .Where(t => t.Status && t.TraProducto == request.productId)
                    .SumAsync(t => (int?)(t.TraTipoTransaccion == "Venta" ? -t.TraCantidad : t.TraCantidad)) ?? 0;

                return new()
                {
                    Data =
                        new TransactionHistoryResponse
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            CurrentStock = currentStock,
                            Filters = new TransactionHistoryFilters
                            {
                                From = request.from,
                                To = request.to,
                                Type = request.type,
                                Sort = sort
                            },
                            Total = total,
                            Page = page,
                            PageSize = pageSize,
                            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                            Items = items
                        }
                    ,
                    Success = true,
                    Message = "Se obtuvo correctamente la data"
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<TransactionHistoryResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "CTC: " + ex.Message
                }; 
            }
        }

        public async Task<GenericResponse<bool>> PostCompraAsync(TransactionRequest request)
        {
            try
            {
                List<Transaction> transactions = new List<Transaction>();
                var fecha = DateTime.UtcNow;

                foreach (var item in request.Productos)
                {
                    var data = await inventarioContext.Set<Product>().Where(x => x.Id == item.ProId).FirstOrDefaultAsync();
                    if (data == null)
                    {
                        return new()
                        {
                            Data = false,
                            Success = false,
                            Message = "El producto no existe"
                        };
                    }
                    var transaction = new Transaction
                    {
                        TraCantidad = item.Cantidad,
                        TraDescripcion = request.Descripcion,
                        TraFechaRealizada = fecha,
                        TraTipoTransaccion = "Compra",
                        TraUserExecuted = request.UserId,
                        TraProducto = item.ProId,
                        TraPrecioTotal = data.ProPrecio * item.Cantidad,
                        TraPrecioUnitario = data.ProPrecio,
                    };
                    transactions.Add(transaction);
                }

                var insertResult = await repository.InsertManyAsync(transactions);

                foreach (var item in request.Productos)
                {
                    var data = await inventarioContext.Set<Product>().Where(x => x.Id == item.ProId).FirstOrDefaultAsync();
                    data.ProStock  = data.ProStock - item.Cantidad;

                    var updateResult = await productRepository.UpdateAsync(data);
                    if (!updateResult.Success)
                    {
                        return new GenericResponse<bool>
                        {
                            Data = false,
                            Success = false,
                            Message = "Error al actualizar el stock del producto"
                        };
                    }
                }

                if (!insertResult.Success)
                {
                    return new GenericResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Error al insertar las transacciones de compra"
                    };
                }

                return new GenericResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Compra procesada exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error al procesar la compra: {ex.Message}"
                };
            }
        }

        public async Task<GenericResponse<bool>> PostVentaAsync(TransactionRequest request)
        {
            try
            {
                List<Transaction> transactions = new List<Transaction>();
                var fecha = DateTime.UtcNow;

                foreach (var item in request.Productos)
                {
                    var data = await inventarioContext.Set<Product>().Where(x => x.Id == item.ProId).FirstOrDefaultAsync();
                    if (data == null)
                    {
                        return new()
                        {
                            Data = false,
                            Success = false,
                            Message = "El producto no existe"
                        };
                    }
                    var transaction = new Transaction
                    {
                        TraCantidad = item.Cantidad,
                        TraDescripcion = request.Descripcion,
                        TraFechaRealizada = fecha,
                        TraTipoTransaccion = "Venta",
                        TraUserExecuted = request.UserId,
                        TraProducto = item.ProId,
                        TraPrecioTotal = data.ProPrecio * item.Cantidad,
                        TraPrecioUnitario = data.ProPrecio,
                    };
                    transactions.Add(transaction);
                }

                var insertResult = await repository.InsertManyAsync(transactions);

                foreach (var item in request.Productos)
                {
                    var data = await inventarioContext.Set<Product>().Where(x => x.Id == item.ProId).FirstOrDefaultAsync();
                    data.ProStock = data.ProStock - item.Cantidad;

                    var updateResult = await productRepository.UpdateAsync(data);
                    if (!updateResult.Success)
                    {
                        return new GenericResponse<bool>
                        {
                            Data = false,
                            Success = false,
                            Message = "Error al actualizar el stock del producto"
                        };
                    }
                }

                if (!insertResult.Success)
                {
                    return new GenericResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Error al insertar las transacciones de compra"
                    };
                }

                return new GenericResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Venta procesada exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = $"Error al procesar la venta: {ex.Message}"
                };
            }
        }
    }
}
