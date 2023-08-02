using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class DetallesCuentaAdministrador
{
    public int IdDetallesCuentaAdministrador { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdImagenPerfil { get; set; }

    public virtual ICollection<CuentaAdministrador> CuentaAdministradors { get; set; } = new List<CuentaAdministrador>();

    public virtual ImagenPerfil? IdImagenPerfilNavigation { get; set; }
}
