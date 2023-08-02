using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class CentroComercial
{
    public int IdCentroComercial { get; set; }

    public string? Imagen { get; set; }

    public virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();
}
