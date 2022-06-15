using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Validaciones
{
    //La palabra Attribute al final del nombre de la clase es un sufijo para
    //indicarle a visual que es una clase de atributo como el sufijo Controller.
    //De igual forma debemos agregar a la clase que herede de ValidationAttribute
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Primero Verificamos si el value es nulo
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                //Regresamos que la validacion es correcta
                return ValidationResult.Success;
            }

            //Obtenemos la primera letra del value
            var primeraLetra = value.ToString()[0].ToString();

            //Verificamos si la primera letra es distinta a su letra en mayuscula
            if (primeraLetra != primeraLetra.ToUpper())
            {
                //Regresamos el error
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;
        }
    }
}
