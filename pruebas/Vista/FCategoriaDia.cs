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
    public partial class FCategoriaDia : Form
    {
        ModuloTexto moduloTexto = new ModuloTexto();
        ModuloInicio moduloInicio = new ModuloInicio();
        int idc;
        int idd;
        public FCategoriaDia()
        {
            InitializeComponent();
            datagridDia.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.tipodias;");
            dataGridCategoria.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.categorias");
            dataGridCategoria.Columns[0].Visible = false;
            datagridDia.Columns[0].Visible = false;
            if (moduloInicio.ObtenerAutorizacion() != "A")
            {
                btnCarga.Visible = false;
            }


        }

        #region metodos tipodia
        private void btnCarga_Click(object sender, EventArgs e)
        {
            moduloTexto.AbrirLeer();
        }

        private void datagridDia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                if (datagridDia.CurrentRow.Cells[1].Value.ToString() != "")
                {
                    idd = int.Parse(datagridDia.CurrentRow.Cells[0].Value.ToString());
                    txtNombreD.Text = datagridDia.CurrentRow.Cells[1].Value.ToString();
                    txtimporteD.Text = datagridDia.CurrentRow.Cells[2].Value.ToString();
                      btnAltaD.Enabled = false;          
                }
                else { moduloInicio.LimpiarTexto(this); btnAltaD.Enabled = true; 
                    MessageBox.Show("seleccione o debe dar de alta tipo");  }
                
            }
        }


        private void txtNombreD_Validating(object sender, CancelEventArgs e)
        {
            if (txtNombreD.Text != "")
            {
                if (moduloInicio.IsNumeric(txtNombreD.Text))
                {MessageBox.Show("Introduce un tipo de día"); e.Cancel = true; }
                  
            }
        }

        private void txtimporteD_Validating(object sender, CancelEventArgs e)
        {
            if (txtimporteD.Text != "")
            {
                if (!moduloInicio.isnumericDouble(txtimporteD.Text) || !moduloInicio.IsNumeric(txtimporteD.Text))
                {MessageBox.Show("Introduce un importe"); e.Cancel = true; }
            
            }
        }

        private void btnAltaD_Click(object sender, EventArgs e)
        {            
            if (txtNombreD.Text == "")
            {  MessageBox.Show("Introduce Denominación"); txtNombreD.Focus(); return;}
               
            if (txtimporteD.Text == "")
            {MessageBox.Show("Introduce importe");txtimporteD.Focus();return; }
                 
                using (var ConexionContext = new MyDbContext())
                {
                    TipoDia Nuevotipo;
                    Nuevotipo = new TipoDia { Denominacion = txtNombreD.Text.ToUpper(), Importe = double.Parse(txtimporteD.Text) };

                    if (!moduloInicio.Existe("select Denominacion from pyme.tipodias where Denominacion='" + txtNombreD.Text + "';"))
                    {
                    ConexionContext.TipoDias.Add(Nuevotipo);
                    ConexionContext.SaveChanges();


                    datagridDia.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.tipodias;");
                    }
                   else { MessageBox.Show("El tipo de día ya existe"); }
                }
            moduloInicio.LimpiarTexto(this);
        }

        private void btnModificarD_Click(object sender, EventArgs e)
        {
            if(txtNombreD.Text!=""&& txtimporteD.Text != "")
            {                
                using(var ConexionContext = new MyDbContext())
                {
                    TipoDia tipo = ConexionContext.TipoDias.Where(x => x.IdTipoDia == idd).FirstOrDefault();
                   
                   tipo.Denominacion= moduloInicio.ControlarModificar("select Denominacion " +
                        "from pyme.tipodias where Denominacion='" + txtNombreD.Text.ToUpper() + "';", "select Denominacion " +
                        "from pyme.tipodias where IdTipoDia=" + idd + ";", 0, txtNombreD.Text);
                    tipo.Importe = double.Parse(txtimporteD.Text);
                    ConexionContext.SaveChanges();
                }
               datagridDia.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.tipodias;");
               moduloInicio.LimpiarTexto(this); btnAltaD.Enabled = true;
            }
            else { MessageBox.Show("Seleccione un tipo"); }
           
        }

      

        private void btneliminarD_Click(object sender, EventArgs e)
        {
            if (txtNombreD.Text != "" && txtimporteD.Text != "")
                {
                if (!moduloInicio.Existe("select IdTipoDia from pyme.periodoes where IdTipoDia=" + idd + ";"))
                {
                    using (var context = new MyDbContext())
                    {
                        if (MessageBox.Show("Este proceso borra tipo día " +
                            datagridDia.CurrentRow.Cells[1].Value.ToString().ToUpper() +
                            " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            TipoDia tipo = context.TipoDias.Where(x => x.IdTipoDia == idd).FirstOrDefault();
                            context.TipoDias.Remove(tipo);
                            context.SaveChanges();
                        }
                    }
                    datagridDia.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.tipodias;");
                    
                }
                else { MessageBox.Show("Periodo con Tipo"); }
                moduloInicio.LimpiarTexto(this); btnAltaD.Enabled = true;
            }
            else { MessageBox.Show("selecciona un tipo de día"); }
           
        }

        private void btnCancelarD_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);
            btnAltaD.Enabled = true; btnAltaC.Enabled = true;
        }


        #endregion

        #region metodos categoria
        private void dataGridCategoria_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {  
                if (dataGridCategoria.CurrentRow.Cells[0].Value.ToString() != "")
                {
                    idc = int.Parse(dataGridCategoria.CurrentRow.Cells[0].Value.ToString());
                    txtnombreC.Text = dataGridCategoria.CurrentRow.Cells[1].Value.ToString();
                    txtImporteC.Text = dataGridCategoria.CurrentRow.Cells[2].Value.ToString();
                    btnAltaC.Enabled = false;
                }
                else { moduloInicio.LimpiarTexto(this); btnAltaC.Enabled = true;
                    MessageBox.Show("sin seleccionar o debe dar de alta categoría");  }
               
            }
        }

        private void txtnombreC_Validating(object sender, CancelEventArgs e)
        {
            if (txtnombreC.Text != "")
            {
                if (moduloInicio.IsNumeric(txtnombreC.Text)) { MessageBox.Show("introduce una categoría"); e.Cancel = true; }
            }
        }

        private void txtImporteC_Validating(object sender, CancelEventArgs e)
        {
            if (txtImporteC.Text != "")
            {
                if (!moduloInicio.IsNumeric(txtImporteC.Text) || !moduloInicio.isnumericDouble(txtImporteC.Text))
                { MessageBox.Show("introduce un importe"); e.Cancel = true; }
            }
        }

        private void btnAltaC_Click(object sender, EventArgs e)
        {
            if (txtnombreC.Text == "") { MessageBox.Show("Introduce categoría"); txtnombreC.Focus(); return; }
            if (txtImporteC.Text == "") { MessageBox.Show("Introduce categoría"); txtImporteC.Focus(); return; }

            using (var ConexionContext = new MyDbContext())
            {                
                if (!moduloInicio.Existe("select Nombre from pyme.categorias where Nombre='" + txtnombreC.Text + "';"))
                {
                    Categoria Nuevacategoria = new Categoria { Nombre = txtnombreC.Text.ToUpper(),
                        Dinero = double.Parse(txtImporteC.Text)};
                    ConexionContext.Categorias.Add(Nuevacategoria);
                    ConexionContext.SaveChanges();
                    dataGridCategoria.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.categorias");
                   
                }
                else { MessageBox.Show("Existe trabajadores con esta categoría"); }
            }
            moduloInicio.LimpiarTexto(this);
        }
        
        private void btnModificarC_Click(object sender, EventArgs e)
        {
            if(txtnombreC.Text!="" && txtImporteC.Text != "")
            {                
                using (var context= new MyDbContext())
                {
                    Categoria categoria = context.Categorias.Where(x => x.IdCategoria == idc).FirstOrDefault();
                  
                   categoria.Nombre= moduloInicio.ControlarModificar("select Nombre " +
                        "from pyme.categorias where Nombre='" + txtnombreC.Text.ToUpper() + "';", "select Nombre " +
                        "from pyme.categorias where IdCategoria=" + idc + ";", 0, txtnombreC.Text);
                    categoria.Dinero = double.Parse(txtImporteC.Text);
                    context.SaveChanges();
                }
              dataGridCategoria.DataSource = moduloInicio.CargaGridyCombo("select * from pyme.categorias");
              moduloInicio.LimpiarTexto(this);btnAltaC.Enabled = true;
            }
            else { MessageBox.Show("seleccione una categoría"); }
            
        }

        private void btneliminarC_Click(object sender, EventArgs e)
        {
            if (txtnombreC.Text != "" && txtImporteC.Text != "")
            {
                if (!moduloInicio.Existe("select IdCategoria from pyme.trabajadors where IdCategoria=" + idc + ";"))
                {
                    using (var context = new MyDbContext())
                    {
                        if (MessageBox.Show("Este proceso borra categoría " + dataGridCategoria.CurrentRow.Cells[1].Value.ToString().ToUpper() + " de la bd, lo quieres hacer S/N", "CUIDADO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Categoria categoria = context.Categorias.Where(x => x.IdCategoria == idc).FirstOrDefault();
                            context.Categorias.Remove(categoria);
                            context.SaveChanges();
                        }
                    }
                }
                else { MessageBox.Show("Trabajador con Categoría, debe eliminar el trabajador"); }
                     moduloInicio.LimpiarTexto(this); btnAltaC.Enabled = true;
            }
            else { MessageBox.Show("selecciona una categoría"); }
            
        }


        private void btnCancelarC_Click(object sender, EventArgs e)
        {
            moduloInicio.LimpiarTexto(this);
            btnAltaC.Enabled = true; btnAltaD.Enabled = true;
        }

        #endregion

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }

}
