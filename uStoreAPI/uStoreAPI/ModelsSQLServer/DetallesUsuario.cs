using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class DetallesUsuario
{
    public int IdDetallesUsuario { get; set; }

    public string? Estado { get; set; }

    public int? IdPenalizacionUsuario { get; set; }

    public int? IdRatioUsuario { get; set; }

    public int? IdDatos { get; set; }

    public virtual Dato? IdDatosNavigation { get; set; }

    public virtual PenalizacionUsuario? IdPenalizacionUsuarioNavigation { get; set; }

    public virtual RatioUsuario? IdRatioUsuarioNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
