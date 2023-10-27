using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_Ventas.Models;

public partial class Usuario
{
    public string NomUsuario { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Estado { get; set; }
    [JsonIgnore]
    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
