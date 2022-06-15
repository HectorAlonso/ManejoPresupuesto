using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Models
{
    public class ReporteSemanalViewModel
    {
        public decimal Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);

        public decimal Gastos => TransaccionesPorSemana.Sum(x => x.Gastos);

        public decimal Total => Ingresos - Gastos;

        public DateTime FechaReferencia { get; set; }

        public IEnumerable<ResultadoObtenerPorSemana> TransaccionesPorSemana { get; set; }
    }
}
