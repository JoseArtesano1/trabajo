using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
   public class TrabajadorEpi
    {
       
        public int IdTrabajador { get; set; }
        public Trabajador Trabajador { get; set; }
        
        public int IdEpi { get; set; }
        public Epi Epi { get; set; }
        public string FechaEpi { get; set; }
    }
}
