using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace uStoreAPI.ModelsAzureDB;

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

    public virtual DbSet<CalificacionProducto> CalificacionProductos { get; set; }

    public virtual DbSet<CalificacionTiendum> CalificacionTienda { get; set; }

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
        modelBuilder.UseCollation("Modern_Spanish_CI_AS");

        modelBuilder.Entity<AdministradorTiendum>(entity =>
        {
            entity.HasKey(e => e.IdAdministrador).HasName("PK__administ__2B3E34A8C0FC9494");

            entity.ToTable("administrador_tienda");

            entity.HasOne(d => d.IdDetallesAdministradorNavigation).WithMany(p => p.AdministradorTienda)
                .HasForeignKey(d => d.IdDetallesAdministrador)
                .HasConstraintName("administrador_tienda_ibfk_detalles_administrador");
        });

        modelBuilder.Entity<AlertaApartado>(entity =>
        {
            entity.HasKey(e => e.IdAlertaApartado).HasName("PK__alerta_a__8A0394B89A43C01D");

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
            entity.HasKey(e => e.IdApartado).HasName("PK__apartado__C5FE962405E06459");

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

        modelBuilder.Entity<CalificacionProducto>(entity =>
        {
            entity.HasKey(e => e.IdCalificacionProducto).HasName("PK__califica__EA5510DEE7271197");

            entity.ToTable("calificacion_producto");

            entity.HasIndex(e => new { e.IdProductos, e.IdUsuario }, "UQ__califica__143A26FF2DBF3CEE").IsUnique();

            entity.Property(e => e.IdCalificacionProducto).ValueGeneratedNever();

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.CalificacionProductos)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("FK__calificac__IdPro__6D9742D9");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CalificacionProductos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__calificac__IdUsu__6E8B6712");
        });

        modelBuilder.Entity<CalificacionTiendum>(entity =>
        {
            entity.HasKey(e => e.IdCalificacionTienda).HasName("PK__califica__5CDFD1C3862890FF");

            entity.ToTable("calificacion_tienda");

            entity.HasIndex(e => new { e.IdTienda, e.IdUsuario }, "UQ__califica__3FA8E29388E3372A").IsUnique();

            entity.Property(e => e.IdCalificacionTienda).ValueGeneratedNever();

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.CalificacionTienda)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK__calificac__IdTie__68D28DBC");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CalificacionTienda)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__calificac__IdUsu__69C6B1F5");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__categori__A3C02A10EF344F84");

            entity.ToTable("categorias");

            entity.Property(e => e.Categoria1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Categoria");
        });

        modelBuilder.Entity<CategoriasProducto>(entity =>
        {
            entity.HasKey(e => e.IdCp).HasName("PK__categori__B773909356A3FD3D");

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
            entity.HasKey(e => e.IdCt).HasName("PK__categori__B773909713DD1A65");

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
            entity.HasKey(e => e.IdCentroComercial).HasName("PK__centro_c__A98C2E26E80AAF2A");

            entity.ToTable("centro_comercial");

            entity.Property(e => e.DireccionCentroComercial)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HorarioCentroComercial)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.IconoCentroComercial)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ImagenCentroComercial)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreCentroComercial)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PK__chat__3817F38CB2461E0A");

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
            entity.HasKey(e => e.IdComentarios).HasName("PK__comentar__3A900588B2B70FBF");

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
            entity.HasKey(e => e.IdCuentaAdministrador).HasName("PK__cuenta_a__656C314025F6429C");

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
            entity.HasKey(e => e.IdCuentaGerente).HasName("PK__cuenta_g__0E9C9B5637FB9160");

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
            entity.HasKey(e => e.IdCuentaUsuario).HasName("PK__cuenta_u__401CAAFC063DBD2F");

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
            entity.HasKey(e => e.IdDatos).HasName("PK__datos__A4BC7BC5C310E642");

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
            entity.HasKey(e => e.IdDetallesAdministrador).HasName("PK__detalles__3980381D4F65EAA2");

            entity.ToTable("detalles_administrador");

            entity.HasOne(d => d.IdDatosNavigation).WithMany(p => p.DetallesAdministradors)
                .HasForeignKey(d => d.IdDatos)
                .HasConstraintName("detalles_administrador_ibfk_datos");
        });

        modelBuilder.Entity<DetallesCuentaAdministrador>(entity =>
        {
            entity.HasKey(e => e.IdDetallesCuentaAdministrador).HasName("PK__detalles__1006E22D21EB29BB");

            entity.ToTable("detalles_cuenta_administrador");

            entity.Property(e => e.FechaRegistro).HasColumnType("date");

            entity.HasOne(d => d.IdImagenPerfilNavigation).WithMany(p => p.DetallesCuentaAdministradors)
                .HasForeignKey(d => d.IdImagenPerfil)
                .HasConstraintName("detalles_cuenta_administrador_ibfk_imagen_perfil");
        });

        modelBuilder.Entity<DetallesCuentaGerente>(entity =>
        {
            entity.HasKey(e => e.IdDetallesCuentaGerente).HasName("PK__detalles__B6D2140078C32273");

            entity.ToTable("detalles_cuenta_gerente");

            entity.Property(e => e.FechaRegistro).HasColumnType("date");

            entity.HasOne(d => d.IdImagenPerfilNavigation).WithMany(p => p.DetallesCuentaGerentes)
                .HasForeignKey(d => d.IdImagenPerfil)
                .HasConstraintName("detalles_cuenta_gerente_ibfk_imagen_perfil");
        });

        modelBuilder.Entity<DetallesCuentaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDetallesCuentaUsuario).HasName("PK__detalles__70401F92648A1D51");

            entity.ToTable("detalles_cuenta_usuario");

            entity.Property(e => e.FechaRegistro).HasColumnType("date");

            entity.HasOne(d => d.IdImagenPerfilNavigation).WithMany(p => p.DetallesCuentaUsuarios)
                .HasForeignKey(d => d.IdImagenPerfil)
                .HasConstraintName("detalles_cuenta_usuario_ibfk_imagen_perfil");
        });

        modelBuilder.Entity<DetallesMensaje>(entity =>
        {
            entity.HasKey(e => e.IdDetallesMensaje).HasName("PK__detalles__10722F5E482777DD");

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
            entity.HasKey(e => e.IdDetallesUsuario).HasName("PK__detalles__4892D961D130A1DE");

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
            entity.HasKey(e => e.IdFavoritos).HasName("PK__favorito__085B60776BFB2CBE");

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
            entity.HasKey(e => e.IdGerente).HasName("PK__gerente__06016C454B6C5587");

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
            entity.HasKey(e => e.IdHistorial).HasName("PK__historia__9CC7DBB455B4EF9D");

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
            entity.HasKey(e => e.IdHorario).HasName("PK__horario__1539229B89DF5DD8");

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
            entity.HasKey(e => e.IdImagenPerfil).HasName("PK__imagen_p__32D3A360AB36DBF3");

            entity.ToTable("imagen_perfil");

            entity.Property(e => e.IconoPerfil)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ImagenesMensaje>(entity =>
        {
            entity.HasKey(e => e.IdImagenesMensaje).HasName("PK__imagenes__3446ACF1FF24AE23");

            entity.ToTable("imagenes_mensaje");

            entity.Property(e => e.ImagenMensaje)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ImagenesProducto>(entity =>
        {
            entity.HasKey(e => e.IdImagenesProductos).HasName("PK__imagenes__99684703E299853E");

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
            entity.HasKey(e => e.IdImagenesPublicacion).HasName("PK__imagenes__AA5AE371CC714484");

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
            entity.HasKey(e => e.IdImagenesTiendas).HasName("PK__imagenes__E284A875298BFEEF");

            entity.ToTable("imagenes_tiendas");

            entity.Property(e => e.ImagenTienda)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.ImagenesTienda)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("imagenes_tiendas_ibfk_tienda");
        });

        modelBuilder.Entity<MensajeAdministrador>(entity =>
        {
            entity.HasKey(e => e.IdMensajeAdministrador).HasName("PK__mensaje___8654F5EECB00812A");

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
            entity.HasKey(e => e.IdMensajeGerente).HasName("PK__mensaje___D9B2149B877478AB");

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
            entity.HasKey(e => e.IdMensajeUsuario).HasName("PK__mensaje___8350592BDFBABF16");

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
            entity.HasKey(e => e.IdPenalizacionUsuario).HasName("PK__penaliza__5B75DF6730E3D5F2");

            entity.ToTable("penalizacion_usuario");
        });

        modelBuilder.Entity<PeriodosPredeterminado>(entity =>
        {
            entity.HasKey(e => e.IdApartadoPredeterminado).HasName("PK__periodos__363145418572D049");

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
            entity.HasKey(e => e.IdProductos).HasName("PK__producto__718C7D075659E7FE");

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
            entity.HasKey(e => e.IdPublicacion).HasName("PK__publicac__24F1B7D3AE7231BE");

            entity.ToTable("publicaciones");

            entity.Property(e => e.Contenido).HasColumnType("text");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Publicaciones)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("publicaciones_ibfk_tienda");
        });

        modelBuilder.Entity<RatioUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRatioUsuario).HasName("PK__ratio_us__9359AF44F5794221");

            entity.ToTable("ratio_usuario");
        });

        modelBuilder.Entity<SolicitudesApartado>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__36899CEF46DF2058");

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
            entity.HasKey(e => e.IdTendencia).HasName("PK__tendenci__C139244D5638269D");

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
            entity.HasKey(e => e.IdTienda).HasName("PK__tienda__5A1EB96BDCB9A78D");

            entity.ToTable("tienda");

            entity.Property(e => e.LogoTienda)
                .HasMaxLength(255)
                .IsUnicode(false);
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
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__5B65BF97A4DC3351");

            entity.ToTable("usuarios");

            entity.HasOne(d => d.IdDetallesUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdDetallesUsuario)
                .HasConstraintName("usuarios_ibfk_detalles_usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
