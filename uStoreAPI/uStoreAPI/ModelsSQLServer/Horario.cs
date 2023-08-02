using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class Horario
{
    public int IdHorario { get; set; }

    public string? Dia { get; set; }

    public string? HorarioApertura { get; set; }

    public string? HorarioCierre { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
