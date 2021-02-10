using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
  public  class Festivo
    {
        [Key]
        public int IdFestivo { get; set; }
        public string FechaFestivo { get; set; }


    }
}
