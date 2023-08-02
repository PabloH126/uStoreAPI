using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class MensajeGerente
{
    public int IdMensajeGerente { get; set; }

    public int? IdDetallesMensaje { get; set; }

    public int? IdGerente { get; set; }

    public virtual DetallesMensaje? IdDetallesMensajeNavigation { get; set; }

    public virtual Gerente? IdGerenteNavigation { get; set; }
}
