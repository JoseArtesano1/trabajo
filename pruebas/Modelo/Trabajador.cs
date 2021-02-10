using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Modelo
{
   public class Trabajador
    {
        [Key]
        public int IdTrabajador { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int Telefono { get; set; }
        public long Nseguridads { get; set; }

        public string FechaAlta { get; set; }
        public string FechaMedico { get; set; }

        public string FechaDni { get; set; }

        public int Valor { get; set; }

        public bool Activo { get; set; }

        public int IdCategoria { get; set; }

        public Categoria Categoria { get; set; }

        public List<Extra>LasExtras { get; set; }

        public List<Periodo> Periodos { get; set; }

        public List<Curso> CursosTrabajador { get; set; }

        public List<TrabajadorEpi> EpiTrabajador { get; set; }

        public List<Control> ControlTrabajador { get; set; }

    }
}
