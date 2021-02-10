using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
 public class Epi
    {
        [Key]
        public int IdEpi { get; set; }
        public string Nombre { get; set; }

        public List<TrabajadorEpi> TrabajadoresEpi { get; set; }
        public List<Control> ControlEpi { get; set; }
    }
}
