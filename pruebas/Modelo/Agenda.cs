using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
  public  class Agenda
    {
        [Key]
        public int IdAgenda { get; set; }
        public DateTime FechaEvento { get; set; }
        public string Asunto { get; set; }
        public int IdUsuario { get; set; }

        public Usuario Usuario { get; set; }
    }
}
