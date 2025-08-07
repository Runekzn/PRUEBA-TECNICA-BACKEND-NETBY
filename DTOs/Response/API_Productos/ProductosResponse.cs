using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response.API_Productos
{
    public class ProductosResponse
    {
        public int Id { get; set; }

        public string ProNombre { get; set; } = null!;

        public string ProDescripcion { get; set; } = null!;

        public int ProCategoria { get; set; }

        public string ProImagen { get; set; } = null!;

        public decimal ProPrecio { get; set; }

        public int ProStock { get; set; }

        public string ProCreatedBy { get; set; } = null!;

    }
}
