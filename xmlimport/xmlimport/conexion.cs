using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmlimport
{
    public class conexion
    {
        static SqlConnection dbcon;
        SqlDataAdapter da;
        static string connectionString = @"Data Source = NTBOOK ; Initial Catalog = lcd; Integrated Security = true";
        public static int val = 0;
        private static conexion instance = null;        
        public static conexion GetInstance()
        {
            if (instance == null) instance = new conexion();
            return instance;
        }
        public void open()
        {            
            dbcon = new SqlConnection(connectionString);
            dbcon.Open();
        }
        public void close() {
            dbcon.Close();
        }
        public void spExecute(string spName) {
            using (SqlConnection dbcon1 = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand comand = new SqlCommand(spName, dbcon1);
                    comand.CommandTimeout = 0;
                    comand.CommandType = CommandType.StoredProcedure;
                    dbcon1.Open();
                    comand.ExecuteNonQuery();
                    comand.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void spAgregarContribuyente2(string RFC, string Validez, string Estatus, string no, string fFinal, string fInicio)
        {
            using (SqlConnection dbcon1 = new SqlConnection(connectionString)) {                 
                try {
                    SqlCommand comand = new SqlCommand("InsertarContribuyente", dbcon1);
                    dbcon1.Open();
                    comand.CommandType = CommandType.StoredProcedure;
                    comand.Parameters.AddWithValue("@rfc", RFC);
                    comand.Parameters.AddWithValue("@validez", Validez);
                    comand.Parameters.AddWithValue("@status", Estatus);
                    comand.Parameters.AddWithValue("@noCert", no);
                    comand.Parameters.AddWithValue("@fFinal", fFinal);
                    comand.Parameters.AddWithValue("@fInicio", fInicio);
                    comand.ExecuteNonQuery();
                    comand.Dispose();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
        }
        public void ExecuteQueryNonResult(string query)
        {
            using (SqlConnection dbcon1 = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand comand = new SqlCommand(query, dbcon1);
                    dbcon1.Open();
                    comand.ExecuteNonQuery();
                    comand.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            da = new SqlDataAdapter(query, dbcon);
            da.Fill(dt);
            return dt;
        }
    }

    //Inicia
    public class Contribuyente
    {
        public string _RFC;
        public string _ValidezObligaciones;
        public string _EstatusVerficado;
        public string _noCertificado;
        public string _fInicio;
        public string _fFinal;
        //public Certificado _cert;
        public Contribuyente()
        {
            _RFC = null;
        }
        /*public Contribuyente(string RFC, Certificado certificado)
        {
            _RFC = RFC;
            _cert = new Certificado();
            _cert = certificado;
        }*/
        public Contribuyente(string RFC, string ValidezObligaciones, string EstatusVerficado, string noCertificado, string fInicio, string fFinal)
        {
            _RFC = RFC;            
            _ValidezObligaciones = ValidezObligaciones;
            _EstatusVerficado = EstatusVerficado;
            _noCertificado = noCertificado;
            _fInicio = fInicio;
            _fFinal = fFinal;
        }
    }
    public class Certificado
    {
        public string _ValidezObligaciones;
        public string _EstatusVerficado;
        public string _noCertificado;
        public string _fInicio;
        public string _fFinal;
        public Certificado()
        {
            _ValidezObligaciones = null;
            _EstatusVerficado = null;
            _noCertificado = null;
            _fInicio = null;
            _fFinal = null;
        }
        public Certificado(string ValidezObligaciones, string EstatusVerficado, string noCertificado, string fInicio, string fFinal)
        {
            _ValidezObligaciones = ValidezObligaciones;
            _EstatusVerficado = EstatusVerficado;
            _noCertificado = noCertificado;
            _fInicio = fInicio;
            _fFinal = fFinal;
        }
    }
}
