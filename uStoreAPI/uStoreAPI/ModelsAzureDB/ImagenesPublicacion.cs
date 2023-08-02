using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class ImagenesPublicacion
{
    public int IdImagenesPublicacion { get; set; }

    public string? ImagenPublicacion { get; set; }

    public int? IdPublicacion { get; set; }

    public virtual Publicacione? IdPublicacionNavigation { get; set; }
}
