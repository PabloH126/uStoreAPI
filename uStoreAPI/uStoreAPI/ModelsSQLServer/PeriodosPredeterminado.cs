using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class PeriodosPredeterminado
{
    public int IdApartadoPredeterminado { get; set; }

    public string? ApartadoPredeterminado { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
