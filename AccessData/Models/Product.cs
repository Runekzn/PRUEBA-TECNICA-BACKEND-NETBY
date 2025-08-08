using System;
using System.Collections.Generic;

namespace AccessData.Models;

public partial class Product
{
    public int Id { get; set; }

    public string ProNombre { get; set; } = null!;

    public string ProDescripcion { get; set; } = null!;

    public int ProCategoria { get; set; }

    public string ProImagen { get; set; } = null!;

    public decimal ProPrecio { get; set; }

    public int ProStock { get; set; }

    public int ProCreatedBy { get; set; }

    public bool Status { get; set; }

    public virtual Categorium ProCategoriaNavigation { get; set; } = null!;

    public virtual User ProCreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
