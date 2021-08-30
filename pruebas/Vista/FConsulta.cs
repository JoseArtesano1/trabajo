using pruebas.Controlador;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace pruebas.Vista
{
    public partial class FConsulta : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        ModuloFechas moduloFechas = new ModuloFechas();
      

        public FConsulta()
        {
            InitializeComponent();
            if (moduloInicio.ObtenerAutorizacion() == "A")
            {
                CargaCombo(moduloInicio.consultas2);
            }
            else { CargaCombo(moduloInicio.consultas); }
            GestionControles(false,false,false,false, false);
        }


        private void CargaCombo(string[]lista)
        {           
            foreach(var item in lista)
            {
                cmbConsulta.Items.Add(item);
            }
        }

        
        private void GestionControles(bool cierto, bool cierto1, bool cierto2, bool cierto3, bool cierto4)
        {   cmbCarga.Visible = cierto;
            lblcarga.Visible = cierto1;
            lblTotal.Visible = cierto2;
            cmbYear.Visible = cierto3;
            lblYear.Visible = cierto4;
        }

        private void cmbConsulta_SelectedIndexChanged(object sender, EventArgs e)
        {
            LimpiarCalendario();
            datagridalerta.DataSource = "";
            datagridalerta.Columns.Clear();
            switch (cmbConsulta.SelectedIndex)
            {
                
                case 0:
                    cmbCarga.DataSource=  moduloInicio.CargaGridyCombo("select Nombre from pyme.cursoes");
                    cmbCarga.ValueMember = "Nombre"; 
                    GestionControles(true,true,false,false,false); cmbCarga.SelectedIndex = -1;
                    break;
                case 1:
                    datagridalerta.DataSource = moduloFechas.CargaGridAlerta();
                    MarcarCalendario();
                    GestionControles(false,false,false,false,false); 
                    break;

                case 2:
                    var tabla = moduloInicio.CargaGridyCombo("select t.Nombre, e.horas as Horas, (e.horas*t.Valor) as Total from pyme.trabajadors t, pyme.extras e where t.IdTrabajador= e.IdTrabajador;");
                    datagridalerta.DataSource = tabla;
                    var sumaTotal = tabla.Compute("SUM(Total)", ""); //funcion con nombre de la columna y un filtro
                    lblTotal.Text = sumaTotal.ToString(); GestionControles(false, false, true,false,false);
                    break;

                case 3:
                    //datagridalerta.DataSource = moduloInicio.CargaGridyCombo("select Nombre, FechaInicio as Inicio, FechaFin as Fin, Denominacion, TIMESTAMPDIFF(day,str_to_date(FechaFin,'%d/%m/%Y'), str_to_date(FechaInicio,'%d/%m/%Y')) as Días_Naturales from pyme.periodoes p, pyme.trabajadors t, pyme.tipodias td where p.IdTrabajador = t.IdTrabajador and p.IdTipoDia = td.IdTipoDia; ");
                    cmbYear.DataSource = moduloInicio.years;
                    GestionControles(false, false, false, true, true); cmbYear.SelectedIndex = -1;
                    break;

                case 4:
                    datagridalerta.DataSource = moduloInicio.CargaGridyCombo("select Nombre, Fecha_inicio, IdControl from pyme.usuarios u, pyme.controls c where u.IdUsuario = c.IdUsuario ;");
                    GestionControles(false, false, false, false, false);
                    break;
            }
        }

        private void cmbCarga_SelectedIndexChanged(object sender, EventArgs e)
        {           
            datagridalerta.DataSource = "";
            if (cmbCarga.SelectedIndex != -1)
            {
                datagridalerta.DataSource = moduloInicio.CargaGridyCombo("select t.Nombre, c.Nombre as Curso, c.Duracion from pyme.trabajadors t, pyme.cursoes c, pyme.trabajadorcursoes where IdCurso = Curso_IdCurso and IdTrabajador = Trabajador_IdTrabajador and c.Nombre ='" + cmbCarga.SelectedValue.ToString() + "';");
            }
            else { MessageBox.Show("selecciona un curso"); }

        }

     

        private void MarcarCalendario()
        {            
            foreach (var item in moduloFechas.milistadotrabajador)
            {               
                if(moduloFechas.CalculosFechas(item.FechaMedico, 365) < 60)
                {
                  DateTime fecha = moduloFechas.ObtenerFechaDate(item.FechaMedico);
                  mcalendarioAlerta.AddBoldedDate(fecha.AddDays(365)); //ojo puede variar
                  mcalendarioAlerta.UpdateBoldedDates();
                }
               if(moduloFechas.CalculosFechas(item.FechaDni, 0) < 60)
                {
                    DateTime fecha1 = moduloFechas.ObtenerFechaDate(item.FechaDni);
                    mcalendarioAlerta.AddBoldedDate(fecha1);
                    mcalendarioAlerta.UpdateBoldedDates();
                }
                if (item.FechaPermiso != "")
                {
                    if (moduloFechas.CalculosFechas(item.FechaPermiso, 0) < 60)
                    {
                        DateTime fecha2 = moduloFechas.ObtenerFechaDate(item.FechaPermiso);
                        mcalendarioAlerta.AddBoldedDate(fecha2);
                        mcalendarioAlerta.UpdateBoldedDates();
                    }
                }

            }
        }
       

        private void LimpiarCalendario()
        {
            mcalendarioAlerta.RemoveAllBoldedDates();
            mcalendarioAlerta.UpdateBoldedDates();
            moduloFechas.milistadotrabajador.Clear();
            lblinforma.Text = "";
            lblinforma2.Text="";
            lblInforma3.Text = "";
        }

        private void mcalendarioAlerta_DateSelected(object sender, DateRangeEventArgs e)
        {
            lblinforma.Text = ""; lblinforma2.Text = ""; lblInforma3.Text = "";
            DateTime fecha = mcalendarioAlerta.SelectionStart.Date;  // obtenemos la fecha del calendario
            foreach (var item in moduloFechas.milistadotrabajador)
            {
                if (DateTime.Compare(DateTime.Parse(item.FechaMedico).AddDays(365), fecha) == 0)  // la comparamos con la base de datos
                {
                    lblinforma.Text += " , " + item.Nombre;
                }

                if (DateTime.Compare(DateTime.Parse(item.FechaDni), fecha) == 0)
                {
                    lblinforma2.Text += " , " + item.Nombre;
                }

                if (item.FechaPermiso != "")
                {
                    if (DateTime.Compare(DateTime.Parse(item.FechaPermiso), fecha) == 0)
                    {
                        lblInforma3.Text += " , " + item.Nombre;
                    }
                }
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbYear_SelectedIndexChanged(object sender, EventArgs e)
        {  
            datagridalerta.DataSource = "";
            
            if (cmbYear.SelectedIndex != -1)
            {  datagridalerta.DataSource = moduloFechas.CargaGridDias1(cmbYear.SelectedValue.ToString());
            }
            else { MessageBox.Show("selecciona un año"); }
        }
    }
}
