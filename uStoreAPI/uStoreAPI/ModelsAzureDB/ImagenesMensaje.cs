using System;
using System.Collections.Generic;

namespace uStoreAPI.ModelsAzureDB;

public partial class ImagenesMensaje
{
    public int IdImagenesMensaje { get; set; }

    public string? ImagenMensaje { get; set; }

    public virtual ICollection<DetallesMensaje> DetallesMensajes { get; set; } = new List<DetallesMensaje>();
}
