using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class MensajeUsuario
{
    public int IdMensajeUsuario { get; set; }

    public int? IdDetallesMensaje { get; set; }

    public int? IdUsuario { get; set; }

    public virtual DetallesMensaje? IdDetallesMensajeNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
