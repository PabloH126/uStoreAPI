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

    public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }

    public virtual DbSet<AlertaApartado> AlertaApartados { get; set; }

    public virtual DbSet<CalificacionProducto> CalificacionProductos { get; set; }

    public virtual DbSet<CalificacionTiendum> CalificacionTienda { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<CategoriasProducto> CategoriasProductos { get; set; }

    public virtual DbSet<CategoriasTienda> CategoriasTiendas { get; set; }

    public virtual DbSet<CentroComercial> CentroComercials { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ComentariosProducto> ComentariosProductos { get; set; }

    public virtual DbSet<ComentariosTienda> ComentariosTiendas { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<CuentaAdministrador> CuentaAdministradors { get; set; }

    public virtual DbSet<CuentaGerente> CuentaGerentes { get; set; }

    public virtual DbSet<CuentaUsuario> CuentaUsuarios { get; set; }

    public virtual DbSet<Dato> Datos { get; set; }

    public virtual DbSet<DetallesAdministrador> DetallesAdministradors { get; set; }

    public virtual DbSet<DetallesCuentaAdministrador> DetallesCuentaAdministradors { get; set; }

    public virtual DbSet<DetallesCuentaGerente> DetallesCuentaGerentes { get; set; }

    public virtual DbSet<DetallesCuentaUsuario> DetallesCuentaUsuarios { get; set; }

    public virtual DbSet<DetallesUsuario> DetallesUsuarios { get; set; }

    public virtual DbSet<Favorito> Favoritos { get; set; }

    public virtual DbSet<Gerente> Gerentes { get; set; }

    public virtual DbSet<Hash> Hashes { get; set; }

    public virtual DbSet<Historial> Historials { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<ImagenPerfil> ImagenPerfils { get; set; }

    public virtual DbSet<ImagenesProducto> ImagenesProductos { get; set; }

    public virtual DbSet<ImagenesTienda> ImagenesTiendas { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobParameter> JobParameters { get; set; }

    public virtual DbSet<JobQueue> JobQueues { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }

    public virtual DbSet<PenalizacionUsuario> PenalizacionUsuarios { get; set; }

    public virtual DbSet<PeriodosPredeterminado> PeriodosPredeterminados { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Publicacione> Publicaciones { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<SolicitudesApartado> SolicitudesApartados { get; set; }

    public virtual DbSet<State> States { get; set; }

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

        modelBuilder.Entity<AggregatedCounter>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK_HangFire_CounterAggregated");

            entity.ToTable("AggregatedCounter", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
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

        modelBuilder.Entity<CalificacionProducto>(entity =>
        {
            entity.HasKey(e => e.IdCalificacionProducto).HasName("PK__califica__EA5510DE2770903B");

            entity.ToTable("calificacion_producto");

            entity.HasIndex(e => new { e.IdProductos, e.IdUsuario }, "UQ__califica__143A26FFED97307D").IsUnique();

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.CalificacionProductos)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("FK__calificac__IdPro__22FF2F51");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CalificacionProductos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__calificac__IdUsu__23F3538A");
        });

        modelBuilder.Entity<CalificacionTiendum>(entity =>
        {
            entity.HasKey(e => e.IdCalificacionTienda).HasName("PK__califica__5CDFD1C39FC8126B");

            entity.ToTable("calificacion_tienda");

            entity.HasIndex(e => new { e.IdTienda, e.IdUsuario }, "UQ__califica__3FA8E29398E07191").IsUnique();

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.CalificacionTienda)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK__calificac__IdTie__1E3A7A34");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CalificacionTienda)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__calificac__IdUsu__1F2E9E6D");
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
            entity.HasKey(e => e.IdChat).HasName("PK__chat__3817F38C0DA7FF04");

            entity.ToTable("chat");

            entity.HasIndex(e => new { e.IdMiembro1, e.TypeMiembro1, e.IdMiembro2, e.TypeMiembro2 }, "Unique_Chat_Combination").IsUnique();

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.TypeMiembro1)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TypeMiembro2)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK_Chats_IdTienda");
        });

        modelBuilder.Entity<ComentariosProducto>(entity =>
        {
            entity.HasKey(e => e.IdComentarioProducto).HasName("PK__comentar__A2E1FF5AEC6D261F");

            entity.ToTable("comentarios_productos");

            entity.Property(e => e.Comentario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FechaComentario).HasColumnType("datetime");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ComentariosProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comentarios_productos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ComentariosProductos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comentarios_productos_usuarios");
        });

        modelBuilder.Entity<ComentariosTienda>(entity =>
        {
            entity.HasKey(e => e.IdComentarioTienda).HasName("PK__comentar__164286A018EBD231");

            entity.ToTable("comentarios_tiendas");

            entity.Property(e => e.Comentario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FechaComentario).HasColumnType("datetime");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.ComentariosTienda)
                .HasForeignKey(d => d.IdTienda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comentarios_tiendas");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ComentariosTienda)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comentarios_tiendas_usuarios");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_Counter");

            entity.ToTable("Counter", "HangFire");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
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

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
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

        modelBuilder.Entity<DetallesUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDetallesUsuario).HasName("PK__detalles__4892D961D130A1DE");

            entity.ToTable("detalles_usuario");

            entity.Property(e => e.ApartadosExitosos).HasDefaultValueSql("((0))");
            entity.Property(e => e.ApartadosFallidos).HasDefaultValueSql("((0))");
            entity.Property(e => e.Estado)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDatosNavigation).WithMany(p => p.DetallesUsuarios)
                .HasForeignKey(d => d.IdDatos)
                .HasConstraintName("detalles_usuario_ibfk_datos");
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

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Field }).HasName("PK_HangFire_Hash");

            entity.ToTable("Hash", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Field).HasMaxLength(100);
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

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Job");

            entity.ToTable("Job", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName").HasFilter("([StateName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(20);
        });

        modelBuilder.Entity<JobParameter>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Name }).HasName("PK_HangFire_JobParameter");

            entity.ToTable("JobParameter", "HangFire");

            entity.Property(e => e.Name).HasMaxLength(40);

            entity.HasOne(d => d.Job).WithMany(p => p.JobParameters)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_JobParameter_Job");
        });

        modelBuilder.Entity<JobQueue>(entity =>
        {
            entity.HasKey(e => new { e.Queue, e.Id }).HasName("PK_HangFire_JobQueue");

            entity.ToTable("JobQueue", "HangFire");

            entity.Property(e => e.Queue).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FetchedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_List");

            entity.ToTable("List", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.IdMensaje).HasName("PK__mensaje__E4D2A47F757B4B08");

            entity.ToTable("mensaje");

            entity.Property(e => e.FechaMensaje).HasColumnType("datetime");
            entity.Property(e => e.TypeRemitente)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdChatNavigation).WithMany(p => p.Mensajes)
                .HasForeignKey(d => d.IdChat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IdChat");
        });

        modelBuilder.Entity<PenalizacionUsuario>(entity =>
        {
            entity.HasKey(e => e.IdPenalizacion).HasName("PK__penaliza__85528AEABB18061B");

            entity.ToTable("penalizacion_usuario");

            entity.Property(e => e.FinPenalizacion).HasColumnType("datetime");
            entity.Property(e => e.IdJob).HasMaxLength(50);
            entity.Property(e => e.InicioPenalizacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PenalizacionUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__penalizac__IdUsu__0FB750B3");
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
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
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
            entity.Property(e => e.FechaPublicacion).HasColumnType("date");
            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCentroComercialNavigation).WithMany(p => p.Publicaciones)
                .HasForeignKey(d => d.IdCentroComercial)
                .HasConstraintName("FK_publicaciones_centro_comercial");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Publicaciones)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("publicaciones_ibfk_tienda");
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK_HangFire_Schema");

            entity.ToTable("Schema", "HangFire");

            entity.Property(e => e.Version).ValueGeneratedNever();
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Server");

            entity.ToTable("Server", "HangFire");

            entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

            entity.Property(e => e.Id).HasMaxLength(200);
            entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Value }).HasName("PK_HangFire_Set");

            entity.ToTable("Set", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(256);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<SolicitudesApartado>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__36899CEF46DF2058");

            entity.ToTable("solicitudes_apartado");

            entity.Property(e => e.FechaApartado).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitud).HasColumnType("datetime");
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");
            entity.Property(e => e.IdJob).HasMaxLength(50);
            entity.Property(e => e.PeriodoApartado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StatusSolicitud)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProductosNavigation).WithMany(p => p.SolicitudesApartados)
                .HasForeignKey(d => d.IdProductos)
                .HasConstraintName("solicitudes_apartado_ibfk_productos");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.SolicitudesApartados)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK_tienda_solicitudes_apartado");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.SolicitudesApartados)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("solicitudes_apartado_ibfk_usuarios");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Id }).HasName("PK_HangFire_State");

            entity.ToTable("State", "HangFire");

            entity.HasIndex(e => e.CreatedAt, "IX_HangFire_State_CreatedAt");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.Job).WithMany(p => p.States)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_State_Job");
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
