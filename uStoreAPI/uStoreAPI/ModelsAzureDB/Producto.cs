using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Producto
{
    public int IdProductos { get; set; }

    public string? NombreProducto { get; set; }

    public double? PrecioProducto { get; set; }

    public int? CantidadApartado { get; set; }

    public string? Descripcion { get; set; }

    public int? Stock { get; set; }

    public int? IdTienda { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<AlertaApartado> AlertaApartados { get; set; } = new List<AlertaApartado>();

    public virtual ICollection<CalificacionProducto> CalificacionProductos { get; set; } = new List<CalificacionProducto>();

    public virtual ICollection<CategoriasProducto> CategoriasProductos { get; set; } = new List<CategoriasProducto>();

    public virtual ICollection<ComentariosProducto> ComentariosProductos { get; set; } = new List<ComentariosProducto>();

    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

    public virtual ICollection<Historial> Historials { get; set; } = new List<Historial>();

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual ICollection<ImagenesProducto> ImagenesProductos { get; set; } = new List<ImagenesProducto>();

    public virtual ICollection<SolicitudesApartado> SolicitudesApartados { get; set; } = new List<SolicitudesApartado>();

    public virtual ICollection<TendenciasVentum> TendenciasVenta { get; set; } = new List<TendenciasVentum>();
}
