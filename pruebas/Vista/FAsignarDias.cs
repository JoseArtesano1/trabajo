using pruebas.Controlador;
using pruebas.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using objWord = Microsoft.Office.Interop.Word;

namespace pruebas.Vista
{
    public partial class FAsignarDias : Form
    {
        ModuloFechas moduloFechas = new ModuloFechas();
        ModuloTexto moduloTexto = new ModuloTexto();
        ModuloInicio moduloInicio = new ModuloInicio();
        string fInicial, fFinal;
        int idcont, idTrab;
        DateTime fechaInicio, fechaFin;
       
        
        public FAsignarDias()
        {
            InitializeComponent();
            CargaCombos();
            idcont = moduloInicio.ObtenerIdControl(Constants.Id_usuario).Last().IdControl;
            checkBoxCalculo.Checked = false;
            grupBoxCalculo.Visible = false;
        }

        private void CargaCombos()
        {
            cmbTrabajador.DataSource = moduloInicio.CargaGridyCombo("select Nombre from pyme.trabajadors where Activo=true");
            cmbTrabajador.ValueMember = "Nombre";
            cmbTipodia.DataSource = moduloInicio.CargaGridyCombo("select Denominacion from pyme.tipodias");
            cmbTipodia.ValueMember = "Denominacion";
            cmbTrabajador.SelectedIndex = -1;
            cmbTipodia.SelectedIndex = -1;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

        private void btnAlta_Click(object sender, EventArgs e)
        {
             fechaInicio = datepickInicio.Value.Date;
             fechaFin = datepickFin.Value.Date;
            string fIni = moduloFechas.ObtenerFecha(fechaInicio);
            string fFin = moduloFechas.ObtenerFecha(fechaFin);
            

            if (cmbTrabajador.SelectedIndex == -1) { MessageBox.Show("Selecciona trabajador"); cmbTrabajador.Focus(); return; }
            if (cmbTipodia.SelectedIndex == -1) { MessageBox.Show("Selecciona tipo"); cmbTipodia.Focus(); return; }
            if (moduloTexto.isFileOpen(moduloTexto.ruta)) { MessageBox.Show("cierre el documento"); return; }

            int idtp = moduloInicio.ObtenerId("select IdTipoDia from pyme.tipodias where Denominacion='" + cmbTipodia.SelectedValue.ToString() + "';");
            idTrab = moduloInicio.ObtenerId("select IdTrabajador from pyme.trabajadors where Nombre='" + cmbTrabajador.SelectedValue.ToString() + "';");
            ActualizarGrid(idTrab);

            if (moduloFechas.CompararFecha("select FechaAlta from pyme.trabajadors where IdTrabajador=" + idTrab+";",fechaInicio))
            {
                if (DateTime.Compare(fechaInicio, fechaFin) < 0 || DateTime.Compare(fechaInicio, fechaFin) == 0)
                {                                          
                    using (var contexto = new MyDbContext())
                    {
                        if (moduloInicio.Existe("select IdPeriodo from pyme.periodoes where IdTrabajador=" + idTrab + ";"))
                        {
                            if (!moduloFechas.ComprobarPeriodo(fechaInicio.Date, fechaFin.Date, contexto, idTrab))
                            {
                                Periodo periodo = new Periodo
                                {
                                    FechaInicio = moduloFechas.ObtenerFecha(fechaInicio),
                                    FechaFin = moduloFechas.ObtenerFecha(fechaFin),
                                    IdTrabajador = idTrab,
                                    IdTipoDia = idtp
                                };

                                contexto.Periodos.Add(periodo);
                                contexto.SaveChanges();

                                int idp = contexto.Periodos.Where(x => x.IdTrabajador == idTrab && x.FechaInicio ==
                                fIni && x.FechaFin == fFin).FirstOrDefault().IdPeriodo;
                                moduloInicio.OperarSql("Insert into pyme.periodocontrols (Periodo_IdPeriodo, Control_IdControl) values(" + idp + "," + idcont + ");"); //ojo
                            }
                            else { MessageBox.Show("periódo existe"); }
                           
                        }else 
                        {
                            Periodo periodo = new Periodo
                            {
                                FechaInicio = moduloFechas.ObtenerFecha(fechaInicio),
                                FechaFin = moduloFechas.ObtenerFecha(fechaFin),
                                IdTrabajador = idTrab,
                                IdTipoDia = idtp
                            };
                            contexto.Periodos.Add(periodo);
                            contexto.SaveChanges();

                            int idp = contexto.Periodos.Where(x => x.IdTrabajador == idTrab && x.FechaInicio == 
                            fIni && x.FechaFin == fFin).FirstOrDefault().IdPeriodo;
                            moduloInicio.OperarSql("Insert into pyme.periodocontrols (Periodo_IdPeriodo, Control_IdControl) values(" + idp + "," + idcont + ");"); //ojo
                        }
                       
                        ActualizarGrid(idTrab);
                        if (cmbTipodia.SelectedValue.ToString().StartsWith("V")) //generar solo vacaciones
                        {  moduloTexto.GenerarWordDias(cmbTrabajador.SelectedValue.ToString(), fechaInicio, fechaFin);}
                                         
                    }
                }
                else { MessageBox.Show("la fecha del inicio debe ser inferior a la fecha final"); }
            }
            else { MessageBox.Show("ya existe este periodo"); }
            moduloInicio.LimpiarTexto(this); cmbTipodia.SelectedIndex = -1;
        }



        private void ActualizarGrid( int idt)
        {
            datagridDias.DataSource = moduloFechas.CargaGridDias(idt);
        }

        

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);  moduloInicio.LimpiarComboyCheck(this);//cmbTipodia.SelectedIndex = -1; 
            datagridDias.DataSource = "";
            
        }

