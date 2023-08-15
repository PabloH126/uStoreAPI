using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class RecoverDto
    {
        [Required]
        public string? email { get; set; }
    }
}
