using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class MensajeAdministrador
{
    public int IdMensajeAdministrador { get; set; }

    public int? IdDetallesMensaje { get; set; }

    public int? IdAdministrador { get; set; }

    public virtual AdministradorTiendum? IdAdministradorNavigation { get; set; }

    public virtual DetallesMensaje? IdDetallesMensajeNavigation { get; set; }
}
