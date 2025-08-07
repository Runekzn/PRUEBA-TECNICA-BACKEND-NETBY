using System;
using System.Collections.Generic;

namespace AccessData.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public DateTime TraFechaRealizada { get; set; }

    public string TraTipoTransaccion { get; set; } = null!;

    public int TraProducto { get; set; }

    public int TraCantidad { get; set; }

    public decimal TraPrecioUnitario { get; set; }

    public decimal TraPrecioTotal { get; set; }

    public string TraDescripcion { get; set; } = null!;

    public int TraUserExecuted { get; set; }

    public bool? Status { get; set; }

    public virtual Product TraProductoNavigation { get; set; } = null!;

    public virtual User TraUserExecutedNavigation { get; set; } = null!;
}
