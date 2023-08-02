using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace uStoreAPI.ModelsSQLServer;

public partial class UstoreContext : DbContext
{
    public UstoreContext()
    {
    }

    public UstoreContext(DbContextOptions<UstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdministradorTiendum> AdministradorTienda { get; set; }

    public virtual DbSet<AlertaApartado> AlertaApartados { get; set; }

    public virtual DbSet<ApartadoActivo> ApartadoActivos { get; set; }

    public virtual DbSet<Calificacion> Calificacions { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<CategoriasProducto> CategoriasProductos { get; set; }

    public virtual DbSet<CategoriasTienda> CategoriasTiendas { get; set; }

    public virtual DbSet<CentroComercial> CentroComercials { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<CuentaAdministrador> CuentaAdministradors { get; set; }

    public virtual DbSet<CuentaGerente> CuentaGerentes { get; set; }

    public virtual DbSet<CuentaUsuario> CuentaUsuarios { get; set; }

    public virtual DbSet<Dato> Datos { get; set; }

    public virtual DbSet<DetallesAdministrador> DetallesAdministradors { get; set; }

    public virtual DbSet<DetallesCuentaAdministrador> DetallesCuentaAdministradors { get; set; }

    public virtual DbSet<DetallesCuentaGerente> DetallesCuentaGerentes { get; set; }

    public virtual DbSet<DetallesCuentaUsuario> DetallesCuentaUsuarios { get; set; }

    public virtual DbSet<DetallesMensaje> DetallesMensajes { get; set; }

    public virtual DbSet<DetallesUsuario> DetallesUsuarios { get; set; }

    public virtual DbSet<Favorito> Favoritos { get; set; }

    public virtual DbSet<Gerente> Gerentes { get; set; }

    public virtual DbSet<Historial> Historials { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<ImagenPerfil> ImagenPerfils { get; set; }

    public virtual DbSet<ImagenesMensaje> ImagenesMensajes { get; set; }

    public virtual DbSet<ImagenesProducto> ImagenesProductos { get; set; }

    public virtual DbSet<ImagenesPublicacion> ImagenesPublicacions { get; set; }

    public virtual DbSet<ImagenesTienda> ImagenesTiendas { get; set; }

    public virtual DbSet<LogoTiendum> LogoTienda { get; set; }

    public virtual DbSet<MensajeAdministrador> MensajeAdministradors { get; set; }

    public virtual DbSet<MensajeGerente> MensajeGerentes { get; set; }

    public virtual DbSet<MensajeUsuario> MensajeUsuarios { get; set; }

    public virtual DbSet<PenalizacionUsuario> PenalizacionUsuarios { get; set; }

    public virtual DbSet<PeriodosPredeterminado> PeriodosPredeterminados { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Publicacione> Publicaciones { get; set; }

    public virtual DbSet<RatioUsuario> RatioUsuarios { get; set; }

    public virtual DbSet<SolicitudesApartado> SolicitudesApartados { get; set; }

    public virtual DbSet<TendenciasVentum> TendenciasVenta { get; set; }

    public virtual DbSet<Tiendum> Tienda { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministradorTiendum>(entity =>
        {
            entity.HasKey(e => e.IdAdministrador).HasName("PK__administ__2B3E34A8DE14A4FE");

            entity.ToTable("administrador_tienda");

            entity.HasOne(d => d.IdDetallesAdministradorNavigation).WithMany(p => p.AdministradorTienda)
                .HasForeignKey(d => d.IdDetallesAdministrador)
                .HasConstraintName("administrador_tienda_ibfk_detalles_administrador");
        });

        modelBuilder.Entity<AlertaApartado>(entity =>
        {
            entity.HasKey(e => e.IdAlertaApartado).HasName("PK__alerta_a__8A0394B84EA516EC");

            entity.ToTable("alerta_apartado");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.AlertaApartados)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("alerta_apartado_ibfk_productos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.AlertaApartados)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("alerta_apartado_ibfk_usuarios");
        });

        modelBuilder.Entity<ApartadoActivo>(entity =>
        {
            entity.HasKey(e => e.IdApartado).HasName("PK__apartado__C5FE96245426B87E");

            entity.ToTable("apartado_activo");

            entity.Property(e => e.EstadoApartado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaApartado).HasColumnType("datetime");
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.ApartadoActivos)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("apartado_activo_ibfk_productos");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.ApartadoActivos)
                .HasForeignKey(d => d.IdSolicitud)
                .HasConstraintName("apartado_activo_ibfk_solicitudes_apartado");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.ApartadoActivos)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("apartado_activo_ibfk_tienda");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ApartadoActivos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("apartado_activo_ibfk_usuarios");
        });

        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.HasKey(e => e.IdCalificacion).HasName("PK__califica__40E4A751D1160DD1");

            entity.ToTable("calificacion");

            entity.Property(e => e.Calificacion1).HasColumnName("Calificacion");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.Calificacions)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("calificacion_ibfk_productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Calificacions)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("calificacion_ibfk_tienda");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__categori__A3C02A10DAF2AD82");

            entity.ToTable("categorias");

            entity.Property(e => e.Categoria1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Categoria");
        });

        modelBuilder.Entity<CategoriasProducto>(entity =>
        {
            entity.HasKey(e => e.IdCp).HasName("PK__categori__B7739093AF5EC440");

            entity.ToTable("categorias_producto");

            entity.Property(e => e.IdCp).HasColumnName("IdCP");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.CategoriasProductos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("categorias_producto_ibfk_categorias");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.CategoriasProductos)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("categorias_producto_ibfk_productos");
        });

