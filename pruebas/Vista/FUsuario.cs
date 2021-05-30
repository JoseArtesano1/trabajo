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
    public partial class FUsuario : Form
    { 
        ModuloInicio moduloInicio = new ModuloInicio();
       
        public string[] autorizaciones = new string[] { "A", "U" };
        int iduser;
        public FUsuario()
        {
            InitializeComponent();
            cmbAutoriza.SelectedIndex = -1;
            LLenarComboAutorizacion();
            datagridUsuarios.DataSource = moduloInicio.CargaGridCategoria(btnAlta, btneliminar, txtnombre, cmbAutoriza, lblAutoriza,btnControl);
            datagridUsuarios.Columns[0].Visible = false;

        }
                    
        private void Recarga()
        {
            datagridUsuarios.DataSource = moduloInicio.CargaGridCategoria(btnAlta, btneliminar, txtnombre, cmbAutoriza, lblAutoriza,btnControl);
            moduloInicio.LimpiarTexto(this);
            cmbAutoriza.SelectedIndex = -1;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtnombre_Validating(object sender, CancelEventArgs e)
        {
            if (txtnombre.Text != "")
            {
                if (moduloInicio.IsNumeric(txtnombre.Text)){MessageBox.Show("Debe introducir un nombre"); e.Cancel = true;}
        
            }
        }

        private void txtusuario_Validating(object sender, CancelEventArgs e)
        {
            if (txtusuario.Text != "")
            {
                if (moduloInicio.IsNumeric(txtusuario.Text)) {MessageBox.Show("Debe introducir un usuario"); e.Cancel = true; }
         
            }
        }

        private void txtpass_Validating(object sender, CancelEventArgs e)
        {
            if (txtpass.Text != "")
            {
                if (moduloInicio.IsNumeric(txtpass.Text))  { MessageBox.Show("Debe introducir contraseña alfanúmerica"); 
                    e.Cancel = true;  }
            }
        }

        private void LLenarComboAutorizacion()
        {
            for (int i = 0; i < autorizaciones.Length; i++)
            {
                cmbAutoriza.Items.Add(autorizaciones[i]);
            }
        }

        private void datagridUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (datagridUsuarios.CurrentRow.Cells[0].Value.ToString()!= "")
                {
                    iduser = int.Parse(datagridUsuarios.CurrentRow.Cells[0].Value.ToString());
                    int i = datagridUsuarios.CurrentRow.Index;  // indice de la fila
                    txtnombre.Text = datagridUsuarios.Rows[i].Cells[1].Value.ToString();
                    txtusuario.Text = datagridUsuarios.Rows[i].Cells[2].Value.ToString();
                    txtpass.Text = datagridUsuarios.Rows[i].Cells[3].Value.ToString();
                    cmbAutoriza.SelectedItem = datagridUsuarios.Rows[i].Cells[4].Value.ToString();
                    btnAlta.Enabled = false;
                }
                else { moduloInicio.LimpiarTexto(this); cmbAutoriza.SelectedIndex = -1;
                    MessageBox.Show("selecciona uno o debe dar de alta Usuario"); btnAlta.Enabled = true;
                }
               
            }
        }

        private void btnAlta_Click(object sender, EventArgs e)
        {
            if (txtnombre.Text == ""){ MessageBox.Show("Introduce Nombre"); txtnombre.Focus(); return; }
            if (txtusuario.Text == "") { MessageBox.Show("Introduce el usuario"); txtusuario.Focus(); return; }
            if (txtpass.Text == "") { MessageBox.Show("Introduce la contraseña"); txtpass.Focus(); return; }

            if (cmbAutoriza.SelectedIndex==-1)  { MessageBox.Show("selecciona autorización"); cmbAutoriza.Focus();
                return; }    
         
                using(var ConexionContext = new MyDbContext())
                {                               
                  Usuario miusuario = new Usuario {Nombre=txtnombre.Text,User=txtusuario.Text,Contrasenia=txtpass.Text,
                    Autorizacion=cmbAutoriza.SelectedItem.ToString()};
               
                    if (moduloInicio.Existe("select * from pyme.usuarios where User='" + txtusuario.Text + "'or Contrasenia='" + txtpass.Text + "';"))
                    {
                        MessageBox.Show("Usuario o Contraseña repetidos");
                    }
                    else
                    {
                        ConexionContext.Usuarios.Add(miusuario);
                        ConexionContext.SaveChanges();
                    }
                
            }
            Recarga();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {          
             if (txtnombre.Text != "")
            {                
                if (txtnombre.Text == "") { MessageBox.Show("Introduce Nombre"); txtnombre.Focus(); return; }
                if (txtusuario.Text == "") { MessageBox.Show("Introduce el usuario"); txtusuario.Focus(); return; }
                if (txtpass.Text == "") { MessageBox.Show("Introduce la contraseña"); txtpass.Focus(); return; }

                if (cmbAutoriza.SelectedIndex == -1)
                {
                    MessageBox.Show("selecciona autorización"); cmbAutoriza.Focus(); return;
                }
                
               using (MyDbContext context = new MyDbContext())
               {
                Usuario usuario = context.Usuarios.Where(x => x.IdUsuario == iduser).FirstOrDefault();
                usuario.Nombre = txtnombre.Text;
                usuario.User = txtusuario.Text;
                usuario.Contrasenia = moduloInicio.ControlContraseña(txtpass.Text, context, iduser);
                usuario.Autorizacion = cmbAutoriza.SelectedItem.ToString();
                  context.SaveChanges();
                    Recarga();
               }
                btnAlta.Enabled = true;
            }
            else { MessageBox.Show("no se ha seleccionado usuario"); }
          
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (txtnombre.Text != "")
            {                           
                var idCtl = moduloInicio.ObtenerIdControl(iduser);  //listado controles del usuario
              int i = 0;
            while(moduloInicio.Existe("select * from pyme.controls where IdUsuario=" + iduser + ";")&& i<idCtl.Count)
            {
               if(!moduloInicio.Existe("select * from pyme.trabajadorcontrols where Control_IdControl=" + idCtl[i].IdControl + ";"))
                {
                    if(!moduloInicio.Existe("select * from pyme.cursocontrols where Control_IdControl=" + idCtl[i].IdControl + ";"))
                    {
                        if (!moduloInicio.Existe("select * from pyme.epicontrols where Control_IdControl=" + idCtl[i].IdControl + ";"))
                        {
                            if (!moduloInicio.Existe("select * from pyme.periodocontrols where Control_IdControl=" + idCtl[i].IdControl + ";"))
                            {
                                if(!moduloInicio.Existe("select * from pyme.agenda where IdUsuario=" + iduser + ";"))
                                {
                                
                                }
                                else { MessageBox.Show("no se puede eliminar, agenda asociada"); return; }

                            }
                            else { MessageBox.Show("no se puede eliminar, controles/periodos asociados");return; }
                        }
                        else { MessageBox.Show("no se puede eliminar, controles/epi asociados");return; }
                    }
                    else { MessageBox.Show("no se puede eliminar, controles/curso asociados"); return; }
                }
                else { MessageBox.Show("no se puede eliminar, controles/trabajador asociados");return; }
                i++;
            }
                // eliminar controles de un usuario
                moduloInicio.OperarSql("delete from pyme.controls where IdUsuario=" + iduser + ";");

                if (MessageBox.Show("Este proceso borra el usuario " + datagridUsuarios.CurrentRow.Cells[1].Value.ToString().ToUpper() + " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                     
                        using (MyDbContext context = new MyDbContext())
                        {
                            Usuario usuario = context.Usuarios.Where(x => x.IdUsuario == iduser).FirstOrDefault();
                            context.Usuarios.Remove(usuario);
                            context.SaveChanges();
                        }
                    Recarga();
                }
                btnAlta.Enabled = true;
            }  else {MessageBox.Show("Usuario no seleccionado");} 
                         
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);
            cmbAutoriza.SelectedIndex = -1;
            btnAlta.Enabled = true;
        }

        private void btnControl_Click(object sender, EventArgs e)
        {
            var controlesUsuario = moduloInicio.ObtenerIdControl(iduser);
            if (MessageBox.Show("Este proceso borra los controles del usuario: " + datagridUsuarios.CurrentRow.Cells[1].Value.ToString().ToUpper() + " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (var controlUsuario in controlesUsuario)
                {
                  moduloInicio.OperarSql("delete from pyme.trabajadorcontrols where Control_IdControl=" + controlUsuario.IdControl + ";");
                  moduloInicio.OperarSql("delete from pyme.periodocontrols where Control_IdControl=" + controlUsuario.IdControl + ";");
                  moduloInicio.OperarSql("delete from pyme.cursocontrols where Control_IdControl=" + controlUsuario.IdControl + ";");
                  moduloInicio.OperarSql("delete from pyme.epicontrols where Control_IdControl=" + controlUsuario.IdControl + ";");
                }

                moduloInicio.OperarSql("delete from pyme.controls where IdUsuario=" + iduser + ";");

                if (MessageBox.Show("Este proceso borra la agenda del usuario: " + datagridUsuarios.CurrentRow.Cells[1].Value.ToString().ToUpper() + " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    moduloInicio.OperarSql("delete from pyme.agenda where IdUsuario=" + iduser + ";");
                }
                    
            }
            else { MessageBox.Show("Usuario no seleccionado"); }
        }
    }
}
