using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class DetallesCuentaUsuario
{
    public int IdDetallesCuentaUsuario { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdImagenPerfil { get; set; }

    public virtual ICollection<CuentaUsuario> CuentaUsuarios { get; set; } = new List<CuentaUsuario>();

    public virtual ImagenPerfil? IdImagenPerfilNavigation { get; set; }
}
