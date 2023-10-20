using System;
using System.Collections.Generic;

namespace API_Ventas.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string DescProducto { get; set; } = null!;

    public int Precio { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
