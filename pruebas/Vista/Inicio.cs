using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using pruebas.Controlador;
using pruebas.Vista;
using pruebas.Modelo;

namespace pruebas
{
    public partial class Form1 : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        public Form1()
        {
            InitializeComponent();   
            moduloInicio.InsertStartData();
            
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

       

        private void btnaceptar_Click(object sender, EventArgs e)
        {
            if (moduloInicio.ObtenerUsuario(txtUsuario.Text, txtPass.Text) != null)
            {
                Constants.Id_usuario = moduloInicio.ObtenerUsuario(txtUsuario.Text, txtPass.Text).IdUsuario;
                moduloInicio.OperarSql("Insert into pyme.controls(Fecha_inicio,IdUsuario) values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + Constants.Id_usuario + ");");
                FMenu menuPrincipal = new FMenu();
                menuPrincipal.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Usuario o Contraseña incorretos");
                txtPass.Clear();
                txtUsuario.Clear();
                txtUsuario.Focus();
            }
        }

        private void btnminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btncerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Mover el formulario
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0x0f012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0x0f012, 0);
        }

        private void txtUsuario_Enter(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "USUARIO")
            {   
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.Black;
               
            }
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtUsuario.Text = "USUARIO";
                txtUsuario.ForeColor = Color.LightGray; 

            }
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            if (txtPass.Text == "CONTRASEÑA")
            {
                txtPass.Text = "";
                txtPass.UseSystemPasswordChar = true;
                txtPass.ForeColor = Color.Black;
            }
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (txtPass.Text == "")
            {
                txtPass.Text = "CONTRASEÑA";
                txtPass.UseSystemPasswordChar = false;
                txtPass.ForeColor = Color.LightGray;
            }
        }
    }
}
