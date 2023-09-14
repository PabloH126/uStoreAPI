using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class CuentaUsuario
{
    public int IdCuentaUsuario { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? IdDetallesCuentaUsuario { get; set; }

    public int? IdUsuario { get; set; }

    public virtual DetallesCuentaUsuario? IdDetallesCuentaUsuarioNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
