namespace uStoreAPI.Dtos
{
    public class PenalizacionUsuarioDto
    {
        public int IdPenalizacion { get; set; }

        public DateTime? InicioPenalizacion { get; set; }

        public DateTime? FinPenalizacion { get; set; }

        public int? IdUsuario { get; set; }
    }
}
