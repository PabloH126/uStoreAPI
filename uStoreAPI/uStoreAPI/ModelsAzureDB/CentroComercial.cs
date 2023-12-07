using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class CentroComercial
{
    public int IdCentroComercial { get; set; }

    public string? IconoCentroComercial { get; set; }

    public string? ImagenCentroComercial { get; set; }

    public string? NombreCentroComercial { get; set; }

    public string? HorarioCentroComercial { get; set; }

    public string? DireccionCentroComercial { get; set; }

    public string? IconoCentroComercialThumbNail { get; set; }

    public string? ImagenCentroComercialThumbNail { get; set; }

    public virtual ICollection<Publicacione> Publicaciones { get; set; } = new List<Publicacione>();

    public virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();
}
