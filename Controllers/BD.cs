using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using WSGlobal.Models;

namespace WSGlobal.Controllers
{
    public class DataResponse
    {
        public List<Usuario> lstUsuarios{ get; set; }
        public List<SessionLog> lstSessionLog{ get; set; }
      
        public string ID { get; set; } = "";
        public string Status { get; set; } = "";
        public string Message { get; set; } = "";
    }
    public class DatosInstaciaBDD
    {
        
        public String Tipo { get; set; } = "Local";
        public static String ConexionLocal1 { get; set; } = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=GLOBAL_BMW;User ID=sa;Password=electronica";
        public static String ConexionLocal2 { get; set; } = @"metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=localhost\SQLEXPRESS;initial catalog=GLOBAL_BMW;persist security info=True;user id=sa;password=electronica;multipleactiveresultsets=True;application name=EntityFramework&quot;";
        public static String ConexionServer1 { get; set; } = @"Data Source=SW300048\SQLEXPRESS;Initial Catalog=GLOBAL_BMW;User ID=sa;Password=electronica";
        public static String ConexionServer2 { get; set; } = @"metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=SW300048\SQLEXPRESS;initial catalog=GLOBAL_BMW;persist security info=True;user id=sa;password=electronica;multipleactiveresultsets=True;application name=EntityFramework&quot;";
        public void SetConexion()
        {
            if (Tipo == "Local")
            {
                //ConfigurnewConnectionString("ConexionSQLLocal", ConexionLocal1, "System.Data.SqlClient");
                //
                //ConfigurnewConnectionString("GLOBAL_BMW1", ConexionLocal2, "System.Data.EntityClient");
                //ConfigurnewConnectionString("ConexionSQLLocal", @"DESKTOP-N2NIQMG\SQLEXPRESS", "GLOBAL_BMW", "sa", "electronica",0,"");
                //ConfigurnewConnectionString("GLOBAL_BMW1", @"DESKTOP-N2NIQMG\SQLEXPRESS", "GLOBAL_BMW", "sa", "electronica",1, ConexionLocal2);


                //var settings = ConfigurationManager.ConnectionStrings["ConexionSQLLocal"];
                //var fi = typeof(ConfigurationElement).GetField(
                //              "_bReadOnly",
                //              BindingFlags.Instance | BindingFlags.NonPublic);
                //fi.SetValue(settings, false);
                //settings.ConnectionString = ConexionLocal1;


                //var settings2 = ConfigurationManager.ConnectionStrings["GLOBAL_BMW1"];
                //var fi2 = typeof(ConfigurationElement).GetField(
                //              "_bReadOnly",
                //              BindingFlags.Instance | BindingFlags.NonPublic);
                //fi2.SetValue(settings2, false);
                //settings2.ConnectionString = ConexionLocal2;

                //AddUpdateAppSettings("ConexionSQLLocal", ConexionLocal1);
                //ConfigurationManager.ConnectionStrings["ConexionSQLLocal"].ConnectionString = ConexionLocal1;





            }
            else
            {

                //var settings = ConfigurationManager.ConnectionStrings["ConexionSQLLocal"];
                //var fi = typeof(ConfigurationElement).GetField(
                //              "_bReadOnly",
                //              BindingFlags.Instance | BindingFlags.NonPublic);
                //fi.SetValue(settings, false);
                //settings.ConnectionString = ConexionServer1;


                //var settings2 = ConfigurationManager.ConnectionStrings["GLOBAL_BMW1"];
                //var fi2 = typeof(ConfigurationElement).GetField(
                //              "_bReadOnly",
                //              BindingFlags.Instance | BindingFlags.NonPublic);
                //fi2.SetValue(settings2, false);
                //settings2.ConnectionString = ConexionServer2;
                //ConfigurationManager.ConnectionStrings["ConexionSQLLocal"].ConnectionString = ConexionServer1;
                //ConfigurationManager.ConnectionStrings["GLOBAL_BMW1"].ConnectionString = ConexionServer2;
            }

        }
        public void ConfigurnewConnectionString(string nameConetion, string SQLConn, string ProviderName)
        {
            
            //string str = "server=" + server + ";database=" + database + "; User ID=" + userid + "; Password=" + password + "";
            //Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //str = System.Web.Configuration.WebConfigurationManager.AppSettings["myKey"];
            //myConfiguration.Save();
            System.Configuration.Configuration Config1 = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection conSetting = (ConnectionStringsSection)Config1.GetSection("connectionStrings");
            ConnectionStringSettings StringSettings = new ConnectionStringSettings(nameConetion, SQLConn, ProviderName);
            conSetting.ConnectionStrings.Remove(StringSettings);
            conSetting.ConnectionStrings.Add(StringSettings);
            Config1.Save(ConfigurationSaveMode.Modified);
            //Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //myConfiguration.AppSettings.Settings.Item("myKey").Value = txtmyKey.Text;
            //myConfiguration.Save();
        }

