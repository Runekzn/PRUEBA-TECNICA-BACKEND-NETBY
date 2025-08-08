using System;
using System.Collections.Generic;

namespace AccessData.Models;

public partial class Categorium
{
    public int Id { get; set; }

    public string CatNombre { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
