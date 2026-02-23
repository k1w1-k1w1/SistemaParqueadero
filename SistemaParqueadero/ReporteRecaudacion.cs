using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaParqueadero.Models
{
    /// <summary>
    /// DTO para reportes de recaudación
    /// </summary>
    public class ReporteRecaudacion
    {
        public DateTime Fecha { get; set; }
        public int TotalVehiculos { get; set; }
        public int VehiculosVIP { get; set; }
        public int VehiculosEstandar { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal RecaudadoVIP { get; set; }
        public decimal RecaudadoEstandar { get; set; }
        public decimal PromedioHoras { get; set; }
    }
}
