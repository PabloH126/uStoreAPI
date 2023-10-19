using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class AdministradorTiendum
{
    public int IdAdministrador { get; set; }

    public int? IdDetallesAdministrador { get; set; }

    public virtual ICollection<CuentaAdministrador> CuentaAdministradors { get; set; } = new List<CuentaAdministrador>();

    public virtual ICollection<Gerente> Gerentes { get; set; } = new List<Gerente>();

    public virtual DetallesAdministrador? IdDetallesAdministradorNavigation { get; set; }

    public virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();
}
