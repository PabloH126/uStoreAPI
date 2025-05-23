﻿using System;
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

    public string? LogoTiendaThumbNail { get; set; }

    public virtual ICollection<CalificacionTiendum> CalificacionTienda { get; set; } = new List<CalificacionTiendum>();

    public virtual ICollection<CategoriasTienda> CategoriasTienda { get; set; } = new List<CategoriasTienda>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<ComentariosTienda> ComentariosTienda { get; set; } = new List<ComentariosTienda>();

    public virtual ICollection<FavoritosTiendum> FavoritosTienda { get; set; } = new List<FavoritosTiendum>();

    public virtual ICollection<Gerente> Gerentes { get; set; } = new List<Gerente>();

    public virtual ICollection<Historial> Historials { get; set; } = new List<Historial>();

    public virtual ICollection<Horario> Horarios { get; set; } = new List<Horario>();

    public virtual AdministradorTiendum? IdAdministradorNavigation { get; set; }

    public virtual CentroComercial? IdCentroComercialNavigation { get; set; }

    public virtual ICollection<ImagenesTienda> ImagenesTienda { get; set; } = new List<ImagenesTienda>();

    public virtual ICollection<NotificacionUsuario> NotificacionUsuarios { get; set; } = new List<NotificacionUsuario>();

    public virtual ICollection<PeriodosPredeterminado> PeriodosPredeterminados { get; set; } = new List<PeriodosPredeterminado>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Publicacione> Publicaciones { get; set; } = new List<Publicacione>();

    public virtual ICollection<SolicitudesApartado> SolicitudesApartados { get; set; } = new List<SolicitudesApartado>();

    public virtual ICollection<TendenciasVentum> TendenciasVenta { get; set; } = new List<TendenciasVentum>();
}
