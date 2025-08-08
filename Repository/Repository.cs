using DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class Repository<TEntity, TContext> : IRepository<TEntity, TContext> where TEntity : class
    where TContext : DbContext
    {
        private readonly TContext dbContext;
        private readonly DbSet<TEntity> dbSet;

        public Repository(TContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<TEntity>();
        }
        public async Task<GenericResponse<bool>> DeleteAsync(int id)
        {
            var response = new GenericResponse<bool>();

            try
            {
                var entity = await dbSet.FindAsync(id);

                if (entity != null)
                {
                    var statusProperty = entity.GetType().GetProperty("Status");
                    if (statusProperty?.PropertyType == typeof(bool))
                    {
                        statusProperty.SetValue(entity, false);
                        dbContext.Entry(entity).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();

                        response.Data = true;
                        response.Message = "OK";
                        response.Success = true;
                    }
                    else
                    {
                        response.Data = false;
                        response.Message = "Error al eliminar";
                        response.Success = false;
                    }
                }
                else
                {
                    response.Data = false;
                    response.Message = "Error al eliminar";
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.Message = $"Error al eliminar : {ex.Message}";
                response.Success = false;
            }

            return response;
        }

        public async Task<GenericResponse<List<TEntity>>> GetAllAsync()
        {
            try
            {
                var data = await dbSet.ToListAsync();

                return new()
                {
                    Success = true,
                    Message = "OK",
                    Data = data.ToList()
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = $"Error al obtener : {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<TEntity>> GetByIdAsync(int id)
        {
            var response = new GenericResponse<TEntity>();

            try
            {
                // Encontrar la entidad usando el ID dinámicamente
                var entity = await dbSet.FindAsync(id);

                if (entity != null)
                {
                    // Verificar si Status es true, si la propiedad existe
                    var statusProperty = entity.GetType().GetProperty("Status");
                    bool isActive = statusProperty == null || (bool)statusProperty.GetValue(entity);

                    if (isActive)
                    {
                        response.Data = entity;
                        response.Message = "OK";
                        response.Success = true;
                    }
                    else
                    {
                        response.Data = null;
                        response.Message = "Error al obtener registro";
                        response.Success = false;
                    }
                }
                else
                {
                    response.Data = null;
                    response.Message = "Error al obtener registro";
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = $"Error al eliminar : {ex.Message}";
                response.Success = false;
            }

            return response;
        }

        public async Task<GenericResponse<bool>> InsertAsync(TEntity data)
        {
            try
            {
                await dbSet.AddAsync(data);
                await dbContext.SaveChangesAsync();
                return new()
                {
                    Success = true,
                    Message = "OK",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = $"Error al insertar : {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GenericResponse<bool>> InsertManyAsync(List<TEntity> data)
        {
            try
            {
                await dbSet.AddRangeAsync(data);
                await dbContext.SaveChangesAsync();

                return new()
                {
                    Success = true,
                    Message = "OK",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = $"Error al insertar : {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GenericResponse<bool>> UpdateAsync(TEntity data)
        {
            try
            {
                await Task.Run(() =>
                {
                    dbSet.Attach(data);
                    dbContext.Entry(data).State = EntityState.Modified;
                });
                await dbContext.SaveChangesAsync();

                return new()
                {
                    Success = true,
                    Message = "OK",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = $"Error al actualizar : {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GenericResponse<bool>> UpdateManyAsync(List<TEntity> dataList)
        {
            try
            {
                await Task.Run(() =>
                {
                    foreach (var data in dataList)
                    {
                        dbSet.Attach(data);
                        dbContext.Entry(data).State = EntityState.Modified;
                    }
                });

                await dbContext.SaveChangesAsync();


                return new()
                {
                    Success = true,
                    Message = "OK",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    Message = $"Error al actualizar : {ex.Message}",
                    Data = false,
                };
            }
        }
    }
}
