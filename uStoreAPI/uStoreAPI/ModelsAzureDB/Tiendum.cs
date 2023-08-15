using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Tiendum
{
    public int IdTienda { get; set; }

    public string? NombreTienda { get; set; }

    public string? RangoPrecio { get; set; }

    public int? IdCentroComercial { get; set; }

    public int? IdAdministrador { get; set; }

    public int? Apartados { get; set; }

    public int? Vistas { get; set; }

    public string? LogoTienda { get; set; }

    public virtual ICollection<ApartadoActivo> ApartadoActivos { get; set; } = new List<ApartadoActivo>();

    public virtual ICollection<Calificacion> Calificacions { get; set; } = new List<Calificacion>();

    public virtual ICollection<CategoriasTienda> CategoriasTienda { get; set; } = new List<CategoriasTienda>();

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

    public virtual ICollection<Gerente> Gerentes { get; set; } = new List<Gerente>();

    public virtual ICollection<Historial> Historials { get; set; } = new List<Historial>();

    public virtual ICollection<Horario> Horarios { get; set; } = new List<Horario>();

    public virtual AdministradorTiendum? IdAdministradorNavigation { get; set; }

    public virtual CentroComercial? IdCentroComercialNavigation { get; set; }

    public virtual ICollection<ImagenesTienda> ImagenesTienda { get; set; } = new List<ImagenesTienda>();

    public virtual ICollection<PeriodosPredeterminado> PeriodosPredeterminados { get; set; } = new List<PeriodosPredeterminado>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Publicacione> Publicaciones { get; set; } = new List<Publicacione>();

    public virtual ICollection<TendenciasVentum> TendenciasVenta { get; set; } = new List<TendenciasVentum>();
}
