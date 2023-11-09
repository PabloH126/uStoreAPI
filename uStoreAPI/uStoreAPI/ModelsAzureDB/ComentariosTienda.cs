using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class ComentariosTienda
{
    public int IdComentarioTienda { get; set; }

    public string Comentario { get; set; } = null!;

    public DateTime FechaComentario { get; set; }

    public int IdUsuario { get; set; }

    public int IdTienda { get; set; }

    public virtual Tiendum IdTiendaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
