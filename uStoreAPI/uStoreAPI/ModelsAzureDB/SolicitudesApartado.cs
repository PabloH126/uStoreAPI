using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class SolicitudesApartado
{
    public int IdSolicitud { get; set; }

    public string? PeriodoApartado { get; set; }

    public int? UnidadesProducto { get; set; }

    public string? StatusSolicitud { get; set; }

    public int? IdProductos { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdTienda { get; set; }

    public DateTime? FechaApartado { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public DateTime? FechaSolicitud { get; set; }

    public string? IdJob { get; set; }

    public virtual Producto? IdProductosNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
