using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class FavoritosTiendum
{
    public int IdFavoritoTienda { get; set; }

    public int IdUsuario { get; set; }

    public int IdTienda { get; set; }

    public virtual Tiendum IdTiendaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
