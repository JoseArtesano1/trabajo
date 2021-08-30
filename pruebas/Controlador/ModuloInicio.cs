using MySql.Data.MySqlClient;
using pruebas.Modelo;
using pruebas.Vista;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pruebas.Controlador
{
    public class ModuloInicio
    {                                
        string conexion = "server=localhost;User Id=root; Persist Security Info=True;database=Pyme;password=root";// cadena de conexión para trabajar con syntax smysql
              public  string[] consultas = new string[] { "CURSOS", "ALERTAS", "HORAS", "PERIODOS" };
              public  string[] consultas2 = new string[] { "CURSOS", "ALERTAS", "HORAS", "PERIODOS", "CONTROLES" };
              public string[] years = new string[] { "2021", "2022", "2023", "2024", "2025", "2026", "2027", "2028", "2029", "2030" };


         public void InsertStartData()
        {
            using (var ConexionContext = new MyDbContext())
            {
                var registroCount = ConexionContext.Usuarios.Count();

                if (registroCount == 0)
                {
                    ConexionContext.Usuarios.Add(new Usuario { Nombre = "Jose", User = "Jose", Contrasenia = "123j", Autorizacion = "A" });

                    ConexionContext.SaveChanges();
                }

            }
        }

        public Usuario ObtenerUsuario(string nombre, string pass)
        {
            Usuario usuario;
            using (var ConexionContext = new MyDbContext())
            {
                usuario = ConexionContext.Usuarios.Where(x => x.User == nombre && x.Contrasenia == pass).FirstOrDefault();

            }
            return usuario;
        }

       

       public List<Trabajador> TrabajadoresEmpresa()
        {
            List<Trabajador> listado = new List<Trabajador>();
            using (var ConexionContext = new MyDbContext())
            {
                listado=ConexionContext.Trabajadors.ToList();
            }
            return listado;
        }

        public Categoria ObtenerCategoria(int idCat)
        {
            Categoria categoria;
            using (var ConexionContext = new MyDbContext())
            {
                categoria = ConexionContext.Categorias.Where(x => x.IdCategoria == idCat).FirstOrDefault();
            }
            return categoria;

        }

        public List<Agenda> ObtenerAgenda()
        {
            List<Agenda> agendaCompleta = new List<Agenda>();
            using (var ConexionContext = new MyDbContext())
            {
                agendaCompleta = ConexionContext.Agendas.ToList();
            }
            return agendaCompleta;
        }
       
        public DataTable CargaGridyCombo(string sql)
        {
            using (MySqlConnection con = new MySqlConnection(conexion))
            {
                MySqlCommand cm = new MySqlCommand(sql, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }

        }

        public string ObtenerAutorizacion()
        {
            string AutorizacionLog;
            using (var Context = new MyDbContext())
            {
                AutorizacionLog = Context.Usuarios.Where(x => x.IdUsuario == Constants.Id_usuario).FirstOrDefault().Autorizacion;
            }

            return AutorizacionLog;
        }

        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        public bool isnumericDouble(string txtbox)
        {
            return double.TryParse(txtbox, out double valor);
        }

        public bool isnumericLong(string txtbox)
        {
            return long.TryParse(txtbox, out long valor);
        }

          

        public void LimpiarTexto(System.Windows.Forms.Control root)
        {
            foreach (System.Windows.Forms.Control ctrl in root.Controls)
            { if (ctrl is TextBox || ctrl is DateTimePicker)
                {  //ctrl.Enabled = false;
                    ctrl.Text = String.Empty;
                }
                else
                {  if (ctrl.Controls.Count > 0) { LimpiarTexto(ctrl); }
                }
            }
        }

        public void LimpiarComboyCheck(System.Windows.Forms.Control root)
        {
            foreach (System.Windows.Forms.Control ctrl in root.Controls)
            {
                if (ctrl is ComboBox)
                {
                    ctrl.Text = String.Empty;
                }
                else if (ctrl is CheckBox)
                {
                    CheckBox checkbox = ctrl as CheckBox;
                    checkbox.Checked = false;
                }
                else
                {
                    if (ctrl.Controls.Count > 0) { LimpiarComboyCheck(ctrl); }
                }
            }

        }

        public  void bloqueo(System.Windows.Forms.Control root, System.Windows.Forms.Control root1,
            System.Windows.Forms.Control root2, System.Windows.Forms.Control root3, System.Windows.Forms.Control root4, System.Windows.Forms.Control root5, bool activar)
        {           
            root.Visible = activar;
            root1.Visible = activar;
            root2.Enabled = activar;
            root3.Visible = activar;
            root4.Visible = activar;
            root5.Visible = activar;
        }

       
        public DataTable CargaGridCategoria(System.Windows.Forms.Control root, System.Windows.Forms.Control root1,
            System.Windows.Forms.Control root2, System.Windows.Forms.Control root3, System.Windows.Forms.Control root4, System.Windows.Forms.Control root5)
        {
            if (ObtenerAutorizacion() == "A")
            {
                return CargaGridyCombo("select * from pyme.usuarios");
            }
            else
            {
                bloqueo(root, root1, root2, root3, root4,root5, false);
                return CargaGridyCombo("select * from pyme.usuarios where IdUsuario=" + Constants.Id_usuario + ";");
            }
        }



        public bool Existe(string sql)
        {
            bool existen=false;
            using (MySqlConnection con = new MySqlConnection(conexion))
            using (MySqlCommand comando = new MySqlCommand(sql, con))
            {
                try
                {
                    con.Open();
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.HasRows) { existen = true; }
                        else{  existen=false;  }
                     }
                }
                catch (Exception ex){MessageBox.Show("Error: " + ex.Message); }
             }
            return existen;
        }

        public string ControlContraseña(string pass, MyDbContext context, int idUsuario)
        {
            var usuarios = context.Usuarios.ToList();// listado de todos
            Usuario miusuario= context.Usuarios.Where(x => x.IdUsuario == idUsuario).FirstOrDefault(); //nuestro usuario
            bool existe=false; int i = 0;
           
            while(existe==false&& i < usuarios.Count)
            {
                if (usuarios[i].Contrasenia == pass)  //comprobar que existe el usuario
                {
                    if (miusuario.Contrasenia == pass) {  existe = true;} // si coincide con el actual devolvemos el parametro
              
                    else
                    {    MessageBox.Show("la contraseña ya existe");
                        return miusuario.Contrasenia; // devolvemos el usuario actual guardado
                    }
                 }
                 i++;
            }
            return pass;
        }

          
        public void OperarSql(string sql)    // insertar modificar y borrar
        {
            using (MySqlConnection con = new MySqlConnection(conexion))
            {
                using (MySqlCommand comando = new MySqlCommand(sql, con))
                {
                    con.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void ModificarTrabajador(string dni, string nombre, string direccion, string alta, string fdni,
            string fmed, int mov, long ss, int val, int idcat, bool act, string fperm, int idt)
        {
            string sql = @"UPDATE  pyme.trabajadors SET Dni=@dni, Nombre = @nombre, Direccion = @direc, Telefono = @tef, Nseguridads = @sso, FechaAlta = @alt, FechaMedico = @med, FechaDni = @fdn, Valor = @va, Activo = @ac, IdCategoria=@idc,FechaPermiso =@fper where IdTrabajador=@idt";
          
            using (MySqlConnection con = new MySqlConnection(conexion))
            {
                con.Open();
                using (MySqlCommand comando = new MySqlCommand(sql, con))
                {                    
                    comando.Parameters.AddWithValue("@dni", dni);
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@direc", direccion);
                    comando.Parameters.AddWithValue("@tef", mov);
                    comando.Parameters.AddWithValue("@sso", ss);
                    comando.Parameters.AddWithValue("@alt", alta);
                    comando.Parameters.AddWithValue("@med", fmed);
                    comando.Parameters.AddWithValue("@fdn", fdni);
                    comando.Parameters.AddWithValue("@va", val);
                    comando.Parameters.AddWithValue("@ac", act);
                    comando.Parameters.AddWithValue("@idc", idcat);
                    comando.Parameters.AddWithValue("@fper", fperm);
                    comando.Parameters.AddWithValue("@idt", idt);
                     comando.ExecuteNonQuery();
                }
            }
        }

        public void EliminarCursoTrabajador( int c, int t)
        {  string sql = @"DELETE from pyme.trabajadorcursoes WHERE Curso_IdCurso=@idc and  Trabajador_IdTrabajador =@idt";

            using (MySqlConnection con = new MySqlConnection(conexion))
            {
                con.Open();
                using (MySqlCommand comando = new MySqlCommand(sql, con))
                {
                    comando.Parameters.AddWithValue("@idc", c);
                    comando.Parameters.AddWithValue("@idt", t);
                    comando.ExecuteNonQuery();
                }
            }
        }

        public List<Modelo.Control> ObtenerIdControl(int idU)
        {
            List<Modelo.Control> ids = new List<Modelo.Control>();
         
            using (var contexto = new MyDbContext())
            {
                ids= contexto.Controls.Where(x => x.IdUsuario == idU).ToList();
            }
            return ids;
        }

        public int ObtenerId(string sql)
        {    int id=0;
            using (MySqlConnection con = new MySqlConnection(conexion))
            using (MySqlCommand comando = new MySqlCommand(sql, con))
            {
                try
                {   con.Open();
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {  id= reader.GetInt32(0); }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
            return id;
        }

      // controla no repetir modificaciones

        public string Obtenerdato(string sql, int posicion)
        {
            string nombre="";
            using (MySqlConnection con = new MySqlConnection(conexion))
            using (MySqlCommand comando = new MySqlCommand(sql, con))
            {  try
                {
                    con.Open();
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        { nombre=reader.GetString(posicion);}
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
            return nombre;
        }


        public string ControlarModificar( string sql1,string sql2,int posicion,string nombre)
        {
            if (Existe(sql1))
            {
                return Obtenerdato(sql2, posicion); 
            }
          return nombre.ToUpper();
        }


    }
}
