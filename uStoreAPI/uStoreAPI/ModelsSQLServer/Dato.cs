using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsSQLServer;

public partial class Dato
{
    public int IdDatos { get; set; }

    public string? PrimerNombre { get; set; }

    public string? PrimerApellido { get; set; }

    public virtual ICollection<DetallesAdministrador> DetallesAdministradors { get; set; } = new List<DetallesAdministrador>();

    public virtual ICollection<DetallesUsuario> DetallesUsuarios { get; set; } = new List<DetallesUsuario>();

    public virtual ICollection<Gerente> Gerentes { get; set; } = new List<Gerente>();
}
