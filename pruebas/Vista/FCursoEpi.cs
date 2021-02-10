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
    public partial class FCursoEpi : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        int idCurso;
        int idEpi;
        int idcont=0;
        public FCursoEpi()
        {
            InitializeComponent();
            datagridCurso.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.cursoes");
            datagridEpi.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.epis");
            datagridCurso.Columns[0].Visible = false;
            datagridEpi.Columns[0].Visible = false;
            idcont = moduloInicio.ObtenerIdControl(Constants.Id_usuario).Last().IdControl;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region metodos Curso
        private void datagridCurso_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (datagridCurso.CurrentRow.Cells[0].Value.ToString() != "")
                {
                idCurso = int.Parse( datagridCurso.CurrentRow.Cells[0].Value.ToString());
                txtnombreCur.Text = datagridCurso.CurrentRow.Cells[1].Value.ToString();
                txtHoras.Text = datagridCurso.CurrentRow.Cells[2].Value.ToString();
                btnaltaCur.Enabled = false;
                }
                else { moduloInicio.LimpiarTexto(this); btnaltaCur.Enabled = true;
                    MessageBox.Show("selecciona o debe dar de alta un curso"); }
               
            }
        }

        private void txtnombreCur_Validating(object sender, CancelEventArgs e)
        {
            if (txtnombreCur.Text != "")
            {
                if (moduloInicio.IsNumeric(txtnombreCur.Text)) { MessageBox.Show("Introduce curso"); e.Cancel = true; }
            }
        }

        private void txtHoras_Validating(object sender, CancelEventArgs e)
        {
            if (txtHoras.Text != "")
            {
                if (!moduloInicio.IsNumeric(txtHoras.Text)) { MessageBox.Show("Introduce horas"); e.Cancel = true; }
            }
        }

        private void btnaltaCur_Click(object sender, EventArgs e)
        {
            if (txtnombreCur.Text == "") { MessageBox.Show("Introduce curso"); txtnombreCur.Focus();return; }
            if (txtHoras.Text == "") { MessageBox.Show("Introduce horas"); txtHoras.Focus(); return; }

            if(!moduloInicio.Existe("select Nombre from pyme.cursoes where Nombre='" + txtnombreCur.Text + "' and Duracion="+txtHoras.Text+";"))
            {
                using(var contexto= new MyDbContext())
                {                    
                    Curso Nuevocurso = new Curso { Nombre = txtnombreCur.Text.ToUpper(), Duracion = int.Parse(txtHoras.Text) };
                    contexto.Cursos.Add(Nuevocurso);
                    contexto.SaveChanges();
                  int idCursoCtl=  moduloInicio.ObtenerId("select IdCurso from pyme.cursoes where Nombre='" + txtnombreCur.Text + "';");
                    moduloInicio.OperarSql("Insert into pyme.cursocontrols(Curso_IdCurso,Control_IdControl) values(" + idCursoCtl + "," + idcont + ");");
                }
                datagridCurso.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.cursoes");
                
            }
            else { MessageBox.Show("El curso ya existe"); }
            moduloInicio.LimpiarTexto(this);
        }

        private void btnModifCur_Click(object sender, EventArgs e)
        {
            if (txtnombreCur.Text!= "" || txtHoras.Text!= "")
            {
                using (var contexto = new MyDbContext())
                {
                   Curso curso= contexto.Cursos.Where(x => x.IdCurso == idCurso).FirstOrDefault();
                   
                   curso.Nombre= moduloInicio.ControlarModificar("select Nombre " +
                        "from pyme.cursoes where Nombre='" + txtnombreCur.Text.ToUpper() + "';", "select Nombre " +
                        "from pyme.cursoes where IdCurso=" + idCurso + ";", 0, txtnombreCur.Text);
                    curso.Duracion = int.Parse(txtHoras.Text);
                    contexto.SaveChanges();
                }
                datagridCurso.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.cursoes");
                moduloInicio.LimpiarTexto(this);btnaltaCur.Enabled = true;
            }
            else { MessageBox.Show("Selecciona un curso"); }
            
        }

        private void btnEliminarCur_Click(object sender, EventArgs e)
        {
            if (txtnombreCur.Text != "" || txtHoras.Text != "")
            {
                if (!moduloInicio.Existe("select Curso_IdCurso from pyme.trabajadorcursoes where Curso_IdCurso=" + idCurso + ";"))
                {
                    if (MessageBox.Show("Este proceso borra el curso " + datagridCurso.CurrentRow.Cells[1].Value.ToString().ToUpper() + " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (var contexto = new MyDbContext())
                        {
                            moduloInicio.OperarSql("delete from pyme.cursocontrols where Curso_IdCurso=" + idCurso + ";");
                            Curso curso = contexto.Cursos.Where(x => x.IdCurso == idCurso).FirstOrDefault();
                            contexto.Cursos.Remove(curso);
                            contexto.SaveChanges();
                        }
                        datagridCurso.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.cursoes");
                    }
                }
                else { MessageBox.Show("No se puede,tiene trabajadores asociados"); }
               moduloInicio.LimpiarTexto(this); btnaltaCur.Enabled = true;
            }
            else { MessageBox.Show("selecciona un curso"); }
           
        }

        private void btncancelarC_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);
            btnaltaCur.Enabled = true; btnaltaEpi.Enabled = true;
        }

        #endregion

        #region metodos Epi
        private void txtEpi_Validating(object sender, CancelEventArgs e)
        {
            if (txtEpi.Text != "")
            {
                if(moduloInicio.IsNumeric(txtEpi.Text)) { MessageBox.Show("Introduce un Epi");e.Cancel = true; }
            }
        }

        private void datagridEpi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (datagridEpi.CurrentRow.Cells[0].Value.ToString() != "")
                {
                 idEpi= int.Parse( datagridEpi.CurrentRow.Cells[0].Value.ToString());
                 txtEpi.Text = datagridEpi.CurrentRow.Cells[1].Value.ToString();
                 btnaltaEpi.Enabled = false;  
                }
                else { moduloInicio.LimpiarTexto(this); btnaltaEpi.Enabled = true;
                    MessageBox.Show("selecciona o debe dar de alta epi"); }
                             
            }
        }

        private void btnaltaEpi_Click(object sender, EventArgs e)
        {
            if (txtEpi.Text == "") { MessageBox.Show("Introduce Epi"); txtEpi.Focus(); return; }

            if (!moduloInicio.Existe("select Nombre from pyme.epis where IdEpi=" + idEpi +";"))
            {
                using(var contexto=new MyDbContext())
                {   
                    Epi epi = new Epi { Nombre = txtEpi.Text.ToUpper() };
                    contexto.Epis.Add(epi);
                    contexto.SaveChanges();
                    int idepis = moduloInicio.ObtenerId("select IdEpi from pyme.epis where Nombre='" + txtEpi.Text + "';");
                    moduloInicio.OperarSql("Insert into pyme.epicontrols(Epi_IdEpi,Control_IdControl) values(" + idepis + "," + idcont + ");");
                }
                datagridEpi.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.epis");
            }
            else { MessageBox.Show("El epi ya existe"); }
            moduloInicio.LimpiarTexto(this);
        }

        private void btnModificarEpi_Click(object sender, EventArgs e)
        {
            string nombreEpi=datagridEpi.CurrentRow.Cells[1].Value.ToString();
            if (txtEpi.Text != "")
            {
                if(!moduloInicio.Existe("select Nombre from pyme.epis where Nombre ='" + nombreEpi + "';"))
                {
                    using (var contexto = new MyDbContext())
                    {
                        Epi epi= contexto.Epis.Where(x => x.IdEpi == idEpi).FirstOrDefault();
                        epi.Nombre = txtEpi.Text;
                        contexto.Epis.Add(epi);
                        contexto.SaveChanges();
                    }
                   datagridEpi.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.epis");
                }
                else { MessageBox.Show("El epi ya existe"); }
                 moduloInicio.LimpiarTexto(this); btnaltaEpi.Enabled = true;
            }
            else { MessageBox.Show("Selecciona un Epi"); }
           
        }

        private void btneliminarEp_Click(object sender, EventArgs e)
        {
            if (txtEpi.Text != "")
            {

                if (!moduloInicio.Existe("select IdEpi from pyme.trabajadorepis where IdEpi=" + idEpi + ";"))
            {
                if (MessageBox.Show("Este proceso borra el epi " + datagridEpi.CurrentRow.Cells[1].Value.ToString().ToUpper() + " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (var contexto = new MyDbContext())
                    {
                        moduloInicio.OperarSql("delete from pyme.epicontrols where Epi_IdEpi=" + idEpi + ";");
                        Epi epi = contexto.Epis.Where(x => x.IdEpi == idEpi).FirstOrDefault();
                        contexto.Epis.Remove(epi);
                        contexto.SaveChanges();
                    }
                    datagridEpi.DataSource = moduloInicio.CargaGridyCombo("select*from pyme.epis");
                }
            }
            else { MessageBox.Show("No se puede,tiene trabajadores asociados"); }
            moduloInicio.LimpiarTexto(this); btnaltaEpi.Enabled = true;
            }
            else { MessageBox.Show("dar de alta o selecciona uno"); }
        }

        private void btncancelarE_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);
            btnaltaEpi.Enabled = true; btnaltaCur.Enabled = true;
        }


        #endregion


    }
}
