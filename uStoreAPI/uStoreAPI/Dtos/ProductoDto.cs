﻿using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class ProductoDto
    {
        public int IdProductos { get; set; }
        [Required]
        public string? NombreProducto { get; set; }
        [Required]
        public double? PrecioProducto { get; set; }
        [Required]
        public int? CantidadApartado { get; set; }
        [Required]
        public string? Descripcion { get; set; }
        public int? Stock { get; set; }
        [Required]
        public int? IdTienda { get; set; }
        public string? ImageProducto { get; set; }
        public string? ImageProductoThumbNail { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? NombreTienda { get; set; }
        public string? LogoTienda { get; set; }
        public string? LogoTiendaThumbNail { get; set; }
        public string? IsFavorito { get; set; }
    }
}
