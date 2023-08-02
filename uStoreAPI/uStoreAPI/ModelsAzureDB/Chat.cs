using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Chat
{
    public int IdChat { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdGerente { get; set; }

    public int? IdAdministrador { get; set; }

    public virtual ICollection<DetallesMensaje> DetallesMensajes { get; set; } = new List<DetallesMensaje>();

    public virtual AdministradorTiendum? IdAdministradorNavigation { get; set; }

    public virtual Gerente? IdGerenteNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
