using DTOs.Response;
using Transacciones.Request;
using Transacciones.Response;

namespace Transacciones.Services
{
    public interface ITransactionService
    {
        Task<GenericResponse<bool>> PostCompraAsync(TransactionRequest request);
        Task<GenericResponse<bool>> PostVentaAsync(TransactionRequest request);
        Task<GenericResponse<TransactionHistoryResponse>> GetAllTrasactions(TransactionHistoryRequest request);
    }
}
