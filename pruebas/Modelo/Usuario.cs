using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
 public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string User { get; set; }
        public string Contrasenia { get; set; }
        public string Autorizacion { get; set; }

        public List<Control> Controles { get; set; }

        public List<Agenda> agendas { get; set; }

    }
}
