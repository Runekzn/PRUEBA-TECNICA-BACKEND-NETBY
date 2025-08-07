namespace DTOs.Request.API_Productos
{
    public class ProductoRequest
    {
        public int ProCategoria { get; set; }
        public string ProDescripcion { get; set; } = string.Empty;

        public string ProImagen { get; set; } = string.Empty;
        public string ProNombre { get; set; } = string.Empty;
        public decimal ProPrecio { get; set; }
        public int ProStock { get; set; }
    }
}