        modelBuilder.Entity<CategoriasTienda>(entity =>
        {
            entity.HasKey(e => e.IdCt).HasName("PK__categori__B7739097FB33CEE9");

            entity.ToTable("categorias_tiendas");

            entity.Property(e => e.IdCt).HasColumnName("IdCT");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.CategoriasTienda)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("categorias_tiendas_ibfk_categorias");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.CategoriasTienda)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("categorias_tiendas_ibfk_tienda");
        });

        modelBuilder.Entity<CentroComercial>(entity =>
        {
            entity.HasKey(e => e.IdCentroComercial).HasName("PK__centro_c__A98C2E269308F1AF");

            entity.ToTable("centro_comercial");

            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PK__chat__3817F38CCB37F75A");

            entity.ToTable("chat");

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("chat_ibfk_administrador_tienda");

            entity.HasOne(d => d.IdGerenteNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdGerente)
                .HasConstraintName("chat_ibfk_gerente");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("chat_ibfk_usuarios");
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.IdComentarios).HasName("PK__comentar__3A9005883D90B4BE");

            entity.ToTable("comentarios");

            entity.Property(e => e.Comentario1)
                .HasColumnType("text")
                .HasColumnName("Comentario");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("comentarios_ibfk_productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("comentarios_ibfk_tienda");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("comentarios_ibfk_usuarios");
        });

        modelBuilder.Entity<CuentaAdministrador>(entity =>
        {
            entity.HasKey(e => e.IdCuentaAdministrador).HasName("PK__cuenta_a__656C3140EFF8E31B");

            entity.ToTable("cuenta_administrador");

            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.CuentaAdministradors)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("cuenta_administrador_ibfk_administrador_tienda");

            entity.HasOne(d => d.IdDetallesCuentaAdministradorNavigation).WithMany(p => p.CuentaAdministradors)
                .HasForeignKey(d => d.IdDetallesCuentaAdministrador)
                .HasConstraintName("cuenta_administrador_ibfk_detalles_cuenta_administrador");
        });

        modelBuilder.Entity<CuentaGerente>(entity =>
        {
            entity.HasKey(e => e.IdCuentaGerente).HasName("PK__cuenta_g__0E9C9B5608ECC12B");

            entity.ToTable("cuenta_gerente");

            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDetallesCuentaGerenteNavigation).WithMany(p => p.CuentaGerentes)
                .HasForeignKey(d => d.IdDetallesCuentaGerente)
                .HasConstraintName("cuenta_gerente_ibfk_detalles_cuenta_gerente");

            entity.HasOne(d => d.IdGerenteNavigation).WithMany(p => p.CuentaGerentes)
                .HasForeignKey(d => d.IdGerente)
                .HasConstraintName("cuenta_gerente_ibfk_gerente");
        });

        modelBuilder.Entity<CuentaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdCuentaUsuario).HasName("PK__cuenta_u__401CAAFC9B95E450");

            entity.ToTable("cuenta_usuario");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDetallesCuentaUsuarioNavigation).WithMany(p => p.CuentaUsuarios)
                .HasForeignKey(d => d.IdDetallesCuentaUsuario)
                .HasConstraintName("cuenta_usuario_ibfk_detalles_cuenta_usuario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CuentaUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("cuenta_usuario_ibfk_usuarios");
        });

        modelBuilder.Entity<Dato>(entity =>
        {
            entity.HasKey(e => e.IdDatos).HasName("PK__datos__A4BC7BC534B01EDE");

            entity.ToTable("datos");

            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PrimerNombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DetallesAdministrador>(entity =>
        {
            entity.HasKey(e => e.IdDetallesAdministrador).HasName("PK__detalles__3980381D5D0FBC1D");

            entity.ToTable("detalles_administrador");

            entity.HasOne(d => d.IdDatosNavigation).WithMany(p => p.DetallesAdministradors)
                .HasForeignKey(d => d.IdDatos)
                .HasConstraintName("detalles_administrador_ibfk_datos");
        });

        modelBuilder.Entity<DetallesCuentaAdministrador>(entity =>
        {
            entity.HasKey(e => e.IdDetallesCuentaAdministrador).HasName("PK__detalles__1006E22D8BF27033");

            entity.ToTable("detalles_cuenta_administrador");

            entity.Property(e => e.FechaRegistro).HasColumnType("date");

            entity.HasOne(d => d.IdImagenPerfilNavigation).WithMany(p => p.DetallesCuentaAdministradors)
                .HasForeignKey(d => d.IdImagenPerfil)
                .HasConstraintName("detalles_cuenta_administrador_ibfk_imagen_perfil");
        });

        modelBuilder.Entity<DetallesCuentaGerente>(entity =>
        {
            entity.HasKey(e => e.IdDetallesCuentaGerente).HasName("PK__detalles__B6D214009B89894C");

            entity.ToTable("detalles_cuenta_gerente");

            entity.Property(e => e.FechaRegistro).HasColumnType("date");

            entity.HasOne(d => d.IdImagenPerfilNavigation).WithMany(p => p.DetallesCuentaGerentes)
                .HasForeignKey(d => d.IdImagenPerfil)
                .HasConstraintName("detalles_cuenta_gerente_ibfk_imagen_perfil");
        });

        modelBuilder.Entity<DetallesCuentaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDetallesCuentaUsuario).HasName("PK__detalles__70401F92F94EE96D");

            entity.ToTable("detalles_cuenta_usuario");

            entity.Property(e => e.FechaRegistro).HasColumnType("date");

            entity.HasOne(d => d.IdImagenPerfilNavigation).WithMany(p => p.DetallesCuentaUsuarios)
                .HasForeignKey(d => d.IdImagenPerfil)
                .HasConstraintName("detalles_cuenta_usuario_ibfk_imagen_perfil");
        });

        modelBuilder.Entity<DetallesMensaje>(entity =>
        {
            entity.HasKey(e => e.IdDetallesMensaje).HasName("PK__detalles__10722F5E6C3FC77E");

            entity.ToTable("detalles_mensaje");

            entity.Property(e => e.Contenido).HasColumnType("text");
            entity.Property(e => e.FechaMensaje).HasColumnType("datetime");

            entity.HasOne(d => d.IdChatNavigation).WithMany(p => p.DetallesMensajes)
                .HasForeignKey(d => d.IdChat)
                .HasConstraintName("detalles_mensaje_ibfk_chat");

            entity.HasOne(d => d.IdImagenesMensajeNavigation).WithMany(p => p.DetallesMensajes)
                .HasForeignKey(d => d.IdImagenesMensaje)
                .HasConstraintName("detalles_mensaje_ibfk_imagenes_mensaje");
        });

        modelBuilder.Entity<DetallesUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDetallesUsuario).HasName("PK__detalles__4892D96196B12C6A");

            entity.ToTable("detalles_usuario");

            entity.Property(e => e.Estado)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDatosNavigation).WithMany(p => p.DetallesUsuarios)
                .HasForeignKey(d => d.IdDatos)
                .HasConstraintName("detalles_usuario_ibfk_datos");

            entity.HasOne(d => d.IdPenalizacionUsuarioNavigation).WithMany(p => p.DetallesUsuarios)
                .HasForeignKey(d => d.IdPenalizacionUsuario)
                .HasConstraintName("detalles_usuario_ibfk_penalizacion_usuario");

            entity.HasOne(d => d.IdRatioUsuarioNavigation).WithMany(p => p.DetallesUsuarios)
                .HasForeignKey(d => d.IdRatioUsuario)
                .HasConstraintName("detalles_usuario_ibfk_ratio_usuario");
        });

        modelBuilder.Entity<Favorito>(entity =>
        {
            entity.HasKey(e => e.IdFavoritos).HasName("PK__favorito__085B60772EF3B0F6");

            entity.ToTable("favoritos");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("favoritos_ibfk_productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("favoritos_ibfk_tienda");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("favoritos_ibfk_usuarios");
        });

        modelBuilder.Entity<Gerente>(entity =>
        {
            entity.HasKey(e => e.IdGerente).HasName("PK__gerente__06016C45DD7C1FF6");

            entity.ToTable("gerente");

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.Gerentes)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("gerente_ibfk_administrador_tienda");

            entity.HasOne(d => d.IdDatosNavigation).WithMany(p => p.Gerentes)
                .HasForeignKey(d => d.IdDatos)
                .HasConstraintName("gerente_ibfk_datos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Gerentes)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("gerente_ibfk_tienda");
        });

        modelBuilder.Entity<Historial>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__historia__9CC7DBB4E25B1CE4");

            entity.ToTable("historial");

            entity.Property(e => e.FechaVisita).HasColumnType("datetime");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.Historials)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("historial_ibfk_productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Historials)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("historial_ibfk_tienda");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Historials)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("historial_ibfk_usuarios");
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("PK__horario__1539229B54FA5BBC");

            entity.ToTable("horario");

            entity.Property(e => e.Dia)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.HorarioApertura)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.HorarioCierre)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Horarios)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("horario_ibfk_tienda");
        });

        modelBuilder.Entity<ImagenPerfil>(entity =>
        {
            entity.HasKey(e => e.IdImagenPerfil).HasName("PK__imagen_p__32D3A3600224DB61");

            entity.ToTable("imagen_perfil");

            entity.Property(e => e.IconoPerfil)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ImagenesMensaje>(entity =>
        {
            entity.HasKey(e => e.IdImagenesMensaje).HasName("PK__imagenes__3446ACF10D5623B4");

            entity.ToTable("imagenes_mensaje");

            entity.Property(e => e.ImagenMensaje)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ImagenesProducto>(entity =>
        {
            entity.HasKey(e => e.IdImagenesProductos).HasName("PK__imagenes__99684703CD9D5017");

            entity.ToTable("imagenes_productos");

            entity.Property(e => e.ImagenProducto)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.ImagenesProductos)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("imagenes_productos_ibfk_productos");
        });

        modelBuilder.Entity<ImagenesPublicacion>(entity =>
        {
            entity.HasKey(e => e.IdImagenesPublicacion).HasName("PK__imagenes__AA5AE371AD4D14E1");

            entity.ToTable("imagenes_publicacion");

            entity.Property(e => e.ImagenPublicacion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.ImagenesPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("imagenes_publicacion_ibfk_publicaciones");
        });

        modelBuilder.Entity<ImagenesTienda>(entity =>
        {
            entity.HasKey(e => e.IdImagenesTiendas).HasName("PK__imagenes__E284A8750EFF731F");

            entity.ToTable("imagenes_tiendas");

            entity.Property(e => e.ImagenTienda)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.ImagenesTienda)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("imagenes_tiendas_ibfk_tienda");
        });

        modelBuilder.Entity<LogoTiendum>(entity =>
        {
            entity.HasKey(e => e.IdLogoTienda).HasName("PK__logo_tie__66AF103C1E3275E2");

            entity.ToTable("logo_tienda");

            entity.Property(e => e.LogoTienda)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.LogoTienda)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("logo_tienda_ibfk_tienda");
        });

        modelBuilder.Entity<MensajeAdministrador>(entity =>
        {
            entity.HasKey(e => e.IdMensajeAdministrador).HasName("PK__mensaje___8654F5EE161270D2");

            entity.ToTable("mensaje_administrador");

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.MensajeAdministradors)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("mensaje_administrador_ibfk_administrador_tienda");

            entity.HasOne(d => d.IdDetallesMensajeNavigation).WithMany(p => p.MensajeAdministradors)
                .HasForeignKey(d => d.IdDetallesMensaje)
                .HasConstraintName("mensaje_administrador_ibfk_detalles_mensaje");
        });

        modelBuilder.Entity<MensajeGerente>(entity =>
        {
            entity.HasKey(e => e.IdMensajeGerente).HasName("PK__mensaje___D9B2149B36AABDE9");

            entity.ToTable("mensaje_gerente");

            entity.HasOne(d => d.IdDetallesMensajeNavigation).WithMany(p => p.MensajeGerentes)
                .HasForeignKey(d => d.IdDetallesMensaje)
                .HasConstraintName("mensaje_gerente_ibfk_detalles_mensaje");

            entity.HasOne(d => d.IdGerenteNavigation).WithMany(p => p.MensajeGerentes)
                .HasForeignKey(d => d.IdGerente)
                .HasConstraintName("mensaje_gerente_ibfk_gerente");
        });

        modelBuilder.Entity<MensajeUsuario>(entity =>
        {
            entity.HasKey(e => e.IdMensajeUsuario).HasName("PK__mensaje___8350592B218D2103");

            entity.ToTable("mensaje_usuario");

            entity.HasOne(d => d.IdDetallesMensajeNavigation).WithMany(p => p.MensajeUsuarios)
                .HasForeignKey(d => d.IdDetallesMensaje)
                .HasConstraintName("mensaje_usuario_ibfk_detalles_mensaje");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.MensajeUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("mensaje_usuario_ibfk_usuarios");
        });

        modelBuilder.Entity<PenalizacionUsuario>(entity =>
        {
            entity.HasKey(e => e.IdPenalizacionUsuario).HasName("PK__penaliza__5B75DF67ABEB077C");

            entity.ToTable("penalizacion_usuario");
        });

        modelBuilder.Entity<PeriodosPredeterminado>(entity =>
        {
            entity.HasKey(e => e.IdApartadoPredeterminado).HasName("PK__periodos__363145410BD10C11");

            entity.ToTable("periodos_predeterminados");

            entity.Property(e => e.ApartadoPredeterminado)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.PeriodosPredeterminados)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("periodos_predeterminados_ibfk_tienda");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProductos).HasName("PK__producto__718C7D07B98A3D23");

            entity.ToTable("productos");

            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("productos_ibfk_tienda");
        });

        modelBuilder.Entity<Publicacione>(entity =>
        {
            entity.HasKey(e => e.IdPublicacion).HasName("PK__publicac__24F1B7D3B00E8731");

            entity.ToTable("publicaciones");

            entity.Property(e => e.Contenido).HasColumnType("text");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Publicaciones)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("publicaciones_ibfk_tienda");
        });

        modelBuilder.Entity<RatioUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRatioUsuario).HasName("PK__ratio_us__9359AF448698CF8A");

            entity.ToTable("ratio_usuario");
        });

        modelBuilder.Entity<SolicitudesApartado>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__36899CEFE0D2F4AE");

            entity.ToTable("solicitudes_apartado");

            entity.Property(e => e.PeriodoApartado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StatusSolicitud)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.SolicitudesApartados)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("solicitudes_apartado_ibfk_productos");

            entity.HasOne(d => d.IdRatioUsuarioNavigation).WithMany(p => p.SolicitudesApartados)
                .HasForeignKey(d => d.IdRatioUsuario)
                .HasConstraintName("solicitudes_apartado_ibfk_ratio_usuario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.SolicitudesApartados)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("solicitudes_apartado_ibfk_usuarios");
        });

        modelBuilder.Entity<TendenciasVentum>(entity =>
        {
            entity.HasKey(e => e.IdTendencia).HasName("PK__tendenci__C139244DB0A87D7B");

            entity.ToTable("tendencias_venta");

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.TendenciasVenta)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("tendencias_venta_ibfk_productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.TendenciasVenta)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("tendencias_venta_ibfk_tienda");
        });

        modelBuilder.Entity<Tiendum>(entity =>
        {
            entity.HasKey(e => e.IdTienda).HasName("PK__tienda__5A1EB96B142FB8E7");

            entity.ToTable("tienda");

            entity.Property(e => e.NombreTienda)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RangoPrecio)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAdministradorNavigation).WithMany(p => p.Tienda)
                .HasForeignKey(d => d.IdAdministrador)
                .HasConstraintName("tienda_ibfk_administrador_tienda");

            entity.HasOne(d => d.IdCentroComercialNavigation).WithMany(p => p.Tienda)
                .HasForeignKey(d => d.IdCentroComercial)
                .HasConstraintName("tienda_ibfk_centro_comercial");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__5B65BF9778479944");

            entity.ToTable("usuarios");

            entity.HasOne(d => d.IdDetallesUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdDetallesUsuario)
                .HasConstraintName("usuarios_ibfk_detalles_usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
