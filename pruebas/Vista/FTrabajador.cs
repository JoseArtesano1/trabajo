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
    public partial class FTrabajador : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        ModuloFechas moduloFechas = new ModuloFechas();
        ModuloTexto moduloTexto = new ModuloTexto();
        int idTrabajador;
        int idcont;
       public string[] asignaciones = new string[] {"VER EPIS", "VER CURSOS", "VER PERIODOS", "VER HORAS" };

        public FTrabajador()
        {
            InitializeComponent();
          datagridtrabaj.DataSource=  moduloInicio.CargaGridyCombo("select IdTrabajador, Nombre,Telefono,Activo from pyme.trabajadors");
           datagridtrabaj.Columns[0].Visible = false;
            cmbCategoria.DataSource = moduloInicio.CargaGridyCombo("select Nombre from pyme.categorias");
            cmbCategoria.ValueMember = "Nombre";
             cmbCategoria.SelectedIndex = -1;
            CargaCmbAsignado();
            idcont = moduloInicio.ObtenerIdControl(Constants.Id_usuario).Last().IdControl;
            datePickpermiso.Visible = false;
            lblPC.Visible = false;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VaciarDatagridAsignar()
        {   cmbAsignado.SelectedIndex = -1;
            datagridAsignados.DataSource = null;
            datagridAsignados.Refresh();
        }

        private void datagridtrabaj_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Trabajador trabajador; 
            if (e.RowIndex != -1)
            {
                if (datagridtrabaj.CurrentRow.Cells[0].Value.ToString() != "")
                {
                    cmbAsignado.Enabled = true;
                    idTrabajador = int.Parse(datagridtrabaj.CurrentRow.Cells[0].Value.ToString());
                    txtnombre.Text = datagridtrabaj.CurrentRow.Cells[1].Value.ToString();
                    txtmovil.Text = datagridtrabaj.CurrentRow.Cells[2].Value.ToString();
                    if (bool.Parse(datagridtrabaj.CurrentRow.Cells[3].Value.ToString()))
                    {
                        checboxActivo.Checked = true;
                    }
                    else { checboxActivo.Checked = false; }

                    trabajador = moduloInicio.TrabajadoresEmpresa().Where(x => x.IdTrabajador == idTrabajador).FirstOrDefault(); 
                    int idcs= moduloInicio.ObtenerId("select IdCategoria from pyme.trabajadors where IdTrabajador=" + 
                    idTrabajador + ";");
                    txtdireccion.Text = trabajador.Direccion;
                    txtdni.Text = trabajador.Dni;
                    txtseguridadS.Text = trabajador.Nseguridads.ToString();
                    txtValor.Text = trabajador.Valor.ToString();
                    cmbCategoria.SelectedValue = moduloInicio.ObtenerCategoria(idcs).Nombre;
                    datepickAlta.Value = DateTime.Parse(trabajador.FechaAlta);
                    datepickDni.Value = DateTime.Parse(trabajador.FechaDni);
                    datepickMedico.Value = DateTime.Parse(trabajador.FechaMedico);
                    string fpermiso = moduloInicio.Obtenerdato("select FechaPermiso from pyme.trabajadors where IdTrabajador=" + idTrabajador + ";", 0);
                    if (fpermiso != "")
                    {   checkBoxCoche.Checked = true;
                        datePickpermiso.Value = DateTime.Parse(trabajador.FechaPermiso);
                    }
                    else
                    {   checkBoxCoche.Checked = false;
                        datePickpermiso.Value = DateTime.Today;
                    }

                    VaciarDatagridAsignar();
                    btnAlta.Enabled = false;
                }
                else 
                {
                    moduloInicio.LimpiarTexto(this); moduloInicio.LimpiarComboyCheck(this); //cmbCategoria.SelectedIndex = -1; 
                    MessageBox.Show("selecciona uno o debe dar de alta Trabajador"); btnAlta.Enabled = true;
                    VaciarDatagridAsignar(); cmbAsignado.Enabled = false;
                }
                
            }
          
        }

        public void CargaCmbAsignado()
        {
            foreach(var item in asignaciones)
            {
                cmbAsignado.Items.Add(item);
            }
        }

        private void btnAlta_Click(object sender, EventArgs e)
        {
            if (!checboxActivo.Checked) { MessageBox.Show("Activa el trabajador"); checboxActivo.Focus(); return; }
            ControlDatos();
            DateTime fechaAlta = datepickAlta.Value;
            DateTime fechaDni = datepickDni.Value;
            DateTime fechaMed = datepickMedico.Value;
            string fechaVehiculo;

            if (checkBoxCoche.Checked)
            {   DateTime fechaPermiso = datePickpermiso.Value;
                fechaVehiculo = moduloFechas.ObtenerFecha(fechaPermiso);
            }
            else {fechaVehiculo = "";  }
         
            if (!moduloInicio.Existe("select IdTrabajador from pyme.trabajadors where Dni='" + txtdni.Text + "'or Nseguridads="+txtseguridadS.Text + ";"))
            { 
                if(DateTime.Compare(fechaAlta,fechaDni)<0 || DateTime.Compare(fechaAlta, fechaDni) == 0)
                {
                  using(var contexto= new MyDbContext())
                  {  
                        Trabajador trabajador = new Trabajador
                        { Dni = txtdni.Text.ToUpper(), Nombre = txtnombre.Text.ToUpper(), 
                            Direccion = txtdireccion.Text.ToUpper(), 
                            Telefono = int.Parse(txtmovil.Text),
                            Nseguridads = long.Parse(txtseguridadS.Text),
                            FechaAlta = moduloFechas.ObtenerFecha(fechaAlta),
                            FechaMedico = moduloFechas.ObtenerFecha(fechaMed), 
                            FechaDni = moduloFechas.ObtenerFecha(fechaDni),
                            Valor = int.Parse(txtValor.Text), 
                            Activo = checboxActivo.Checked, 
                            IdCategoria = contexto.Categorias.Where(x => 
                            x.Nombre ==cmbCategoria.SelectedValue.ToString()).FirstOrDefault().IdCategoria,
                            FechaPermiso = fechaVehiculo
                        };
                        contexto.Trabajadors.Add(trabajador);
                        contexto.SaveChanges();
                        int idt = moduloInicio.ObtenerId("select IdTrabajador from pyme.trabajadors where Dni='" + txtdni.Text + "';");
                        moduloInicio.OperarSql("Insert into pyme.trabajadorcontrols(Trabajador_IdTrabajador, Control_IdControl) values(" +  idt + "," + idcont + ");"); //ojo
                        datagridtrabaj.DataSource = moduloInicio.CargaGridyCombo("select IdTrabajador, Nombre,Telefono,Activo from pyme.trabajadors");
                        moduloTexto.GenerarWordAlta_Baja(trabajador, cmbCategoria.SelectedValue.ToString(),checboxActivo.Checked);
                     }
                }
                else { MessageBox.Show("la fecha del dni debe ser inferior a la fecha de alta"); }
            }
            else { MessageBox.Show("el dni/nie O Nº de la Seguridad Social ya existe"); }
            Limpiar();
        }

        private void txtnombre_Validating(object sender, CancelEventArgs e)
        {
            if (txtnombre.Text != "")
            {
                if (moduloInicio.IsNumeric(txtnombre.Text)) { MessageBox.Show("Introduce nombre");e.Cancel = true; }
            }
        }

        private void txtdni_Validating(object sender, CancelEventArgs e)
        {
            if (txtdni.Text != "")
            {
                if (moduloInicio.IsNumeric(txtdni.Text)) { MessageBox.Show("Introduce dni/nie"); e.Cancel = true; }
            }
        }

        private void txtdireccion_Validating(object sender, CancelEventArgs e)
        {
            if (txtdireccion.Text != "")
            {
                if (moduloInicio.IsNumeric(txtdireccion.Text)) { MessageBox.Show("Introduce dirección"); e.Cancel = true; }
            }
        }

        private void txtseguridadS_Validating(object sender, CancelEventArgs e)
        {
            if (txtseguridadS.Text != "")
            {
                if (!moduloInicio.isnumericLong(txtseguridadS.Text)) { MessageBox.Show("Introduce N.Seg.Social");
                    e.Cancel = true; }
            }
        }

        private void txtValor_Validating(object sender, CancelEventArgs e)
        {
            if (txtValor.Text != "")
            {
                if (!moduloInicio.IsNumeric(txtValor.Text)){MessageBox.Show("Introduce Importe"); e.Cancel = true;  }
            }
        }

        private void txtmovil_Validating(object sender, CancelEventArgs e)
        {
            if (txtmovil.Text != "")
            {
                if (!moduloInicio.IsNumeric(txtmovil.Text)) { MessageBox.Show("Introduce teléfono"); e.Cancel = true; }
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {           
            if (txtdni.Text != "")
            {
                ControlDatos();
                DateTime fechaAlta = datepickAlta.Value;
                DateTime fechaDni = datepickDni.Value;
                DateTime fechaMed = datepickMedico.Value;
                string fechaVehiculo;

                if (checkBoxCoche.Checked)
                {
                    DateTime fechaPerm = datePickpermiso.Value;
                    fechaVehiculo = moduloFechas.ObtenerFecha(fechaPerm);
                }
                else{fechaVehiculo = "";}
             

                if (DateTime.Compare(fechaAlta, fechaDni) < 0 || DateTime.Compare(fechaAlta, fechaDni) == 0)
                {
                    using (var contexto = new MyDbContext())
                    {                       
                         string dni = moduloInicio.ControlarModificar("select Dni from " +
                            "pyme.trabajadors where Dni='" + txtdni.Text.ToUpper() + "';", "select Dni from" +
                            " pyme.trabajadors where IdTrabajador=" + idTrabajador+ ";", 0, txtdni.Text);
                       
                         string nombre= txtnombre.Text.ToUpper();
                         string direccion= txtdireccion.Text.ToUpper();
                         int telefono= int.Parse(txtmovil.Text);
                         
                         long ss= long.Parse  ( moduloInicio.ControlarModificar("select Nseguridads from " +
                            "pyme.trabajadors where Nseguridads=" + txtseguridadS.Text + ";", "select Nseguridads from" +
                            " pyme.trabajadors where IdTrabajador=" + idTrabajador + ";", 0, txtseguridadS.Text));
                        string alta= moduloFechas.ObtenerFecha(moduloFechas.CheckfechaModifAlta(contexto, idTrabajador, fechaAlta));
                         string fmedico= moduloFechas.ObtenerFecha(fechaMed);
                         string fdni= moduloFechas.ObtenerFecha(fechaDni);
                         int val= int.Parse(txtValor.Text);
                         bool act= checboxActivo.Checked;
                         int idcat= contexto.Categorias.Where(x => x.Nombre == cmbCategoria.SelectedValue.ToString())
                            .FirstOrDefault().IdCategoria;

                        moduloInicio.ModificarTrabajador(dni, nombre, direccion, alta, fdni, fmedico, telefono, ss, val, idcat, act, fechaVehiculo,idTrabajador);

                        datagridtrabaj.DataSource = moduloInicio.CargaGridyCombo("select IdTrabajador, Nombre,Telefono,Activo from pyme.trabajadors");
                        Trabajador trabajador = moduloInicio.TrabajadoresEmpresa().Where(x => x.IdTrabajador == idTrabajador).FirstOrDefault(); 
                        if (!act)
                        {moduloTexto.GenerarWordAlta_Baja(trabajador, cmbCategoria.SelectedValue.ToString(), act); }
     
                    }
                   
                }
                else { MessageBox.Show("la fecha del dni debe ser inferior a la fecha de alta"); }
                Limpiar(); btnAlta.Enabled = true;
            }
            else { MessageBox.Show("Selecciona un trabajador"); }
        }

        public void ControlDatos()
        {
            
            if (txtnombre.Text == "") { MessageBox.Show("Introduce nombre"); txtnombre.Focus(); return; }
            if (txtdni.Text == "") { MessageBox.Show("Introduce dni/nie"); txtdni.Focus(); return; }
            if (txtdireccion.Text == "") { MessageBox.Show("Introduce dirección"); txtdireccion.Focus(); return; }
            if (txtmovil.Text == "") { MessageBox.Show("Introduce Teléfono"); txtmovil.Focus(); return; }
            if (txtseguridadS.Text == "") { MessageBox.Show("Introduce Nº S.Seguridad"); txtseguridadS.Focus(); return; }
            if (txtValor.Text == "") { MessageBox.Show("Introduce importe"); txtValor.Focus(); return; }
            if (cmbCategoria.SelectedIndex == -1) { MessageBox.Show("Selecciona categoría"); cmbCategoria.Focus(); return; }
        }

        private void cmbAsignado_SelectedIndexChanged(object sender, EventArgs e)
        {
          
            if (idTrabajador != 0)
            {
                switch (cmbAsignado.SelectedIndex)
                {
                    case 0:
                        datagridAsignados.DataSource = moduloInicio.CargaGridyCombo("select e.Nombre, t.FechaEpi " +
                              "from pyme.epis e, pyme.trabajadorepis t where e.IdEpi = t.IdEpi and " +
                              "t.IdTrabajador =" + idTrabajador + ";");
                        break;
                    case 1:
                       datagridAsignados.DataSource= moduloInicio.CargaGridyCombo("select Nombre, Duracion from pyme.cursoes, pyme.trabajadorcursoes where IdCurso=Curso_IdCurso and Trabajador_IdTrabajador=" + idTrabajador+";");
                         break;
                    case 2:
                        datagridAsignados.DataSource = moduloFechas.CargaGridDias(idTrabajador);
                        break;
                    case 3:
                         datagridAsignados.DataSource= moduloInicio.CargaGridyCombo("select e.horas, (e.horas*t.Valor) as Importe" +
                            " from pyme.trabajadors t,  pyme.extras e " +
                            "where t.IdTrabajador = e.IdTrabajador and t.IdTrabajador=" + idTrabajador + ";");
                        break;
                }
            }
            else { cmbAsignado.SelectedIndex = -1; }  //ojo
            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (txtdni.Text != "")
            {
                if (moduloInicio.Existe("select IdTrabajador from pyme.trabajadorepis where IdTrabajador=" + idTrabajador + ";") || moduloInicio.Existe("select Trabajador_IdTrabajador from pyme.trabajadorcursoes where Trabajador_IdTrabajador=" + idTrabajador + ";"))
                {
                    MessageBox.Show("no se puede eliminar, tiene cursos o epis asociados");
                }
                else
                {
                    if (moduloInicio.Existe("select IdTrabajador from pyme.periodoes where IdTrabajador=" + idTrabajador + ";") || moduloInicio.Existe("select IdTrabajador from pyme.extras where IdTrabajador=" + idTrabajador + ";"))
                    {
                        MessageBox.Show("no se puede eliminar, tiene Periodos o extras asociadas");
                    }
                    else
                    {
                        if (MessageBox.Show("Este proceso borra el trabajador " +
                            datagridtrabaj.CurrentRow.Cells[1].Value.ToString().ToUpper() +
                            " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            using (var contexto = new MyDbContext())
                            {
                                moduloInicio.OperarSql("delete from pyme.trabajadorcontrols where Trabajador_IdTrabajador=" + idTrabajador + ";");
                                Trabajador trabajador = contexto.Trabajadors.Where(x => x.IdTrabajador == idTrabajador).FirstOrDefault();
                                contexto.Trabajadors.Remove(trabajador);

                                contexto.SaveChanges();
                            }
                            datagridtrabaj.DataSource = moduloInicio.CargaGridyCombo("select IdTrabajador, Nombre,Telefono,Activo from pyme.trabajadors");
                        }
                    }
                }
                 Limpiar();  btnAlta.Enabled = true;
            }
            else { MessageBox.Show("selecciona un trabajador"); }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Limpiar();  btnAlta.Enabled = true;
        }

        private void Limpiar()
        {   moduloInicio.LimpiarTexto(this);
            checboxActivo.Checked = false;
            checkBoxCoche.Checked = false;
            cmbCategoria.SelectedIndex = -1;
        }

        private void checkBoxCoche_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCoche.Checked)
            {   datePickpermiso.Visible = true;
                lblPC.Visible = true;
            }
            else
            {
                datePickpermiso.Visible = false;
                lblPC.Visible = false;
            }
        }
    }
}
