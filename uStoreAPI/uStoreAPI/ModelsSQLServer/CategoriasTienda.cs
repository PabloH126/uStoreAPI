using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class CategoriasTienda
{
    public int IdCt { get; set; }

    public int? IdTienda { get; set; }

    public int? IdCategoria { get; set; }

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
