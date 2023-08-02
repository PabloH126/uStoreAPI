using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class SolicitudesApartado
{
    public int IdSolicitud { get; set; }

    public string? PeriodoApartado { get; set; }

    public int? UnidadesProducto { get; set; }

    public string? StatusSolicitud { get; set; }

    public int? IdRatioUsuario { get; set; }

    public int? IdProductos { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<ApartadoActivo> ApartadoActivos { get; set; } = new List<ApartadoActivo>();

    public virtual Producto? IdProductosNavigation { get; set; }

    public virtual RatioUsuario? IdRatioUsuarioNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
