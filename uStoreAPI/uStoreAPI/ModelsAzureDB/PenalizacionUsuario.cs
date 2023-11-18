using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class PenalizacionUsuario
{
    public int IdPenalizacion { get; set; }

    public DateTime? InicioPenalizacion { get; set; }

    public DateTime? FinPenalizacion { get; set; }

    public int? IdUsuario { get; set; }

    public string? IdJob { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
