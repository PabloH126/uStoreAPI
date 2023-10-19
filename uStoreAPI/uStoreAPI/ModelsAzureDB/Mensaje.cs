using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class Mensaje
{
    public int IdMensaje { get; set; }

    public DateTime? FechaMensaje { get; set; }

    public string? Contenido { get; set; }

    public bool IsImage { get; set; }

    public int IdRemitente { get; set; }

    public bool IsRead { get; set; }

    public int IdChat { get; set; }

    public virtual Chat IdChatNavigation { get; set; } = null!;
}