        private void btnCalculo_Click(object sender, EventArgs e)
        {
            if (DateTime.Compare(datePickStart.Value.Date, datePickFinish.Value.Date) < 0)
            {
                var dias = (int)(datePickFinish.Value - datePickStart.Value).TotalDays;
                var vacasNaturales = (dias * 30) / 365.00;
                lblNatural.Text = Math.Round(vacasNaturales, 2).ToString() + " días naturales";
               
                var vacasLaborales = (dias * 21) / 365.00;
                lblLaboral.Text = Math.Round(vacasLaborales, 2).ToString() + " días laborales";

            }
            else
            {
                MessageBox.Show("la fecha del inicio debe ser inferior a la fecha final");
            }
        }

        private void checkBoxCalculo_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCalculo.Checked)
            {  grupBoxCalculo.Visible = true;

            }
            else
            {
                grupBoxCalculo.Visible = false;
                datePickStart.Value = DateTime.Today;
                datePickFinish.Value = DateTime.Today;
                lblNatural.Text = "";
                lblLaboral.Text = "";
            }
        }

        private void cmbTrabajador_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTrabajador.SelectedIndex != -1)
            {                
                idTrab = moduloInicio.ObtenerId("select IdTrabajador from pyme.trabajadors where Nombre='" + cmbTrabajador.SelectedValue.ToString() + "';");
                ActualizarGrid(idTrab);
            }
            moduloInicio.LimpiarTexto(this); cmbTipodia.SelectedIndex = -1;
        }

        private void datagridDias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (datagridDias.CurrentRow.Cells[0].Value.ToString() != "")
                { fInicial =  datagridDias.CurrentRow.Cells[1].Value.ToString();
                  fFinal=  datagridDias.CurrentRow.Cells[2].Value.ToString();
                }
                else { MessageBox.Show("No existen periódos, se deben asignar"); }
                   
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (fInicial!= null) 
            {
               int id= moduloInicio.ObtenerId("select IdPeriodo from pyme.periodoes where FechaInicio='" + fInicial+"'and FechaFin='"+fFinal+"';");
                if (MessageBox.Show("Este proceso borra el periódo " +
                       datagridDias.CurrentRow.Cells[0].Value.ToString().ToUpper() + 
                       datagridDias.CurrentRow.Cells[1].Value.ToString()+ datagridDias.CurrentRow.Cells[2].Value.ToString()+
                       " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (var contexto = new MyDbContext())
                    {
                        moduloInicio.OperarSql("delete from pyme.periodocontrols where Periodo_IdPeriodo=" + id + ";");
                        Periodo periodo = contexto.Periodos.Where(x => x.IdPeriodo == id).FirstOrDefault();
                        contexto.Periodos.Remove(periodo);
                        contexto.SaveChanges();

                    }
                    ActualizarGrid(idTrab);
                }
            }
            else { MessageBox.Show("selecciona un periódo"); }
        }
    }
}
