using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class LogoTiendum
{
    public int IdLogoTienda { get; set; }

    public string? LogoTienda { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
