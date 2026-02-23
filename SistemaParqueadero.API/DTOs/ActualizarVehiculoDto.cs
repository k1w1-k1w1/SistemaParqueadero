using System.ComponentModel.DataAnnotations;

namespace SistemaParqueadero.API.DTOs
{
    public class ActualizarVehiculoDto
    {
        [Required, StringLength(10)]
        public string Placa { get; set; } = null!;

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }

        [StringLength(20)]
        public string? Color { get; set; }
    }
}