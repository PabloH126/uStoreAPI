using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class NotificacionUsuario
{
    public int IdNotificacion { get; set; }

    public int IdUsuario { get; set; }

    public int IdPublicacion { get; set; }

    public DateTime FechaNotificacion { get; set; }

    public virtual Publicacione IdPublicacionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
