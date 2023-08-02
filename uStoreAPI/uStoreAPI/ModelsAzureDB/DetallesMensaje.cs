using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class DetallesMensaje
{
    public int IdDetallesMensaje { get; set; }

    public string? Contenido { get; set; }

    public DateTime? FechaMensaje { get; set; }

    public int? IdChat { get; set; }

    public int? IdImagenesMensaje { get; set; }

    public virtual Chat? IdChatNavigation { get; set; }

    public virtual ImagenesMensaje? IdImagenesMensajeNavigation { get; set; }

    public virtual ICollection<MensajeAdministrador> MensajeAdministradors { get; set; } = new List<MensajeAdministrador>();

    public virtual ICollection<MensajeGerente> MensajeGerentes { get; set; } = new List<MensajeGerente>();

    public virtual ICollection<MensajeUsuario> MensajeUsuarios { get; set; } = new List<MensajeUsuario>();
}
