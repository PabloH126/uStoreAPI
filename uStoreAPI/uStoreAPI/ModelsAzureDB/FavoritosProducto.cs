using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class FavoritosProducto
{
    public int IdFavoritoProducto { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
