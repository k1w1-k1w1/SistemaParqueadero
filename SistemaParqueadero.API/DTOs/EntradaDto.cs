using System.ComponentModel.DataAnnotations;
using SistemaParqueadero.Models;

namespace SistemaParqueadero.API.DTOs
{
    public class EntradaDto
    {
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = null!;

        [Required]
        public TipoTarifa TipoTarifa { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}