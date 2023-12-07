using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Publicacione
{
    public int IdPublicacion { get; set; }

    public string? Contenido { get; set; }

    public int? IdTienda { get; set; }

    public DateTime? FechaPublicacion { get; set; }

    public int? IdCentroComercial { get; set; }

    public string? Imagen { get; set; }

    public string? ImagenThumbNail { get; set; }

    public virtual CentroComercial? IdCentroComercialNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual ICollection<NotificacionUsuario> NotificacionUsuarios { get; set; } = new List<NotificacionUsuario>();
}
