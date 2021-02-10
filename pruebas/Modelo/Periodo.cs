using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
   public class Periodo
    {
        [Key]
        public int IdPeriodo { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }

        public int IdTrabajador { get; set; }

        public Trabajador Trabajador { get; set; }

        public int IdTipoDia { get; set; }

        public TipoDia TipoDia { get; set; }

        public List<Control> ControlPdo { get; set; }
    }
}
