namespace Transacciones.Request
{
    public class TransactionRequest
    {
        public List<ProductsInfo> Productos { get; set; } = new List<ProductsInfo>();
        public string Descripcion { get; set; } = string.Empty;
        public int UserId { get; set; }

    }
    public class  ProductsInfo
    {
        public int ProId { get; set; }
        public int Cantidad { get; set; }
    }
}
