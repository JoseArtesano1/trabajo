namespace GestionTrabajadores.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class primera : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agenda",
                c => new
                    {
                        IdAgenda = c.Int(nullable: false, identity: true),
                        FechaEvento = c.DateTime(nullable: false, precision: 0),
                        Asunto = c.String(unicode: false),
                        IdUsuario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdAgenda)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        IdUsuario = c.Int(nullable: false, identity: true),
                        Nombre = c.String(unicode: false),
                        User = c.String(unicode: false),
                        Contrasenia = c.String(unicode: false),
                        Autorizacion = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Controls",
                c => new
                    {
                        IdControl = c.Int(nullable: false, identity: true),
                        Fecha_inicio = c.DateTime(nullable: false, precision: 0),
                        IdUsuario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdControl)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Cursoes",
                c => new
                    {
                        IdCurso = c.Int(nullable: false, identity: true),
                        Nombre = c.String(unicode: false),
                        Duracion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdCurso);
            
            CreateTable(
                "dbo.Trabajadors",
                c => new
                    {
                        IdTrabajador = c.Int(nullable: false, identity: true),
                        Dni = c.String(unicode: false),
                        Nombre = c.String(unicode: false),
                        Direccion = c.String(unicode: false),
                        Telefono = c.Int(nullable: false),
                        Nseguridads = c.Long(nullable: false),
                        FechaAlta = c.String(unicode: false),
                        FechaMedico = c.String(unicode: false),
                        FechaDni = c.String(unicode: false),
                        Valor = c.Int(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        IdCategoria = c.Int(nullable: false),
                        FechaPermiso = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.IdTrabajador)
                .ForeignKey("dbo.Categorias", t => t.IdCategoria, cascadeDelete: true)
                .Index(t => t.IdCategoria);
            
            CreateTable(
                "dbo.Categorias",
                c => new
                    {
                        IdCategoria = c.Int(nullable: false, identity: true),
                        Nombre = c.String(unicode: false),
                        Dinero = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdCategoria);
            
            CreateTable(
                "dbo.TrabajadorEpis",
                c => new
                    {
                        IdEpi = c.Int(nullable: false),
                        IdTrabajador = c.Int(nullable: false),
                        FechaEpi = c.String(unicode: false),
                    })
                .PrimaryKey(t => new { t.IdEpi, t.IdTrabajador })
                .ForeignKey("dbo.Epis", t => t.IdEpi, cascadeDelete: true)
                .ForeignKey("dbo.Trabajadors", t => t.IdTrabajador, cascadeDelete: true)
                .Index(t => t.IdEpi)
                .Index(t => t.IdTrabajador);
            
            CreateTable(
                "dbo.Epis",
                c => new
                    {
                        IdEpi = c.Int(nullable: false, identity: true),
                        Nombre = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.IdEpi);
            
            CreateTable(
                "dbo.Extras",
                c => new
                    {
                        IdExtra = c.Int(nullable: false, identity: true),
                        horas = c.Double(nullable: false),
                        IdTrabajador = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdExtra)
                .ForeignKey("dbo.Trabajadors", t => t.IdTrabajador, cascadeDelete: true)
                .Index(t => t.IdTrabajador);
            
            CreateTable(
                "dbo.Periodoes",
                c => new
                    {
                        IdPeriodo = c.Int(nullable: false, identity: true),
                        FechaInicio = c.String(unicode: false),
                        FechaFin = c.String(unicode: false),
                        IdTrabajador = c.Int(nullable: false),
                        IdTipoDia = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdPeriodo)
                .ForeignKey("dbo.TipoDias", t => t.IdTipoDia, cascadeDelete: true)
                .ForeignKey("dbo.Trabajadors", t => t.IdTrabajador, cascadeDelete: true)
                .Index(t => t.IdTrabajador)
                .Index(t => t.IdTipoDia);
            
            CreateTable(
                "dbo.TipoDias",
                c => new
                    {
                        IdTipoDia = c.Int(nullable: false, identity: true),
                        Denominacion = c.String(unicode: false),
                        Importe = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.IdTipoDia);
            
            CreateTable(
                "dbo.Festivoes",
                c => new
                    {
                        IdFestivo = c.Int(nullable: false, identity: true),
                        FechaFestivo = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.IdFestivo);
            
            CreateTable(
                "dbo.CursoControls",
                c => new
                    {
                        Curso_IdCurso = c.Int(nullable: false),
                        Control_IdControl = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Curso_IdCurso, t.Control_IdControl })
                .ForeignKey("dbo.Cursoes", t => t.Curso_IdCurso, cascadeDelete: true)
                .ForeignKey("dbo.Controls", t => t.Control_IdControl, cascadeDelete: true)
                .Index(t => t.Curso_IdCurso)
                .Index(t => t.Control_IdControl);
            
            CreateTable(
                "dbo.TrabajadorControls",
                c => new
                    {
                        Trabajador_IdTrabajador = c.Int(nullable: false),
                        Control_IdControl = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Trabajador_IdTrabajador, t.Control_IdControl })
                .ForeignKey("dbo.Trabajadors", t => t.Trabajador_IdTrabajador, cascadeDelete: true)
                .ForeignKey("dbo.Controls", t => t.Control_IdControl, cascadeDelete: true)
                .Index(t => t.Trabajador_IdTrabajador)
                .Index(t => t.Control_IdControl);
            
            CreateTable(
                "dbo.TrabajadorCursoes",
                c => new
                    {
                        Trabajador_IdTrabajador = c.Int(nullable: false),
                        Curso_IdCurso = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Trabajador_IdTrabajador, t.Curso_IdCurso })
                .ForeignKey("dbo.Trabajadors", t => t.Trabajador_IdTrabajador, cascadeDelete: true)
                .ForeignKey("dbo.Cursoes", t => t.Curso_IdCurso, cascadeDelete: true)
                .Index(t => t.Trabajador_IdTrabajador)
                .Index(t => t.Curso_IdCurso);
            
            CreateTable(
                "dbo.EpiControls",
                c => new
                    {
                        Epi_IdEpi = c.Int(nullable: false),
                        Control_IdControl = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Epi_IdEpi, t.Control_IdControl })
                .ForeignKey("dbo.Epis", t => t.Epi_IdEpi, cascadeDelete: true)
                .ForeignKey("dbo.Controls", t => t.Control_IdControl, cascadeDelete: true)
                .Index(t => t.Epi_IdEpi)
                .Index(t => t.Control_IdControl);
            
            CreateTable(
                "dbo.PeriodoControls",
                c => new
                    {
                        Periodo_IdPeriodo = c.Int(nullable: false),
                        Control_IdControl = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Periodo_IdPeriodo, t.Control_IdControl })
                .ForeignKey("dbo.Periodoes", t => t.Periodo_IdPeriodo, cascadeDelete: true)
                .ForeignKey("dbo.Controls", t => t.Control_IdControl, cascadeDelete: true)
                .Index(t => t.Periodo_IdPeriodo)
                .Index(t => t.Control_IdControl);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Controls", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Periodoes", "IdTrabajador", "dbo.Trabajadors");
            DropForeignKey("dbo.Periodoes", "IdTipoDia", "dbo.TipoDias");
            DropForeignKey("dbo.PeriodoControls", "Control_IdControl", "dbo.Controls");
            DropForeignKey("dbo.PeriodoControls", "Periodo_IdPeriodo", "dbo.Periodoes");
            DropForeignKey("dbo.Extras", "IdTrabajador", "dbo.Trabajadors");
            DropForeignKey("dbo.TrabajadorEpis", "IdTrabajador", "dbo.Trabajadors");
            DropForeignKey("dbo.TrabajadorEpis", "IdEpi", "dbo.Epis");
            DropForeignKey("dbo.EpiControls", "Control_IdControl", "dbo.Controls");
            DropForeignKey("dbo.EpiControls", "Epi_IdEpi", "dbo.Epis");
            DropForeignKey("dbo.TrabajadorCursoes", "Curso_IdCurso", "dbo.Cursoes");
            DropForeignKey("dbo.TrabajadorCursoes", "Trabajador_IdTrabajador", "dbo.Trabajadors");
            DropForeignKey("dbo.TrabajadorControls", "Control_IdControl", "dbo.Controls");
            DropForeignKey("dbo.TrabajadorControls", "Trabajador_IdTrabajador", "dbo.Trabajadors");
            DropForeignKey("dbo.Trabajadors", "IdCategoria", "dbo.Categorias");
            DropForeignKey("dbo.CursoControls", "Control_IdControl", "dbo.Controls");
            DropForeignKey("dbo.CursoControls", "Curso_IdCurso", "dbo.Cursoes");
            DropForeignKey("dbo.Agenda", "IdUsuario", "dbo.Usuarios");
            DropIndex("dbo.PeriodoControls", new[] { "Control_IdControl" });
            DropIndex("dbo.PeriodoControls", new[] { "Periodo_IdPeriodo" });
            DropIndex("dbo.EpiControls", new[] { "Control_IdControl" });
            DropIndex("dbo.EpiControls", new[] { "Epi_IdEpi" });
            DropIndex("dbo.TrabajadorCursoes", new[] { "Curso_IdCurso" });
            DropIndex("dbo.TrabajadorCursoes", new[] { "Trabajador_IdTrabajador" });
            DropIndex("dbo.TrabajadorControls", new[] { "Control_IdControl" });
            DropIndex("dbo.TrabajadorControls", new[] { "Trabajador_IdTrabajador" });
            DropIndex("dbo.CursoControls", new[] { "Control_IdControl" });
            DropIndex("dbo.CursoControls", new[] { "Curso_IdCurso" });
            DropIndex("dbo.Periodoes", new[] { "IdTipoDia" });
            DropIndex("dbo.Periodoes", new[] { "IdTrabajador" });
            DropIndex("dbo.Extras", new[] { "IdTrabajador" });
            DropIndex("dbo.TrabajadorEpis", new[] { "IdTrabajador" });
            DropIndex("dbo.TrabajadorEpis", new[] { "IdEpi" });
            DropIndex("dbo.Trabajadors", new[] { "IdCategoria" });
            DropIndex("dbo.Controls", new[] { "IdUsuario" });
            DropIndex("dbo.Agenda", new[] { "IdUsuario" });
            DropTable("dbo.PeriodoControls");
            DropTable("dbo.EpiControls");
            DropTable("dbo.TrabajadorCursoes");
            DropTable("dbo.TrabajadorControls");
            DropTable("dbo.CursoControls");
            DropTable("dbo.Festivoes");
            DropTable("dbo.TipoDias");
            DropTable("dbo.Periodoes");
            DropTable("dbo.Extras");
            DropTable("dbo.Epis");
            DropTable("dbo.TrabajadorEpis");
            DropTable("dbo.Categorias");
            DropTable("dbo.Trabajadors");
            DropTable("dbo.Cursoes");
            DropTable("dbo.Controls");
            DropTable("dbo.Usuarios");
            DropTable("dbo.Agenda");
        }
    }
}
