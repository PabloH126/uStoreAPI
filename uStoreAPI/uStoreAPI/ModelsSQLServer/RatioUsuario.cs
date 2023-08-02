using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class RatioUsuario
{
    public int IdRatioUsuario { get; set; }

    public int? ApartadosExitosos { get; set; }

    public int? ApartadosFallidos { get; set; }

    public virtual ICollection<DetallesUsuario> DetallesUsuarios { get; set; } = new List<DetallesUsuario>();

    public virtual ICollection<SolicitudesApartado> SolicitudesApartados { get; set; } = new List<SolicitudesApartado>();
}
