using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class ImagenPerfil
{
    public int IdImagenPerfil { get; set; }

    public string? IconoPerfil { get; set; }

    public virtual ICollection<DetallesCuentaAdministrador> DetallesCuentaAdministradors { get; set; } = new List<DetallesCuentaAdministrador>();

    public virtual ICollection<DetallesCuentaGerente> DetallesCuentaGerentes { get; set; } = new List<DetallesCuentaGerente>();

    public virtual ICollection<DetallesCuentaUsuario> DetallesCuentaUsuarios { get; set; } = new List<DetallesCuentaUsuario>();
}
