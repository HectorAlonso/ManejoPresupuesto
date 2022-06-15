using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizarViewModel : TransaccionCreacionViewModel
    {
        public decimal MontoAnterior { get; set; }

        public int CuentaAnteriorId { get; set; }

        public string  urlRetorno { get; set; }
    }
}
