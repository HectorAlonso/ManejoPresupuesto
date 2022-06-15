using ManejoPresupuesto.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Models
{
    public class Cuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength:50)]
        public string Nombre { get; set; }

        [Display(Name ="Tipo de Cuenta")]
        [Required]
        public int TipoCuentaId { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [StringLength(maximumLength:1000)]
        [Required]
        public string Descripcion { get; set; }


        public string? TipoCuenta { get; set; }
    }
}
