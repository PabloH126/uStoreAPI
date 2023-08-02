using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class Calificacion
{
    public int IdCalificacion { get; set; }

    public int? Calificacion1 { get; set; }

    public int? IdTienda { get; set; }

    public int? IdProductos { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
