using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Chat
{
    public int IdChat { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int IdMiembro1 { get; set; }

    public string? TypeMiembro1 { get; set; }

    public int IdMiembro2 { get; set; }

    public string? TypeMiembro2 { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