        public void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
    public class BDD
    {
        
        public static String Conexion { get; set; } = "";

   



        private SqlConnection AbreConexion(string BD)
        {
            SqlConnection sqlConn;
             Conexion = ConfigurationManager.ConnectionStrings["ConexionSQLLocal"].ToString();
            

            sqlConn = new SqlConnection(Conexion);
            sqlConn.Open();
            if (sqlConn.State == ConnectionState.Open)
            {
                //AgregaLog("Se conecto")
                return sqlConn;
            }
            else
            {
                //AgregaLog("No se conecto")
                return null;
            }
        }

        public DataTable GetSQL(string sSql, string BD)
        {
            DataTable dT = new DataTable();
            SqlConnection sqlConn = new SqlConnection();
            try
            {
                if (sqlConn == null)
                {
                    sqlConn = AbreConexion(BD);
                    if (sqlConn == null)
                    {
                        return dT;
                    }
                    //If Not AbreConexion(ws) Then
                    //    Return dT
                    //End If
                }
                else
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn = AbreConexion(BD);
                        if (sqlConn == null)
                        {
                            return dT;
                        }
                        //If Not AbreConexion(ws) Then
                        //    Return dT
                        //End If
                    }
                }
                SqlCommand sqlCmd = new SqlCommand(sSql, sqlConn);
                SqlDataAdapter sqlDA;
                sqlDA = new SqlDataAdapter(sSql, sqlConn);
                sqlDA.Fill(dT);

                if (dT.Rows.Count == 0)
                {
                    //AgregaLog(sSql);
                    //AgregaLog(sqlConn.State);
                    //AgregaLog(sqlConn.ConnectionString);
                }
                sqlConn.Close();
                GC.Collect();
                //
                //If dT.Rows.Count > 0 Then
                //    For Each row As DataRow In dT.Rows
                //        'Dim tam = row.ItemArray.Count
                //        'For i As Integer = 0 To (tam - 1)
                //        '    Dim x = row(i)
                //        '    DatosQuery.Add("1")
                //        'Next
                //        Resultado = row.ItemArray
                //    Next
                //End If
            }
            catch (Exception ex)
            {
                //AgregaLog("Error en GetSQL: " + sSql.ToString());
                //AgregaLog(ex.Message);
                //MsgBox("GetSQL: sSql=" + sSql.ToString + " - " + ex.Message.ToString)
            }

            return dT;
        }

        public int SetSQL(string sSql, string BD)
        {
            int RegistrosAfectados = 0;
            int Idinsertado = 0;
            SqlConnection sqlConn = new SqlConnection();
            try
            {
                if (sqlConn == null)
                {
                    sqlConn = AbreConexion(BD);
                    if (sqlConn == null)
                    {
                        return 0;
                    }
                }
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn = AbreConexion(BD);
                    if (sqlConn == null)
                    {
                        return 0;
                    }
                }
                SqlCommand sqlCmd = new SqlCommand(sSql, sqlConn);
                RegistrosAfectados = sqlCmd.ExecuteNonQuery();

                GC.Collect();
            }
            catch (Exception ex)
            {
                // AgregaLog("Error: " + ex.Message.ToString());
            }
            return RegistrosAfectados;
        }


    }
}