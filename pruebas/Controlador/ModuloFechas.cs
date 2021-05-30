using MySql.Data.MySqlClient;
using pruebas.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pruebas.Controlador
{
    
   public class ModuloFechas
    {   
        ModuloInicio moduloInicio = new ModuloInicio();
        string conexion = "server=192.168.1.2;User Id=root; Persist Security Info=True;database=Pyme;password=root";
        public List<Trabajador> milistadotrabajador = new List<Trabajador>();
        public List<string> elementos = new List<string>();
        public string[] misfechas;

        public DateTime ObtenerFechaDate(string fecha)
        {
            DateTime f = DateTime.Parse(fecha);
            return f;
        }

        public string ObtenerFecha(DateTime fecha)
        {
            string fechastring = fecha.ToString("d");
            return fechastring;
        }

        public int TotaldiasNaturales(string fecha1, string fecha2)
        {
            TimeSpan v = TimeSpan.Zero;
            var total = v;

            if (fecha1 != null && fecha2 != null)
            {
                var date = DateTime.Parse(fecha1);
                var date1 = DateTime.Parse(fecha2);
                var total1 = date - date1;
                total = total1;
            }
            else { var total1 = TimeSpan.Zero; total = total1; }

            return total.Days + 1;
        }


        public List<DateTime> FestivosConvertidos()
        {
            List<DateTime> festivos = new List<DateTime>();
            using (var contexto = new MyDbContext())
            {
                var dias = contexto.Festivos.ToList();

                foreach (var item in dias)
                {
                    festivos.Add(ObtenerFechaDate(item.FechaFestivo));
                }
            }
            return festivos;
        }

        public int TotaldiasLaborales(string fechaInicial, string fechaFin, string tipo)
        {
            int f = 0;
            int sd = 0; int totaldias = 0;
            if (tipo.StartsWith("V"))
            {
                if (fechaInicial != null && fechaFin != null)
                {
                    for (var fecha = DateTime.Parse(fechaFin); fecha <= DateTime.Parse(fechaInicial); fecha = fecha.AddDays(1))
                    {
                        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                        { sd++; }

                        if (FestivosConvertidos().Contains(fecha)) { f++; }
                    }

                    
                   var diasFecha = (DateTime.Parse(fechaInicial) - DateTime.Parse(fechaFin));
                    totaldias = (diasFecha.Days + 1) - (sd + f);
                }
                else { totaldias = 0; }
            }

            return totaldias;
        }

        public List<string> FechasWordBaja(  int idTrabaj)
        {            
            using (var contexto = new MyDbContext())
            {
                var dias = (from p in contexto.Periodos
                            join t in contexto.TipoDias
                            on p.IdTipoDia equals t.IdTipoDia
                            where p.IdTrabajador == idTrabaj
                            select new
                            {
                                FechaIn = p.FechaInicio,
                                FechaFinal = p.FechaFin,
                                Tipo = t.Denominacion,
                            }).ToArray();

                foreach (var item in dias)
                {
                    if (item.Tipo.StartsWith("V"))
                    {
                        string uno = item.FechaIn.ToString();  string dos = item.FechaFinal.ToString();
                        string todo = uno + "," + dos;
                        elementos.Add(todo); //para reflejar vacaciones en la baja
                    }
                }
            }
            return elementos;

        }

        public DataTable CargaGridDias(int idTrab)
        {
            var dt = new DataTable();
            dt.Columns.Add("Tipo");
            dt.Columns.Add("Inicio");
            dt.Columns.Add("Fin");
            dt.Columns.Add("Naturales");
            dt.Columns.Add("Laborales");

            DataRow row = dt.NewRow();
            using (var contexto = new MyDbContext())
            {
                var dias = (from p in contexto.Periodos
                            join t in contexto.TipoDias
                            on p.IdTipoDia equals t.IdTipoDia
                            where p.IdTrabajador == idTrab
                            select new
                            {   FechaIn = p.FechaInicio,
                                FechaFinal = p.FechaFin,
                                Tipo = t.Denominacion,
                            }).ToArray();

                foreach (var item in dias)
                {                    
                    dt.Rows.Add(item.Tipo.ToString(), item.FechaIn, item.FechaFinal, TotaldiasNaturales(item.FechaFinal, item.FechaIn).ToString(), TotaldiasLaborales(item.FechaFinal, item.FechaIn, item.Tipo).ToString());
                }
            }
            return dt;
        }

       public DataTable CargaGridAlerta()
        {
            var dt = new DataTable();
            dt.Columns.Add("Nombre");
            dt.Columns.Add("Dni/Nie");
            dt.Columns.Add("Rec_Medico");
            dt.Columns.Add("Fin_Dni/nie");
            dt.Columns.Add("Fin_Permiso");

            DataRow row = dt.NewRow();

         var lista= moduloInicio.TrabajadoresEmpresa().Where(x => x.Activo == true).ToList();  // solo los trabajadores activos

            foreach(var item in lista)
            {
                if(CalculosFechas(item.FechaMedico,365)<60 || CalculosFechas(item.FechaDni,0) < 60 
                    || CalculosFechas(item.FechaPermiso, 0) < 60) //plazo de alerta
                {
                    dt.Rows.Add(item.Nombre.ToString(), item.Dni.ToString(), item.FechaMedico.ToString(), 
                        item.FechaDni.ToString(), item.FechaPermiso.ToString());// llenamos datatable
                    milistadotrabajador.Add(item);
                }
                else { MessageBox.Show("fuera de plazo"); }
            }
            return dt;
        }

        public int CalculosFechas(string fechaSinCalculo, int ndias)
        {
           DateTime fcambiada= ObtenerFechaDate(fechaSinCalculo);
           DateTime fechafin= fcambiada.AddDays(ndias);// a la fecha del trabajador se le suma un plazo
            var dias = fechafin - DateTime.Today; // saber los dias que faltan desde hoy hasta la fecha
           return dias.Days;
        }

        public DateTime CheckfechaModifAlta(MyDbContext contexto, int IdTrabr, DateTime fechaAlta)
        {
            var fechasAlta = contexto.Periodos.Where(x => x.IdTrabajador == IdTrabr).ToList(); //fecha de los periodos
            var fechaExistente = contexto.Trabajadors.Where(x => x.IdTrabajador == IdTrabr).FirstOrDefault().FechaAlta; //fecha guardada
            bool existe = false; int a = 0;
            DateTime fechaCorrecta = ObtenerFechaDate(fechaExistente);

            if (fechasAlta.Count != 0)  // controlar si hay periodos
            {
                while ((!existe) && (a < fechasAlta.Count))
                {
                    if (DateTime.Compare(DateTime.Parse(fechasAlta[a].FechaInicio), fechaAlta) > 0
                        || DateTime.Compare(DateTime.Parse(fechasAlta[a].FechaInicio), fechaAlta) == 0)
                    {
                        if (CheckFechaEpi(contexto, IdTrabr, fechaAlta))
                        { fechaCorrecta = fechaAlta; }

                        else { MessageBox.Show("no se puede cambiar");  return fechaCorrecta = ObtenerFechaDate(fechaExistente); }
                    }
                    else { MessageBox.Show("no se puede cambiar"); existe = false; return fechaCorrecta = ObtenerFechaDate(fechaExistente); }

                    a++;
                }
            }
            else
            {
                if (CheckFechaEpi(contexto, IdTrabr, fechaAlta))
                { fechaCorrecta = fechaAlta; }

                else { MessageBox.Show("no se puede cambiar"); return fechaCorrecta = ObtenerFechaDate(fechaExistente); }

            }
            return fechaCorrecta;
        }


        public bool CheckFechaEpi(MyDbContext Context, int IdTrabr, DateTime fechaAlta)
        {
            var fechaEpis = Context.TrabajadorEpis.Where(x => x.IdTrabajador == IdTrabr).ToList();
            bool coincide = false; int i = 0;

            if (fechaEpis.Count != 0) // controlar si hay fecha de epi
            {
                while ((!coincide) && (i < fechaEpis.Count))
                {
                    if (DateTime.Compare(DateTime.Parse(fechaEpis[i].FechaEpi), fechaAlta) > 0
                       || DateTime.Compare(DateTime.Parse(fechaEpis[i].FechaEpi), fechaAlta) == 0)
                    {
                        coincide = true;
                    }
                    else { return coincide = false; }

                    i++;
                }
            }
            else { coincide = true; }

            return coincide;
        }


        public bool CompararFecha(string sql, DateTime fechaUno)
        {
            string fechaDos = "";
            using (MySqlConnection con = new MySqlConnection(conexion))
            using (MySqlCommand comando = new MySqlCommand(sql, con))
            {
                con.Open();
                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fechaDos = reader.GetString(0);
                    }
                }
            }
            return DateTime.Compare(DateTime.Parse(fechaDos), fechaUno) < 0 ||
                DateTime.Compare(DateTime.Parse(fechaDos), fechaUno) == 0;
        }


        public bool ComprobarPeriodo(DateTime uno, DateTime dos, MyDbContext Context, int IdTrabajador)
        {
            bool coincide = false;
            int i = 0;
            var resultado = Context.Periodos.Where(x => x.IdTrabajador == IdTrabajador).ToList();

             // recorremos los periodos del trabajador
            while ((!coincide) && (i < resultado.Count)) // pq regla de negación y afirmación
            {
                // comparamos las fechas finales e iniciales del trabajador con las fechas  seleccionadas 
                if (DateTime.Compare(DateTime.Parse(resultado[i].FechaInicio), dos) > 0
                    || DateTime.Compare(DateTime.Parse(resultado[i].FechaFin), uno) < 0)
                { coincide = false;}
               
                else
                { coincide = true;}
              i++;
            }
            return coincide;
        }


        public string OpcionDias(DateTime uno, DateTime dos)
        {
            string PeriodoFechas = "";

            if (DateTime.Compare(uno, dos) == 0)
            {return PeriodoFechas = "El DIA " + uno.ToString("dd/MM/yyyy"); }
           
            else
            {return PeriodoFechas = "DEL DIA " + uno.ToString("dd/MM/yyyy")
                    + "AL DIA " + dos.ToString("dd/MM/yyyy"); }
        }


    }
}
