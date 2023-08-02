using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string? Categoria1 { get; set; }

    public virtual ICollection<CategoriasProducto> CategoriasProductos { get; set; } = new List<CategoriasProducto>();

    public virtual ICollection<CategoriasTienda> CategoriasTienda { get; set; } = new List<CategoriasTienda>();
}
