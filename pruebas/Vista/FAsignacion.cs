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
    public partial class FAsignacion : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        ModuloFechas moduloFechas = new ModuloFechas();
        ModuloTexto moduloTexto = new ModuloTexto();
        int idtrabajador;
        int id1, id2;
        string dato;

        public FAsignacion()
        {
            InitializeComponent();
            CargaCombos();
            GestionControles(false, false, false, false, false, false, false, false, false,false,false);
            cmbasignacion.Enabled = false;
        }

        private void btncerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GestionControles( bool ver, bool ver1, bool ver2, bool ver3, bool ver4,
            bool ver5, bool ver6, bool ver7, bool ver8, bool ver9, bool ver10)
        { 
            cmbCarga.Visible = ver;
            txtHoras.Visible = ver1;
            datepickFecha.Visible = ver2;
            btnaltaCurso.Visible = ver3;
            btnaltaEpi.Visible = ver4;
            btnaltaH.Visible = ver5;
            lblValor.Visible = ver6;
            lblhora.Visible = ver7;
            lblfecha.Visible = ver8;
            cmbmes.Visible = ver9;
            lblmes.Visible = ver10;
        }

        public void CargaCombos()
        {
            cmbTrabajador.DataSource = moduloInicio.CargaGridyCombo("select Nombre from pyme.trabajadors");
            cmbTrabajador.ValueMember = "Nombre";
            cmbTrabajador.SelectedIndex = -1;
            string[] asignaciones = new string[] { "CURSOS", "EPIS", "HORAS" };
            string[] losmeses = new string[] {"ENERO","FEBRERO","MARZO","ABRIL","MAYO","JUNIO","JULIO","AGOSTO","SEPTIEMBRE",
            "OCTUBRE","NOVIEMBRE","DICIEMBRE"};
                       
                foreach (var item in asignaciones)
                {
                    cmbasignacion.Items.Add(item);
                }

                foreach (var item in losmeses)
                {
                    cmbmes.Items.Add(item);
                }
            
               

        }

        private void CmbTrabajador_SelectedValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CargaGridAsignacion(string sql)
        {
            datagridAsignar.DataSource = moduloInicio.CargaGridyCombo(sql);
            datagridAsignar.Columns[0].Visible = false;
        }

        private void cmbasignacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTrabajador.SelectedIndex != -1)
            {
               idtrabajador = moduloInicio.ObtenerId("select IdTrabajador from pyme.trabajadors where Nombre='" + cmbTrabajador.SelectedValue.ToString() + "';");

                switch (cmbasignacion.SelectedIndex)
                    {   
                        case 0:
                        cmbCarga.DataSource = moduloInicio.CargaGridyCombo("select Nombre from pyme.cursoes where not exists(select *from pyme.trabajadorcursoes where IdCurso=Curso_IdCurso and Trabajador_IdTrabajador =" + idtrabajador + ");");
                        cmbCarga.ValueMember = "Nombre";
                        cmbCarga.SelectedIndex = -1;

                        CargaGridAsignacion("select Curso_IdCurso, Trabajador_IdTrabajador, Nombre, Duracion from pyme.cursoes, pyme.trabajadorcursoes where IdCurso=Curso_IdCurso and Trabajador_IdTrabajador =" + idtrabajador + ";");
                             datagridAsignar.Columns[1].Visible = false;
                            GestionControles(true,false,false,true,false,false,true,false,false,false,false);
                            lblValor.Text = "CURSOS";
                            break;

                        case 1:
                          cmbCarga.DataSource = moduloInicio.CargaGridyCombo("select e.Nombre from pyme.epis e where not exists(select *from pyme.trabajadorepis t where e.IdEpi=t.IdEpi and t.IdTrabajador =" + idtrabajador + ");");
                        cmbCarga.ValueMember = "Nombre";
                        cmbCarga.SelectedIndex = -1;

                        CargaGridAsignacion("select t.IdEpi, t.IdTrabajador, e.Nombre, t.FechaEpi from pyme.epis e,  pyme.trabajadorepis t where e.IdEpi=t.IdEpi and t.IdTrabajador =" + idtrabajador + ";");
                              datagridAsignar.Columns[1].Visible = false;
                            GestionControles(true, false, true, false, true, false, true, false, true,false,false);
                            lblValor.Text = "EPIS";
                            break;

                        case 2:
                            CargaGridAsignacion("select e.IdExtra, e.horas as Horas, (e.horas*t.Valor) as Total from pyme.trabajadors t, pyme.extras e where t.IdTrabajador= e.IdTrabajador and t.IdTrabajador=" + idtrabajador + ";");
                            GestionControles(false, true, false, false, false, true, false, true, false,true,true);
                            lblValor.Text = "HORAS";
                        cmbmes.SelectedIndex = -1;
                        break;
                    }
            
            } 
            else { MessageBox.Show("selecciona un trabajador");} //ojo
        }

        private void txtHoras_Validating(object sender, CancelEventArgs e)
        {
            if (txtHoras.Text != "")
            {
                if (!moduloInicio.IsNumeric(txtHoras.Text) ||(!moduloInicio.isnumericDouble(txtHoras.Text)))
                { MessageBox.Show("Introduce horas");e.Cancel = true; }
            }
        }

        private void Controlar()
        {
            if (cmbTrabajador.SelectedIndex == -1) { MessageBox.Show("Selecciona trabajador"); cmbTrabajador.Focus(); return; }
            if (cmbasignacion.SelectedIndex == -1) { MessageBox.Show("Selecciona Asignación"); cmbasignacion.Focus(); return; }
        }

        private void btnaltaCurso_Click(object sender, EventArgs e)
        {
            Controlar();
            if (cmbCarga.SelectedIndex == -1) { MessageBox.Show("Selecciona un valor"); cmbCarga.Focus(); return; }
            int idcurso = moduloInicio.ObtenerId("select IdCurso from pyme.cursoes where Nombre='" + 
                cmbCarga.SelectedValue.ToString() + "';");
            moduloInicio.OperarSql("insert into pyme.trabajadorcursoes(Trabajador_IdTrabajador,Curso_IdCurso ) values(" + idtrabajador  + "," + idcurso + ");");
            ActualizarControles();
            CargaGridAsignacion("select Curso_IdCurso, Trabajador_IdTrabajador, Nombre, Duracion from pyme.cursoes, pyme.trabajadorcursoes where IdCurso=Curso_IdCurso and Trabajador_IdTrabajador =" + idtrabajador + ";");
            datagridAsignar.Columns[1].Visible = false;
            
        }

        private void btnaltaEpi_Click(object sender, EventArgs e)
        {
            Controlar();
            if (cmbCarga.SelectedIndex == -1) { MessageBox.Show("Selecciona un valor"); cmbCarga.Focus(); return; }
            int idepi = moduloInicio.ObtenerId("select IdEpi from pyme.epis where Nombre='" +
               cmbCarga.SelectedValue.ToString() + "';");
            if (moduloFechas.CompararFecha("select FechaAlta from pyme.trabajadors where IdTrabajador=" + idtrabajador + ";", datepickFecha.Value))
            {
                using (var contexto = new MyDbContext())
                {
                    TrabajadorEpi trabajadorEpi = new TrabajadorEpi { IdEpi = idepi, IdTrabajador = idtrabajador, FechaEpi = moduloFechas.ObtenerFecha(datepickFecha.Value)  };
                    contexto.TrabajadorEpis.Add(trabajadorEpi);
                    contexto.SaveChanges();
                }
                CargaGridAsignacion("select t.IdEpi, t.IdTrabajador, e.Nombre, t.FechaEpi from pyme.epis e, pyme.trabajadorepis t where e.IdEpi=t.IdEpi and t.IdTrabajador =" + idtrabajador + ";");
                datagridAsignar.Columns[1].Visible = false;
            }
            else { MessageBox.Show("la fecha de entrega debe se superior o igual fecha alta"); }
            ActualizarControles(); moduloInicio.LimpiarTexto(this);
        }

        private void btnaltaH_Click(object sender, EventArgs e)
        {
            Controlar();
            if (txtHoras.Text == "") { MessageBox.Show("Introduce Nº horas"); txtHoras.Focus(); return; }
            if (cmbmes.SelectedIndex == -1) { MessageBox.Show("Selecciona mes"); cmbmes.Focus(); return; }
            using (var contexto= new MyDbContext())
            {
                Extra extra = new Extra { horas = double.Parse(txtHoras.Text), IdTrabajador = idtrabajador };//ojo
                contexto.Extras.Add(extra);
                contexto.SaveChanges();
            }
            moduloTexto.GenerarWordHoras(cmbTrabajador.SelectedValue.ToString(), cmbmes.SelectedItem.ToString(),double.Parse(txtHoras.Text));
            ActualizarControles(); moduloInicio.LimpiarTexto(this);
            CargaGridAsignacion("select e.IdExtra, e.horas as Horas, (e.horas*t.Valor) as Total from pyme.trabajadors t, pyme.extras e where t.IdTrabajador= e.IdTrabajador and t.IdTrabajador=" + idtrabajador + ";");
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (cmbasignacion.SelectedIndex != -1)
            {
                switch (cmbasignacion.SelectedIndex)
                {
                    case 0:
                        if (id1 != 0||dato!=null)
                        {
                            if (MessageBox.Show("Este proceso borra el curso asignado " +
                        datagridAsignar.CurrentRow.Cells[2].Value.ToString().ToUpper() +
                        " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                
                                moduloInicio.EliminarCursoTrabajador(id1, id2);
                                CargaGridAsignacion("select Curso_IdCurso, Trabajador_IdTrabajador, Nombre, Duracion from pyme.cursoes, pyme.trabajadorcursoes where IdCurso=Curso_IdCurso and Trabajador_IdTrabajador =" + idtrabajador + ";");
                                datagridAsignar.Columns[1].Visible = false;
                            }
                            id1 = 0; dato = null;
                        }
                        else { MessageBox.Show("selecciona un curso"); }
                        break;

                    case 1:
                        if (id1 != 0 || dato != null)
                        {
                            if (MessageBox.Show("Este proceso borra el epi asignado " +
                        datagridAsignar.CurrentRow.Cells[2].Value.ToString().ToUpper() +
                        " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                using (var contexto = new MyDbContext())
                                {
                                    TrabajadorEpi trabajadorEpi = contexto.TrabajadorEpis.Where(x => x.IdEpi == id1 && x.IdTrabajador == id2).FirstOrDefault();
                                    contexto.TrabajadorEpis.Remove(trabajadorEpi);
                                    contexto.SaveChanges();
                                }
                                CargaGridAsignacion("select t.IdEpi, t.IdTrabajador, e.Nombre, t.FechaEpi from pyme.epis e, pyme.trabajadorepis t where e.IdEpi=t.IdEpi and t.IdTrabajador =" + idtrabajador + ";");
                                datagridAsignar.Columns[1].Visible = false;
                            }
                            id1 = 0; dato = null;
                        }
                        else { MessageBox.Show("selecciona un epi"); }
                        break;

                    case 2:
                        if (id1 != 0 || dato != null)
                        {
                            if (MessageBox.Show("Este proceso borra la extra asignada " +
                                               " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                using (var contexto = new MyDbContext())
                                {
                                    Extra extra = contexto.Extras.Where(x => x.IdExtra == id1).FirstOrDefault();
                                    contexto.Extras.Remove(extra);
                                    contexto.SaveChanges();
                                }
                                CargaGridAsignacion("select e.IdExtra, e.horas as Horas, (e.horas*t.Valor) as Total from pyme.trabajadors t, empresa.extras e where t.IdTrabajador= e.IdTrabajador and t.IdTrabajador=" + idtrabajador + ";");
                            }
                            id1 = 0; dato = null;
                        }
                        else { MessageBox.Show("selecciona un período"); }
                        break;
                }       
            }
            else { MessageBox.Show("selecciona una opción"); }
            
        }

        private void cmbTrabajador_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarControles();
            cmbasignacion.Enabled = true;
            datagridAsignar.DataSource = null;
            datagridAsignar.Refresh();
        }

        private void ActualizarControles()
        {
            GestionControles(false, false, false, false, false, false, false, false, false,false,false);
            cmbasignacion.SelectedIndex = -1;cmbCarga.SelectedIndex = -1;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this); moduloInicio.LimpiarComboyCheck(this);
            datagridAsignar.DataSource = "";
            datagridAsignar.Columns.Clear();
            GestionControles(false, false, false, false, false, false, false, false, false, false, false);
            cmbasignacion.Enabled = false;
        }

        private void datagridAsignar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (datagridAsignar.CurrentRow.Cells[0].Value.ToString() != "")
                {
                   id1= int.Parse(datagridAsignar.CurrentRow.Cells[0].Value.ToString());
                   id2= int.Parse(datagridAsignar.CurrentRow.Cells[1].Value.ToString());
                    dato = datagridAsignar.CurrentRow.Cells[2].Value.ToString();
                    
                }
                else { MessageBox.Show("selecciona o debe dar de alta asignación"); }
              
            }
        }
    }
}
