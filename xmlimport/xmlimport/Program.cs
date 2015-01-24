using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace xmlimport
{
    public class Global
    {
        private static Global Instancia = null;
        public static int val = 0;
        public static Global GetInstance()
        {
            if (Instancia == null) Instancia = new Global();
            return Instancia;
        }
        public int Get() {
            val += 30000;
            return val;
        }
    }

    class Program
    {
        string strPath = @"D:\GITHUB\";
        static Program me;
        int contRegs = 0;
        List<Contribuyente> _lista;
        Contribuyente[] arrayC;
        static void Main(string[] args)
        {
            me = new Program();
            string[] dwFiles = new string[4] { "A1.gz", "A2.gz", "A3.gz", "A4.gz" };
            string[] XML = new string[4] { "LCO_2014-12-10_1", "LCO_2014-12-10_2", "LCO_2014-12-10_3", "LCO_2014-12-10_4" };
            string[] XMLFiles = new string[4] { "A1.xml", "A2.xml", "a3.xml", "a4.xml" };
            string[] XMLFiles1 = new string[4] { "lco1.xml", "lco2.xml", "lco3.xml","lco4.xml" };
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            //me.DownloadFiles(dwFiles);
            //me.unzipFiles(dwFiles);
            //me.CleaningFiles(XMLFiles, XMLFiles1);
            ////Console.WriteLine("Cargando XML a Lista...");
            ////me.LoadXML2List(XMLFiles1[0]);
            ////me.Insertar();
            me.ExecuteSP();
            
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Finalizado");
            Console.ReadKey();
        }        
        public void ejecuta()
        {                            
                int i = Global.GetInstance().Get();
                int limite = i + 30000;
                int len = arrayC.Length;
                //conexion con = new conexion();
                Console.WriteLine("INSERTANDO " + i + " a" + limite);
                while (i < limite && i < len)
                {
                    conexion.GetInstance().spAgregarContribuyente2(arrayC[i]._RFC, arrayC[i]._ValidezObligaciones, arrayC[i]._EstatusVerficado, arrayC[i]._noCertificado, arrayC[i]._fFinal, arrayC[i]._fInicio);                 
                    //con.spAgregarContribuyente2(arrayC[i]._RFC, arrayC[i]._ValidezObligaciones, arrayC[i]._EstatusVerficado, arrayC[i]._noCertificado, arrayC[i]._fFinal, arrayC[i]._fInicio);                 
                    i++;
                }
        }
        #region ManXML
        void Insertar() {
            conexion con = new conexion();
            //string connectionString = @"Data Source = NTBOOK ; Initial Catalog = lcd; Integrated Security = true";
            //string query = "";
            Parallel.ForEach(_lista, _persona =>
            {
                con.spAgregarContribuyente2(_persona._RFC, _persona._ValidezObligaciones, _persona._EstatusVerficado, _persona._noCertificado, _persona._fFinal, _persona._fInicio);
                //query = "insert into LCO values('" + _persona._RFC + "', '" + _persona._ValidezObligaciones + "', '" + _persona._EstatusVerficado + "', '" + _persona._noCertificado + "','" + _persona._fFinal + "', '" + _persona._fInicio + "')";                
                //using (SqlConnection dbcon1 = new SqlConnection(connectionString))
                //{
                //    try
                //    {
                //        SqlCommand comand = new SqlCommand("InsertarContribuyente", dbcon1);
                //        dbcon1.Open();
                //        comand.CommandType = CommandType.StoredProcedure;
                //        comand.Parameters.AddWithValue("@rfc", _persona._RFC);
                //        comand.Parameters.AddWithValue("@validez", _persona._ValidezObligaciones);
                //        comand.Parameters.AddWithValue("@status", _persona._EstatusVerficado);
                //        comand.Parameters.AddWithValue("@noCert", _persona._noCertificado);
                //        comand.Parameters.AddWithValue("@fFinal", _persona._fFinal);
                //        comand.Parameters.AddWithValue("@fInicio", _persona._fInicio);
                //        comand.ExecuteNonQuery();
                //        comand.Dispose();
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //    }
                //}
            });
            /*
             * EJECUTA()
             * int j = Global.val;
            int Longitud = arrayC.Length;
            if (j < Longitud)
            {
                new Thread(new ThreadStart(ejecuta)).Start();
                //Thread.Sleep(500);
                Insertar();
            }*/
            /*
             * INSERTA SECUENCIALMENTE
             * int i = 0;
            int len = arrayC.Length;
            Console.WriteLine("INSERTANDO " + i + " a" + len);
            do
            {
                conexion.GetInstance().spAgregarContribuyente2(arrayC[i]._RFC, arrayC[i]._ValidezObligaciones, arrayC[i]._EstatusVerficado, arrayC[i]._noCertificado, arrayC[i]._fFinal, arrayC[i]._fInicio);
                i++;
            } while (i < len);*/
        }
        void LoadXML2List(string strPathXML)
        {
            Contribuyente Persona;
            _lista = new List<Contribuyente>();

            string _rfc, _ValidezObligaciones, _EstatusVerficado, _noCertificado, _fInicio, _fFinal = "";
            string url = strPath + strPathXML;
            using (XmlTextReader reader = new XmlTextReader(url))
            {
                _rfc = "";
                reader.MoveToContent();
                while (reader.Read())
                {
                    _ValidezObligaciones = "";
                    _EstatusVerficado = "";
                    _noCertificado = "";
                    _fInicio = "";
                    _fFinal = "";
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "lco:Contribuyente")) {
                        _rfc = reader.GetAttribute(0);
                    }
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "lco:Certificado"))
                    {
                        _ValidezObligaciones=reader.GetAttribute(0);
                        _EstatusVerficado=reader.GetAttribute(1);
                        _noCertificado=reader.GetAttribute(2);
                        _fFinal=reader.GetAttribute(3);
                        _fInicio=reader.GetAttribute(4);
                        Persona = new Contribuyente(_rfc, _ValidezObligaciones, _EstatusVerficado, _noCertificado, _fFinal, _fInicio);
                        _lista.Add(Persona);
                    }                    
                }
            }
            //arrayC = new Contribuyente[_lista.Count];
            //arrayC = _lista.ToArray();
            //_lista = null;
            //Console.WriteLine("ARRAY CARGADO ");

        }    
        void LoadXMLToDB(string strPathXML) {
            /*
             * RECORRER XML USANDO XMLDOCUMENT
             * */
            Contribuyente Persona;
            _lista = new List<Contribuyente>();
            string _rfc, _ValidezObligaciones, _EstatusVerficado, _noCertificado, _fInicio, _fFinal = null;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(strPath+strPathXML);
            Console.WriteLine("XML DOCUMENT Loaded.");
            Console.WriteLine("Insertando registros en Base de Datos");
            XmlNodeList lista = xDoc.GetElementsByTagName("lco:Contribuyente");
            foreach (XmlElement nodo in lista)
            {
                _rfc = nodo.GetAttribute("RFC");
                XmlNodeList lista2 = nodo.GetElementsByTagName("lco:Certificado");
                foreach (XmlElement nodo2 in lista2)
                {                    
                    _ValidezObligaciones = nodo2.GetAttribute("ValidezObligaciones");
                    _EstatusVerficado = nodo2.GetAttribute("EstatusCertificado");
                    _noCertificado = nodo2.GetAttribute("noCertificado");
                    _fFinal = nodo2.GetAttribute("FechaFinal");
                    _fInicio = nodo2.GetAttribute("FechaInicio");
                    Persona = new Contribuyente(_rfc, _ValidezObligaciones, _EstatusVerficado, _noCertificado, _fFinal, _fInicio);
                    _lista.Add(Persona);
                    //con.spAgregarContribuyente(_rfc, _ValidezObligaciones, _EstatusVerficado, _noCertificado, _fFinal, _fInicio);
                }
            }

            //Contribuyente[] arrayC = new Contribuyente[_lista.Count];
            //arrayC = _lista.ToArray();
            //Console.WriteLine(_lista.Count);
            Console.WriteLine("ARREGLO COMPLETO");
        }
        #endregion ManXML
        void DownloadFiles(string[] nombres)
        {
            string DirPath = @"D:\GITHUB\";
            string _url = @"http://www.gestionix.com/Zip/";
            var ClienteWeb = new System.Net.WebClient();
            /*Parallel.ForEach(nombres, _nombre =>
            {
                var ClienteWeb = new System.Net.WebClient();
                ClienteWeb.Proxy = null;
                Console.WriteLine("Descargando archivo " + _url + _nombre);
                ClienteWeb.DownloadFile(_url + _nombre, DirPath + _nombre);
                Console.WriteLine(_nombre + "Descargado correctamente");
            });*/
            for (int i = 0; i < nombres.Length;i++ ) {                
                ClienteWeb.Proxy = null;
                Console.WriteLine("Descargando archivo " + _url + nombres[i]);
                ClienteWeb.DownloadFile(_url + nombres[i], DirPath + nombres[i]);
                Console.WriteLine(nombres[i] + "Descargado correctamente");
            }
        }
        void unzipFiles(string[] nombres)
        {
            string DirPath;
            for (int i = 0; i < nombres.Length; i++)
            {
                DirPath = @"D:\GITHUB\";
                DirPath += nombres[i];
                Console.WriteLine("Descomprimiendo [" + nombres[i] + "]");
                DirectoryInfo direc = new DirectoryInfo(DirPath);
                FileInfo archivo = new FileInfo(DirPath);
                FileStream ArchivoOriginal = archivo.OpenRead();
                string NombreArchivo = archivo.FullName;
                string NuevoNombre = NombreArchivo.Remove(NombreArchivo.Length - archivo.Extension.Length);
                FileStream ArchivoDescomprimido = File.Create(NuevoNombre + ".xml");
                GZipStream Descomprimir = new GZipStream(ArchivoOriginal, CompressionMode.Decompress);
                Descomprimir.CopyTo(ArchivoDescomprimido);
                Console.WriteLine(nombres[i] + " Unziped");
            }
        }
        void ExecuteCommand(string _Command)
        {
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + _Command);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
        }
        void CleaningFiles(string[] xml, string[] nombres)
        {
            string DirPath = @"D:\GITHUB\";
            for (int i = 0; i < xml.Length; i++)
            {
                Console.WriteLine("Cleaning [" + xml[i] + "]");
                ExecuteCommand(@"D:\GITHUB\openssl\openssl smime -decrypt -verify -inform DER -in " + DirPath + xml[i] + " -noverify -out " + DirPath+nombres[i] );
                Console.WriteLine(nombres[i] + " Cleaned");
            }
        }
        void ExecuteSP() {
            List<String> SPNames = new List<String>();
            SPNames.Add("XMLIMPORT1");
            SPNames.Add("XMLIMPORT2");
            SPNames.Add("XMLIMPORT3");
            SPNames.Add("XMLIMPORT4");            
            /*Parallel.ForEach(SPNames, _SP =>
            {
                conexion con = new conexion();
                Console.WriteLine("execute "+_SP+" ...");
                con.spExecute(_SP);
            });
             */
            foreach (String _SP in SPNames)
            {
                conexion con = new conexion();
                Console.WriteLine("execute " + _SP + " ...");
                con.spExecute(_SP);
            }

        }
        
    }
}
