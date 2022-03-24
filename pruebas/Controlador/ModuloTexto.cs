using pruebas.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using objWord = Microsoft.Office.Interop.Word;

namespace pruebas.Controlador
{
   public class ModuloTexto
    {
        ModuloInicio moduloInicio = new ModuloInicio();
        ModuloFechas moduloFechas = new ModuloFechas();
       public string rutah = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\horas.docx";
       public string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +"\\Vacaciones.docx";
       public  string rutaAB = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ALTA_BAJA.docx";

        public void AbrirLeer()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            bool vale = true;
          
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        do
                        {                           
                            fileContent = reader.ReadLine();
                            if (fileContent != "" && fileContent!=null){vale=  ComprobarLinea(fileContent); } else { vale = false; }
                            
                        } while (fileContent!= ""&& vale);   //continua leyendo mientras tenga contenido y dias validos
             
                    }
                }else { MessageBox.Show("Seleccione un archivo"); }
            }
           
        }

        public bool ComprobarLinea(string linea) // evitar ficheros no validos
        {
            bool correcto;
            if (linea != "" && ControlDias(linea))
            {              
                correcto = true;
            }
            else{  MessageBox.Show("fichero vacio o erroneo");
                correcto = false; }
     
            return correcto;
        }


        public bool ControlDias(string dia) //controlar que no se metan los mismos dias
        {
            bool correcto;
            if (dia.Length == 10)
            {
                using(var Context = new MyDbContext())
                {
                    if(!moduloInicio.Existe("select FechaFestivo from pyme.festivoes where FechaFestivo='" + dia + "';"))
                    {
                      Festivo NuevoFestivo = new Festivo();
                      NuevoFestivo.FechaFestivo=dia;
                       Context.Festivos.Add(NuevoFestivo);
                       Context.SaveChanges();
                    }
                }
                correcto = true;
            }
            else { MessageBox.Show("error dato " + dia + " dato no cargado", "CUIDADO", MessageBoxButtons.OK);
                correcto = false; }
                              
          return correcto;
        }

     
        public void CargaTablaTitulo( objWord.Document objDocumento,  string titulo, string variable)
         {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";

            objWord.Range wrdRng = objDocumento.Bookmarks.get_Item(ref oEndOfDoc).Range;

            objWord.Table Otable = objDocumento.Tables.Add(wrdRng, 1, 1, ref oMissing, ref oMissing);
                       
            Otable.Borders[objWord.WdBorderType.wdBorderLeft].Visible = true;
            Otable.Borders[objWord.WdBorderType.wdBorderRight].Visible = true;
            Otable.Borders[objWord.WdBorderType.wdBorderTop].Visible = true;
            Otable.Borders[objWord.WdBorderType.wdBorderBottom].Visible = true;
            Otable.Cell(1, 1).Range.Text = titulo + "" + variable;
            Otable.Range.Bold = 12; Otable.Range.Font.Size = 15;

        }

        public void CargaParrafo(objWord.Paragraph objParrfafo,string titulo, string variable, int spacio)
        {            
            objParrfafo.Range.Font.Size = 12;
            objParrfafo.Range.Font.Color = objWord.WdColor.wdColorBlack;
            objParrfafo.Range.Text = titulo +""+ variable;
            objParrfafo.Format.SpaceBefore = spacio;
            //objParrfafo1.Format.SpaceAfter = 40;
            objParrfafo.Range.InsertParagraphAfter();
        }


        public void GenerarWordDias(string nombre,  DateTime fecha1, DateTime fecha2)
        {           
                //documento
                objWord.Application objAplicacion = new objWord.Application();
                objWord.Document objDocumento = objAplicacion.Documents.Add();

                //tabla
                CargaTablaTitulo(objDocumento, "VACACIONES ", nombre.ToUpper());

                //parrafo
                objWord.Paragraph objParrfafo1 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo1, "EL TRABAJADOR " + nombre.ToUpper() +
                    " CON DNI/NIE: ", moduloInicio.TrabajadoresEmpresa().Where(x => x.Nombre == nombre).FirstOrDefault().Dni +
                    " QUE DESARROLLA SU ACTIVIDAD PROFESIONAL COMO TRABAJADOR, " +
                    " EN LA EMPRESA JOSE ANTONIO, SL RECONOCE QUE: ", 50);

                objWord.Paragraph objParrfafo2 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo2, " DISFRUTA, " + moduloFechas.OpcionDias(fecha1, fecha2),
                     ", DE LAS VACACIONES PERTENECIENTES AL AÑO  " + DateTime.Today.Year, 140);

                objWord.Paragraph objParrfafo3 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo3, "EN BURGOS A.  ",
                    DateTime.Today.ToLongDateString().ToUpper(), 240);

                objWord.Paragraph objParrfafo4 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo4, "FDO.  ", nombre.ToUpper(), 40);

                objWord.Paragraph objParrfafo5 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo5, "DNI/NIE: ", moduloInicio.TrabajadoresEmpresa()
                    .Where(x => x.Nombre == nombre).FirstOrDefault().Dni, 5);

            
                objDocumento.SaveAs2(ruta);
                objDocumento.Close();
                objAplicacion.Quit();
           
        }

        public string MovimientoTrabajador(bool alta,string fecha)
        { string dato = "";
            if (alta) { return dato="ALTA: " + fecha; }
            else { return dato = "BAJA: "+ moduloFechas.ObtenerFecha(DateTime.Today); }
        }

        public void GenerarWordAlta_Baja(Trabajador eltrabajador, string cate, bool activar)
        {
           
              string[] misfechas;
        //documento
            objWord.Application objAplicacion = new objWord.Application();
            objWord.Document objDocumento = objAplicacion.Documents.Add();

            //tabla
            CargaTablaTitulo(objDocumento, "ALTA/BAJA TRABAJADOR  ", eltrabajador.Nombre.ToUpper());

            //parrafos
            objWord.Paragraph objParrfafo = objDocumento.Content.Paragraphs.Add(Type.Missing);
            CargaParrafo(objParrfafo, "FECHA ", MovimientoTrabajador(activar,eltrabajador.FechaAlta), 50);

            objWord.Paragraph objParrfafo1 = objDocumento.Content.Paragraphs.Add(Type.Missing);
            CargaParrafo(objParrfafo1, "NOMBRE:  " + eltrabajador.Nombre.ToUpper() +
                "  DNI/NIE: ", eltrabajador.Dni, 20);
         
            objWord.Paragraph objParrfafo2 = objDocumento.Content.Paragraphs.Add(Type.Missing);
            CargaParrafo(objParrfafo2, " Nº SEGURIDAD SOCIAL: ", eltrabajador.Nseguridads.ToString(), 20);
               
            objWord.Paragraph objParrfafo3 = objDocumento.Content.Paragraphs.Add(Type.Missing);
            CargaParrafo(objParrfafo3, "DIRECCIÓN: ",   eltrabajador.Direccion, 20);

            objWord.Paragraph objParrfafo5 = objDocumento.Content.Paragraphs.Add(Type.Missing);
            CargaParrafo(objParrfafo5, "CATEGORÍA: ", cate, 20);

            if (!activar && eltrabajador.Nombre!="") //comprobar si es baja o alta, recibe parametro Activo
            {
                objWord.Paragraph objParrfafo4 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo4, "BAJA: ", " FIN DE OBRA", 20);

                misfechas = moduloFechas.FechasWordBaja(eltrabajador.IdTrabajador).ToArray();
                objWord.Paragraph objParrfafo6 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo6, "VACACIONES DISFRUTADAS:  " , String.Join(",",misfechas), 20);
                //cadena string del listado
            }
            else
            {  
                objWord.Paragraph objParrfafo4 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo4, "FECHA NACIMIENTO: ", "         ", 20);

                objWord.Paragraph objParrfafo6 = objDocumento.Content.Paragraphs.Add(Type.Missing);
                CargaParrafo(objParrfafo6, "CONTRATO:  " + "         ", " OBRA: ", 20);

            }
           

            objDocumento.SaveAs2(rutaAB);

            objDocumento.Close();
            objAplicacion.Quit();
        }

        public void GenerarWordHoras(string valor, string elmes, double horas)
        {   
            
            if (System.IO.File.Exists(rutah))
            {
                              
                object ObjMiss = System.Reflection.Missing.Value;

                Trabajador trabajador = moduloInicio.TrabajadoresEmpresa().Where(x => x.Nombre == valor).FirstOrDefault();
                //documento referencias en el word las pasamos a objetos

                objWord.Application objAplicacion = new objWord.Application();
                
                object miruta = rutah;
                object nombre = "trabajador"; object nombre1 = "trabajador1"; object nombre2 = "trabajador2";
                object nombre3 = "trabajador3"; object nombre4 = "trabajador4";
                object dni = "dni"; object dni1 = "dni1";
                object mes = "meses"; object mes1 = "meses1"; object mes2 = "meses2";
                object elaño = "años";
                object total = "total";
                object fechaHoy = "fechaH"; object fechaHoy1 = "fechaH1";
                objWord.Document objDocumento = objAplicacion.Documents.Open(miruta, ref ObjMiss); 
                // los objetos los pasamos a objWord
                objWord.Range nom = objDocumento.Bookmarks.get_Item(ref nombre).Range;
                nom.Text = trabajador.Nombre;   
                objWord.Range nom1 = objDocumento.Bookmarks.get_Item(ref nombre1).Range;
                nom1.Text = trabajador.Nombre;
                objWord.Range nom2 = objDocumento.Bookmarks.get_Item(ref nombre2).Range;
                nom2.Text = trabajador.Nombre;
                objWord.Range nom3 = objDocumento.Bookmarks.get_Item(ref nombre3).Range;
                nom3.Text = trabajador.Nombre;
                objWord.Range nom4 = objDocumento.Bookmarks.get_Item(ref nombre4).Range;
                nom4.Text = trabajador.Nombre;
                objWord.Range dn = objDocumento.Bookmarks.get_Item(ref dni).Range;
                dn.Text = trabajador.Dni;
                objWord.Range dn1 = objDocumento.Bookmarks.get_Item(ref dni1).Range;
                dn1.Text = trabajador.Dni;
                objWord.Range m = objDocumento.Bookmarks.get_Item(ref mes).Range;
                m.Text = elmes;
                objWord.Range m1 = objDocumento.Bookmarks.get_Item(ref mes1).Range;
                m1.Text = elmes;
                objWord.Range m2 = objDocumento.Bookmarks.get_Item(ref mes2).Range;
                m2.Text = elmes;
                objWord.Range año = objDocumento.Bookmarks.get_Item(ref elaño).Range;
                año.Text = DateTime.Today.Year.ToString();
                objWord.Range fecha2 = objDocumento.Bookmarks.get_Item(ref fechaHoy).Range;
                fecha2.Text = DateTime.Today.ToLongDateString();
                objWord.Range fecha3 = objDocumento.Bookmarks.get_Item(ref fechaHoy1).Range;
                fecha3.Text = DateTime.Today.ToLongDateString();
                objWord.Range importe = objDocumento.Bookmarks.get_Item(ref total).Range;
                importe.Text = (horas * trabajador.Valor).ToString();
                //rango
                object rango1 = nom; object rango11 = nom1; object rango12 = nom2; object rango13 = nom3; object rango14 = nom4;
                object rango2 = dn; object rango21 = dn1;
                object rango3 = m; object rango31 = m1; object rango32 = m2;
                object rango4 = año; object rango5 = fecha2; object rango51 = fecha3; object rango6 = importe;
                objDocumento.Bookmarks.Add("trabajador", ref rango1); objDocumento.Bookmarks.Add("trabajador1", ref rango11);
                objDocumento.Bookmarks.Add("trabajador2", ref rango12); objDocumento.Bookmarks.Add("trabajador3", ref rango13);
                objDocumento.Bookmarks.Add("trabajador4", ref rango14);
                objDocumento.Bookmarks.Add("dni", ref rango2); objDocumento.Bookmarks.Add("dni1", ref rango21);
                objDocumento.Bookmarks.Add("meses", ref rango3); objDocumento.Bookmarks.Add("meses1", ref rango31);
                objDocumento.Bookmarks.Add("meses2", ref rango32);
                objDocumento.Bookmarks.Add("años", ref rango4);
                objDocumento.Bookmarks.Add("fechaH", ref rango5); objDocumento.Bookmarks.Add("fechaH1", ref rango51);
                objDocumento.Bookmarks.Add("total", ref rango6);
                objDocumento.Close();
                objAplicacion.Quit();
            
                
            }
            else { MessageBox.Show("el archivo no existe"); }
          
        }

        public bool isFileOpen(string ruta)
        {            
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenWrite(ruta);
                fs.Close();
            }
            catch (IOException) { return  true; }

            return  false;
        }

    }
}
