using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_Ventas.Models;

public partial class Ventum
{
    [JsonIgnore]
    public int IdVenta { get; set; }

    public string NomUsuario { get; set; } = null!;

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public int Total { get; set; }
    [JsonIgnore]
    public DateTime FechaVenta { get; set; }

    public int Estado { get; set; }
    [JsonIgnore]
    public virtual Producto IdProductoNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Usuario NomUsuarioNavigation { get; set; } = null!;
}
