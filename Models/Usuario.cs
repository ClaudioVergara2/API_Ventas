using System;
using System.Collections.Generic;

namespace API_Ventas.Models;

public partial class Usuario
{
    public string NomUsuario { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Estado { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
