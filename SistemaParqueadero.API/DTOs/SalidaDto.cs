using System.ComponentModel.DataAnnotations;

namespace SistemaParqueadero.API.DTOs
{
    public class SalidaDto
    {
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } = null!;
        // Ej: "Efectivo", "Tarjeta", "Transferencia"

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public bool ConfirmarCierre { get; set; } = true;
    }
}