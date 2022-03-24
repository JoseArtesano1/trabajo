using pruebas.Controlador;
using pruebas.Modelo;
using System;
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
    public partial class FAgenda : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        int idA;
        public FAgenda()
        {
            InitializeComponent();
            MarcarMes();
            
        }

        private void txtAsunto_Validating(object sender, CancelEventArgs e)
        {
            if (txtAsunto.Text != "")
            {
                if (moduloInicio.IsNumeric(txtAsunto.Text)) 
                { MessageBox.Show("introduce texto"); e.Cancel = true; }
            }
        }

        private void btnAlta_Click(object sender, EventArgs e)
        {
            if (txtAsunto.Text == "") { MessageBox.Show("introduce un asunto"); txtAsunto.Focus(); return; }

            if(!moduloInicio.Existe("select Asunto from pyme.agenda where Asunto='"+ txtAsunto.Text + "';"))
            {
                using (var contexto = new MyDbContext())
                {
                    Agenda agenda = new Agenda
                    {
                        Asunto = txtAsunto.Text.ToLower(),
                        FechaEvento = datetimefecha.Value.Date,
                        IdUsuario = Constants.Id_usuario
                    };
                    contexto.Agendas.Add(agenda);
                    contexto.SaveChanges();

                }
              
                MarcarMes();
            }
            else { MessageBox.Show("el asunto ya existe"); }
            moduloInicio.LimpiarTexto(this);
        }

        private void MarcarMes()
        {
           
            var listadoFechas = moduloInicio.ObtenerAgenda();
            var listadoIndividual = listadoFechas.Where(x => x.IdUsuario == Constants.Id_usuario).ToList();
            if (moduloInicio.ObtenerAutorizacion() == "A")
            { 
                foreach(var item in listadoFechas)
                {   monthcalenAgenda.AddBoldedDate(item.FechaEvento);
                    monthcalenAgenda.UpdateBoldedDates();
                }
            }
            else
            {
                foreach(var item in listadoIndividual)
                {  monthcalenAgenda.AddBoldedDate(item.FechaEvento);
                   monthcalenAgenda.UpdateBoldedDates();
                }

            }
      
        }

        private void ActualizarGrid()
        {
           DateTime fecha = monthcalenAgenda.SelectionStart.Date;

            if (moduloInicio.ObtenerAutorizacion() == "A")
            {                                     
                        datagridAgenda.DataSource = moduloInicio.CargaGridyCombo("select IdAgenda, Nombre,FechaEvento as Fecha,Asunto from pyme.agenda a, pyme.usuarios u where u.IdUsuario= a.IdUsuario and  FechaEvento='" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "';");  
                         datagridAgenda.Columns[0].Visible = false;
                                        
            }
            else
            {                              
                        datagridAgenda.DataSource = moduloInicio.CargaGridyCombo("select IdAgenda, FechaEvento as Fecha,Asunto" +
                    " from pyme.agenda a, pyme.usuarios u where u.IdUsuario= a.IdUsuario and a.IdUsuario= "+ Constants.Id_usuario + " and FechaEvento='" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "';");
                        datagridAgenda.Columns[0].Visible = false;
                 
            }
           
        }

        private void monthcalenAgenda_DateSelected(object sender, DateRangeEventArgs e)
        {
            ActualizarGrid();
            moduloInicio.LimpiarTexto(this);
        }

        private void datagridAgenda_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {            
                if (datagridAgenda.CurrentRow.Cells[0].Value.ToString() != "")
                {
                    idA = int.Parse(datagridAgenda.CurrentRow.Cells[0].Value.ToString());
                    if (moduloInicio.ObtenerAutorizacion() == "A")
                    {                      
                       txtAsunto.Text = datagridAgenda.CurrentRow.Cells[3].Value.ToString(); 
                       datetimefecha.Value =DateTime.Parse( datagridAgenda.CurrentRow.Cells[2].Value.ToString());
                    }
                    else
                    {                        
                        txtAsunto.Text = datagridAgenda.CurrentRow.Cells[2].Value.ToString(); 
                        datetimefecha.Value = DateTime.Parse(datagridAgenda.CurrentRow.Cells[1].Value.ToString());
                    }
                   

                }
                else {MessageBox.Show("selecciona uno o debe dar de alta Asunto"); moduloInicio.LimpiarTexto(this); }
    
            }
            btnAlta.Enabled = false; 
        }


        private void btnmodificar_Click(object sender, EventArgs e)
        {          
            if (txtAsunto.Text!="")
            {
                using (var contexto = new MyDbContext())
                {
                    Agenda agenda = contexto.Agendas.Where(x => x.IdAgenda == idA).FirstOrDefault();
                    agenda.FechaEvento = datetimefecha.Value.Date;
                    agenda.Asunto = moduloInicio.ControlarModificar("select Asunto " +
                        "from pyme.agenda where Asunto='" + txtAsunto.Text.ToLower() + "';", "select Asunto " +
                        "from pyme.agenda where IdAgenda=" + idA + ";", 0, txtAsunto.Text).ToLower();
                    contexto.SaveChanges();
                }
                monthcalenAgenda.RemoveAllBoldedDates();
                monthcalenAgenda.UpdateBoldedDates();
                MarcarMes();
                btnAlta.Enabled = true;
                VaciarDataGrid();
            }
            else{ MessageBox.Show("introduce un asunto"); txtAsunto.Focus(); return; }
                           
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if ( txtAsunto.Text != "")
            {
                if (MessageBox.Show("Este proceso borra el asunto " +
                      datagridAgenda.CurrentRow.Cells[2].Value.ToString().ToUpper() +
                      " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (var contexto = new MyDbContext())
                    {
                        Agenda agenda = contexto.Agendas.Where(x => x.IdAgenda == idA).FirstOrDefault();
                        contexto.Agendas.Remove(agenda);
                        contexto.SaveChanges();
                    }
                    monthcalenAgenda.RemoveAllBoldedDates();
                    monthcalenAgenda.UpdateBoldedDates();
                    MarcarMes();
                    btnAlta.Enabled = true;
                    VaciarDataGrid();
                }
            }
            else { MessageBox.Show("selecciona un Asunto"); }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);
            btnAlta.Enabled = true;
        }

        private void VaciarDataGrid()
        {
            datagridAgenda.DataSource = null;
            datagridAgenda.Refresh();
            moduloInicio.LimpiarTexto(this);
        }
    }
}
