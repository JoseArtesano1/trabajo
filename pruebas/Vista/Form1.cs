using pruebas.Controlador;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pruebas.Vista
{
    public partial class FMenu : Form
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        public FMenu()
        {
            InitializeComponent();
        }

        #region diseño formulario

        private int tolerance = 12;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;


        //mover formulario

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        //----------------DIBUJAR RECTANGULO / EXCLUIR ESQUINA PANEL 
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));

            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);

            region.Exclude(sizeGripRectangle);
            this.panelContenedor.Region = region;
            this.Invalidate();
        }
        //----------------COLOR Y GRIP DE RECTANGULO INFERIOR
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(244, 244, 244));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);
                       
            //base.OnPaint(e);
            //ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
        }

        private void btncerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //localizacion
        int lx, ly;
        int sw, sh;

        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0x0f012, 0);
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            lx = this.Location.X;
            ly = this.Location.Y;
            sw = this.Size.Width;
            sh = this.Size.Height;
            btnMaximizar.Visible = false;
            btnRestaurar.Visible = true;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
        }

      

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            btnMaximizar.Visible = true;
            btnRestaurar.Visible = false;
            this.Size = new Size(sw, sh);
            this.Location = new Point(lx, ly);
        }

       


        #endregion

        //abrir formulario en el panel

        private void AbrirFormulario<MiForm>()where MiForm:Form, new()
        {
            Form formulario;
            
            formulario = panelformularios.Controls.OfType<MiForm>().FirstOrDefault(); // buscar el form en la coleccion
           
            if (formulario == null)  // si no existe
            {
                formulario = new MiForm();
                formulario.TopLevel = false;
                formulario.FormBorderStyle = FormBorderStyle.None;
                formulario.Dock = DockStyle.Fill;
                panelformularios.Controls.Add(formulario);
                panelformularios.Tag = formulario;
                formulario.Show();
                formulario.BringToFront();
                //moduloInicio.LimpiarTexto(this); moduloInicio.LimpiarCombo(this); 
                formulario.FormClosed += new FormClosedEventHandler(CerrarForm);
            }
            else
            {
                //moduloInicio.LimpiarTexto(this); moduloInicio.LimpiarCombo(this);
                formulario.BringToFront();
            }
           
        }


        private void btnUsuario_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FUsuario>();
            btnUsuario.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnCategoriaDia_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FCategoriaDia>();
            btnCategoriaDia.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnCursoEpi_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FCursoEpi>();
            btnCursoEpi.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnHoras_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FAsignarDias>();
            btnHoras.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FAsignacion>();
            btnAsignar.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnConsulta_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FConsulta>();
            btnConsulta.BackColor= Color.FromArgb(12, 61, 92);
        }

        private void btnAgenda_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FAgenda>();
            btnAgenda.BackColor = Color.FromArgb(12, 61, 92);
        }

        private void btnTrabajador_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FTrabajador>();
            btnTrabajador.BackColor = Color.FromArgb(12, 61, 92);
        }


        private void CerrarForm(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms["FAgenda"] == null)
            {
                btnAgenda.BackColor = Color.FromArgb(4, 41, 68);
            }

            if (Application.OpenForms["FUsuario"] == null)
            {
                btnUsuario.BackColor = Color.FromArgb(4, 41, 68);
            }
            if (Application.OpenForms["FTrabajador"] == null)
            { 
                btnTrabajador.BackColor = Color.FromArgb(4, 41, 68);
            }
            if (Application.OpenForms["FCategoriaDia"] == null)
            {
                btnCategoriaDia.BackColor = Color.FromArgb(4, 41, 68);
            }
            if (Application.OpenForms["FCursoEpi"] == null)
            {
                btnCursoEpi.BackColor = Color.FromArgb(4, 41, 68);
            }
            if (Application.OpenForms["FAsignarDias"] == null)
            {
                btnHoras.BackColor = Color.FromArgb(4, 41, 68);
            }
            if (Application.OpenForms["FAsignacion"] == null)
            {
                btnAsignar.BackColor = Color.FromArgb(4, 41, 68);
            }
            if (Application.OpenForms["FConsulta"] == null)
            {
                btnConsulta.BackColor = Color.FromArgb(4, 41, 68);
            }


        }
    }
}
