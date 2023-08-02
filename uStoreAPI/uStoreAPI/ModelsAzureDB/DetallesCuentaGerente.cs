using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class DetallesCuentaGerente
{
    public int IdDetallesCuentaGerente { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdImagenPerfil { get; set; }

    public virtual ICollection<CuentaGerente> CuentaGerentes { get; set; } = new List<CuentaGerente>();

    public virtual ImagenPerfil? IdImagenPerfilNavigation { get; set; }
}
