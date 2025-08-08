namespace Transacciones.Response
{
    public class TransactionHistoryResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int CurrentStock { get; set; }
        public string Unit { get; set; } = "unidad"; // opcional si existe en Products

        public TransactionHistoryFilters Filters { get; set; } = new();
        public int Total { get; set; }           // total de registros que cumplen el filtro
        public int Page { get; set; }            // página solicitada (normalizada)
        public int PageSize { get; set; }        // pageSize normalizado
        public int TotalPages { get; set; }      // cálculo: ceil(Total / PageSize)

        public List<TransactionHistoryItem> Items { get; set; } = new();
    }

    public class TransactionHistoryFilters
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Type { get; set; }     // "Compra"|"Venta"
        public string Sort { get; set; } = "dateDesc";
    }

    public class TransactionHistoryItem
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = "";       // "Compra"|"Venta"
        public int Quantity { get; set; }            // Convención: Compra=+, Venta=-
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Description { get; set; }
        public int UserExecutedId { get; set; }
        public string? Warehouse { get; set; }       // si llegas a tener almacén
    }
}
