using DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public interface IRepository<TEntity, TContext> where TEntity : class
    where TContext : DbContext
    {
        Task<GenericResponse<List<TEntity>>> GetAllAsync();
        Task<GenericResponse<TEntity>> GetByIdAsync(int id);
        Task<GenericResponse<bool>> InsertAsync(TEntity data);
        Task<GenericResponse<bool>> UpdateAsync(TEntity data);
        Task<GenericResponse<bool>> DeleteAsync(int id);
        Task<GenericResponse<bool>> InsertManyAsync(List<TEntity> data);
        Task<GenericResponse<bool>> UpdateManyAsync(List<TEntity> dataList);
        Task<GenericResponse<bool>> DeleteManyAsync(List<TEntity> data);
    }
}
