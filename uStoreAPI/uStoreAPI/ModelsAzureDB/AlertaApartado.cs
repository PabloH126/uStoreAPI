using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class AlertaApartado
{
    public int IdAlertaApartado { get; set; }

    public int? IdProductos { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
