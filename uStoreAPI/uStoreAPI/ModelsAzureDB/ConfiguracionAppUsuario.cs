using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class ConfiguracionAppUsuario
{
    public int IdConfiguracion { get; set; }

    public int IdUsuario { get; set; }

    public bool? Notificaciones { get; set; }

    public bool? Favoritos { get; set; }

    public bool? Sugerencias { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
