namespace uStoreAPI.Dtos
{
    public class MensajeDto
    {
        public int IdMensaje { get; set; }

        public DateTime? FechaMensaje { get; set; }

        public string? Contenido { get; set; }

        public bool IsImage { get; set; }

        public int IdRemitente { get; set; }

        public bool IsRead { get; set; }

        public int IdChat { get; set; }
        public bool isRecieved {  get; set; }
    }
}
