using System.ComponentModel.DataAnnotations;

namespace SistemaParqueadero.API.DTOs
{
    public class DegradarOperadorDto
    {
        [Required]
        public int AdminId { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}