using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class CategoriasProducto
{
    public int IdCp { get; set; }

    public int? IdProductos { get; set; }

    public int? IdCategoria { get; set; }

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }
}
