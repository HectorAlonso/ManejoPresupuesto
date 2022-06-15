using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string EmailNormalizado { get; set; }

        public string PasswordHash { get; set; }
    }
}
