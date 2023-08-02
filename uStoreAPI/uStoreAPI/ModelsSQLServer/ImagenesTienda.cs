using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class ImagenesTienda
{
    public int IdImagenesTiendas { get; set; }

    public string? ImagenTienda { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
