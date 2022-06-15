using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }


        [Display(Name ="Fecha Transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;

        public decimal Monto { get; set; }

        [Display(Name ="Categoria ")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaId { get; set; }

        [StringLength(maximumLength:1000,ErrorMessage ="La nota no debe de {1} caracteres")]
        public string? Nota { get; set; }

        [Display(Name ="Cuenta")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una Cuenta")]
        public int CuentaId { get; set; }

        [Display(Name = "Tipo de Operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
        //El valor por defecto sera de tipo Ingreso

        public string? Cuenta { get; set; }
        public string? Categoria { get; set; }
    }
}
