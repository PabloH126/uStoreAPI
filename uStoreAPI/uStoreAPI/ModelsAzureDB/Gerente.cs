using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Gerente
{
    public int IdGerente { get; set; }

    public int? IdDatos { get; set; }

    public int? IdAdministrador { get; set; }

    public int? IdTienda { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<CuentaGerente> CuentaGerentes { get; set; } = new List<CuentaGerente>();

    public virtual AdministradorTiendum? IdAdministradorNavigation { get; set; }

    public virtual Dato? IdDatosNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual ICollection<MensajeGerente> MensajeGerentes { get; set; } = new List<MensajeGerente>();
}
