using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
   public class Extra
    {
        [Key]
        public int IdExtra { get; set; }
     
        public double horas { get; set; }

        public int IdTrabajador { get; set; }

        public Trabajador Trabajadores { get; set; }


    }
}
