using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdDetallesUsuario { get; set; }

    public virtual ICollection<AlertaApartado> AlertaApartados { get; set; } = new List<AlertaApartado>();

    public virtual ICollection<ApartadoActivo> ApartadoActivos { get; set; } = new List<ApartadoActivo>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual ICollection<CuentaUsuario> CuentaUsuarios { get; set; } = new List<CuentaUsuario>();

    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

    public virtual ICollection<Historial> Historials { get; set; } = new List<Historial>();

    public virtual DetallesUsuario? IdDetallesUsuarioNavigation { get; set; }

    public virtual ICollection<MensajeUsuario> MensajeUsuarios { get; set; } = new List<MensajeUsuario>();

    public virtual ICollection<SolicitudesApartado> SolicitudesApartados { get; set; } = new List<SolicitudesApartado>();
}
