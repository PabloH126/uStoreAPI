using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class ImagenesProducto
{
    public int IdImagenesProductos { get; set; }

    public string? ImagenProducto { get; set; }

    public int? IdProductos { get; set; }

    public string? ImagenProductoThumbNail { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }
}
