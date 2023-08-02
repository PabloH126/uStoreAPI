using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class ApartadoActivo
{
    public int IdApartado { get; set; }

    public DateTime? FechaApartado { get; set; }

    public string? EstadoApartado { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdTienda { get; set; }

    public int? IdProductos { get; set; }

    public int? IdSolicitud { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }

    public virtual SolicitudesApartado? IdSolicitudNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
