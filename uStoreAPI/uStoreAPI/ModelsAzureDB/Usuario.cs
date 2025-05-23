﻿using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdDetallesUsuario { get; set; }

    public virtual ICollection<AlertaApartado> AlertaApartados { get; set; } = new List<AlertaApartado>();

    public virtual ICollection<CalificacionProducto> CalificacionProductos { get; set; } = new List<CalificacionProducto>();

    public virtual ICollection<CalificacionTiendum> CalificacionTienda { get; set; } = new List<CalificacionTiendum>();

    public virtual ICollection<ComentariosProducto> ComentariosProductos { get; set; } = new List<ComentariosProducto>();

    public virtual ICollection<ComentariosTienda> ComentariosTienda { get; set; } = new List<ComentariosTienda>();

    public virtual ICollection<ConfiguracionAppUsuario> ConfiguracionAppUsuarios { get; set; } = new List<ConfiguracionAppUsuario>();

    public virtual ICollection<CuentaUsuario> CuentaUsuarios { get; set; } = new List<CuentaUsuario>();

    public virtual ICollection<FavoritosProducto> FavoritosProductos { get; set; } = new List<FavoritosProducto>();

    public virtual ICollection<FavoritosTiendum> FavoritosTienda { get; set; } = new List<FavoritosTiendum>();

    public virtual ICollection<Historial> Historials { get; set; } = new List<Historial>();

    public virtual DetallesUsuario? IdDetallesUsuarioNavigation { get; set; }

    public virtual ICollection<NotificacionUsuario> NotificacionUsuarios { get; set; } = new List<NotificacionUsuario>();

    public virtual ICollection<PenalizacionUsuario> PenalizacionUsuarios { get; set; } = new List<PenalizacionUsuario>();

    public virtual ICollection<SolicitudesApartado> SolicitudesApartados { get; set; } = new List<SolicitudesApartado>();
}
