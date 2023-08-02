using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class CuentaGerente
{
    public int IdCuentaGerente { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? IdDetallesCuentaGerente { get; set; }

    public int? IdGerente { get; set; }

    public virtual DetallesCuentaGerente? IdDetallesCuentaGerenteNavigation { get; set; }

    public virtual Gerente? IdGerenteNavigation { get; set; }
}
