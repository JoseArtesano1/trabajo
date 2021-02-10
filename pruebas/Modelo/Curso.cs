using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
   public class Curso
    {
        [Key]
        public int IdCurso { get; set; }
        public string Nombre { get; set; }
        public int Duracion { get; set; }

        public List<Trabajador> TrabajadoresCurso { get; set; }

        public List<Control> ControlCurso { get; set; }
    }
}
