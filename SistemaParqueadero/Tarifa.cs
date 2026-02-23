using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaParqueadero.Models
{
    public static class Tarifa
    {
        // TARIFAS POR HORA
        public const decimal TARIFA_VIP_HORA = 2.00m;
        public const decimal TARIFA_ESTANDAR_HORA = 1.00m;

        // TARIFAS DIARIAS (más de 5 horas)
        public const decimal TARIFA_VIP_DIA = 8.00m;
        public const decimal TARIFA_ESTANDAR_DIA = 5.00m;

        // UMBRAL PARA COBRO DIARIO
        public const decimal HORAS_PARA_TARIFA_DIARIA = 5.0m;

        /// <summary>
        /// Calcula el monto a cobrar según tipo de tarifa y horas
        /// </summary>
        public static decimal CalcularMonto(TipoTarifa tipoTarifa, decimal horas)
        {
            // Redondear horas hacia arriba (fracción de hora cuenta como hora completa)
            decimal horasRedondeadas = Math.Ceiling(horas);

            // Si supera 5 horas, cobrar tarifa diaria
            if (horasRedondeadas > HORAS_PARA_TARIFA_DIARIA)
            {
                return tipoTarifa == TipoTarifa.VIP
                    ? TARIFA_VIP_DIA
                    : TARIFA_ESTANDAR_DIA;
            }
            else
            {
                // Cobrar por hora
                decimal tarifaPorHora = tipoTarifa == TipoTarifa.VIP
                    ? TARIFA_VIP_HORA
                    : TARIFA_ESTANDAR_HORA;

                return horasRedondeadas * tarifaPorHora;
            }
        }

        /// <summary>
        /// Obtiene descripción de la tarifa aplicada
        /// </summary>
        public static string ObtenerDescripcionTarifa(TipoTarifa tipoTarifa, decimal horas)
        {
            decimal horasRedondeadas = Math.Ceiling(horas);

            if (horasRedondeadas > HORAS_PARA_TARIFA_DIARIA)
            {
                return $"Tarifa Diaria {tipoTarifa} (más de 5 horas)";
            }
            else
            {
                return $"Tarifa por Hora {tipoTarifa} ({horasRedondeadas}h)";
            }
        }
    }
}