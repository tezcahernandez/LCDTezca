using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace WebApplication1
{
    public class ProcesoLCO
    {
        public void Main() {
            string[] dwFiles = new string[4] { "A1.gz", "A2.gz", "A3.gz", "A4.gz" };
            string[] XMLFiles = new string[4] { "A1.xml", "A2.xml", "a3.xml", "a4.xml" };
            string[] XMLFiles1 = new string[4] { "lco1.xml", "lco2.xml", "lco3.xml", "lco4.xml" };
            ////Stopwatch stopWatch = new Stopwatch();
            ////stopWatch.Start();
            ChangeState(true);
            if (!DownloadFiles(dwFiles)) return;
            if(!unzipFiles(dwFiles))return;
            if(!CleaningFiles(XMLFiles, XMLFiles1)) return;
            if(!ExecuteSP()) return;
            ////stopWatch.Stop();
            ////TimeSpan ts = stopWatch.Elapsed;
            ////var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }
        bool DownloadFiles(string[] nombres)
        {
            string DirPath = @"D:\GITHUB\";
            string _url = @"http://www.gestionix.com/Zip/";
            using (WebClient ClienteWeb = new WebClient())
            {
                try
                {
                    for (int i = 0; i < nombres.Length; i++)
                    {
                        ClienteWeb.Proxy = null;
                        ClienteWeb.DownloadFile(_url + nombres[i], DirPath + nombres[i]);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    WriteLog("Download "+ex.Message);
                }
            }
            ChangeState(false);
            return false;
        }
        bool unzipFiles(string[] nombres)
        {
            string DirPath;
            for (int i = 0; i < nombres.Length; i++)
            {
                DirPath = @"D:\GITHUB\";
                DirPath += nombres[i];
                try
                {
                    FileInfo archivo = new FileInfo(DirPath);
                    FileStream ArchivoOriginal = archivo.OpenRead();
                    string NombreArchivo = archivo.FullName;
                    string NuevoNombre = NombreArchivo.Remove(NombreArchivo.Length - archivo.Extension.Length);
                    FileStream ArchivoDescomprimido = File.Create(NuevoNombre + ".xml");
                    GZipStream Descomprimir = new GZipStream(ArchivoOriginal, CompressionMode.Decompress);
                    Descomprimir.CopyTo(ArchivoDescomprimido);
                }
                catch (Exception e)
                {
                    WriteLog("Unzip " + e.Message);
                    ChangeState(false);
                    return false;
                }
            }
            return true;
        }
        bool ExecuteCommand(string _Command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + _Command);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = false;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
                return true;
            }
            catch (Exception e)
            {
                WriteLog("Exec Command " + e.Message);
            }
            ChangeState(false);
            return false;
        }
        bool CleaningFiles(string[] xml, string[] nombres)
        {
            string DirPath = @"D:\GITHUB\";
            Thread.Sleep(100);
            for (int i = 0; i < xml.Length; i++)
            {
                if (!ExecuteCommand(@"D:\GITHUB\openssl\openssl smime -decrypt -verify -inform DER -in " + DirPath + xml[i] + " -noverify -out " + DirPath + nombres[i])) ChangeState(false); return false; 
            }
            return true;
        }
        bool ExecuteSP()
        {
            List<String> SPNames = new List<String>();
            SPNames.Add("XMLIMPORT1");
            SPNames.Add("XMLIMPORT2");
            SPNames.Add("XMLIMPORT3");
            SPNames.Add("XMLIMPORT4");
            foreach (String _SP in SPNames)
            {
                if (!spExecute(_SP)) ChangeState(false);  return false;
            }
            return true;
        }
        bool spExecute(string spName)
        {
            string connectionString = @"Data Source = NTBOOK ; Initial Catalog = lcd; Integrated Security = true";
            using (SqlConnection dbcon1 = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand comand = new SqlCommand(spName, dbcon1);
                    comand.CommandTimeout = 0;
                    comand.CommandType = CommandType.StoredProcedure;
                    dbcon1.Open();
                    WriteLog("SP Executing " + spName);
                    comand.ExecuteNonQuery();
                    comand.Dispose();
                }
                catch (Exception ex)
                {
                    WriteLog("SP Exec " + ex.Message);
                    ChangeState(false);
                    return false;
                }
            }
            return true;
        }
        void WriteLog(string linea) {
            string fic = @"D:\GITHUB\log.txt";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fic,true);
            sw.WriteLine(DateTime.Now.ToString() + " ERROR: " + linea);
            sw.Close();
        }
        void ChangeState(bool state)
        {
            string fic = @"D:\GITHUB\status.txt";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fic);
            sw.WriteLine(state);
            sw.Close();
        }
    }
}