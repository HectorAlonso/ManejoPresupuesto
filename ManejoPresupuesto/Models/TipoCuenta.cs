using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta /*: IValidatableObject*/
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }

        public int Orden { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    //Si el nombre noes nulo
        //    if(Nombre != null && Nombre.Length > 0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();

        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            //Indicamos el mensaje de Error y el Campo al cual le pertenece el error de validacion
        //            yield return new ValidationResult("La primera letra debe ser mayuscula", new[] {nameof(Nombre)});

        //            //Sino le indicamos el nombre, la clase solo mostrara el error en cualquier lado
        //        }
        //    }
        //}
    }
}
