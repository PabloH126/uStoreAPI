﻿using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class DetallesUsuario
{
    public int IdDetallesUsuario { get; set; }

    public string? Estado { get; set; }

    public int? IdPenalizacionUsuario { get; set; }

    public int? IdDatos { get; set; }

    public int? ApartadosExitosos { get; set; }

    public int? ApartadosFallidos { get; set; }

    public virtual Dato? IdDatosNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
