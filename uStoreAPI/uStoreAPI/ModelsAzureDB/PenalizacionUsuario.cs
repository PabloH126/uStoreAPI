using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class PenalizacionUsuario
{
    public int IdPenalizacionUsuario { get; set; }

    public TimeSpan? TiempoPenalizacion { get; set; }

    public int? CantidadPenalizaciones { get; set; }

    public virtual ICollection<DetallesUsuario> DetallesUsuarios { get; set; } = new List<DetallesUsuario>();
}
