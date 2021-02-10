using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
   public class Control
    {
        [Key]
        public int IdControl { get; set; }
        public DateTime Fecha_inicio { get; set; }
       
        public int IdUsuario { get; set; }

        public Usuario Usuario { get; set; }

        public List<Trabajador> TrabajadoresCtl { get; set; }

        public List<Curso> CCurso { get; set; }

        public List<Epi> CEpi { get; set; }

        public List<Periodo> CPeriodo { get; set; }
    }
}
