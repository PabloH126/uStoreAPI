using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class CalificacionTiendum
{
    public int IdCalificacionTienda { get; set; }

    public int? Calificacion { get; set; }

    public int? IdTienda { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
