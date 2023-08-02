using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class DetallesAdministrador
{
    public int IdDetallesAdministrador { get; set; }

    public int? IdDatos { get; set; }

    public virtual ICollection<AdministradorTiendum> AdministradorTienda { get; set; } = new List<AdministradorTiendum>();

    public virtual Dato? IdDatosNavigation { get; set; }
}
