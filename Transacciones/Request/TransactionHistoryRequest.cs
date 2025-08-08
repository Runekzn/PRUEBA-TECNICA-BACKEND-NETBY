namespace Transacciones.Request
{
    public class TransactionHistoryRequest
    {
        public int productId { get; set; }
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
        public string? type { get; set; }        // "Compra" | "Venta"
        public int page  { get; set; }
        public int pageSize { get; set; }
        public string sort { get; set; }
    }
}
