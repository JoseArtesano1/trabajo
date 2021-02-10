using MySql.Data.EntityFramework;
using pruebas.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebas.Controlador
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MyDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
       
        public DbSet<Control> Controls { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Trabajador> Trabajadors { get; set; }

        public DbSet<Extra> Extras { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<TipoDia> TipoDias { get; set; }

        public DbSet<Periodo> Periodos { get; set; }

        public DbSet<Epi> Epis { get; set; }
        public DbSet<TrabajadorEpi> TrabajadorEpis { get; set; }

        public DbSet<Festivo> Festivos { get; set; }

        public DbSet<Agenda> Agendas { get; set; }
        public MyDbContext() : base("name=DefaultConnection")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TrabajadorEpi>().HasKey(t => new { t.IdEpi, t.IdTrabajador });

        }
    }

    
}
