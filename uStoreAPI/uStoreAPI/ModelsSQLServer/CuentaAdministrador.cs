using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class CuentaAdministrador
{
    public int IdCuentaAdministrador { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? IdDetallesCuentaAdministrador { get; set; }

    public int? IdAdministrador { get; set; }

    public virtual AdministradorTiendum? IdAdministradorNavigation { get; set; }

    public virtual DetallesCuentaAdministrador? IdDetallesCuentaAdministradorNavigation { get; set; }
}
