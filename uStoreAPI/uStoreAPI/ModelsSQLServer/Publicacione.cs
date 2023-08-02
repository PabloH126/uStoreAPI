using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class Publicacione
{
    public int IdPublicacion { get; set; }

    public string? Contenido { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual ICollection<ImagenesPublicacion> ImagenesPublicacions { get; set; } = new List<ImagenesPublicacion>();
}
