using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Comentario
{
    public int IdComentarios { get; set; }

    public string? Comentario1 { get; set; }

    public int? IdProductos { get; set; }

    public int? IdTienda { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
