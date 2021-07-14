
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using WSGlobal.Models;




namespace WSGlobal.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST,GET")]
    [Authorize]
    [RoutePrefix("Home/users")]
    
    public class WsUsuariosController : ApiController
    {
        BDD BD = new BDD();
        [HttpGet]
        [Route("Index")]
        public IHttpActionResult Index()
        {
            try
            {

                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<GetUserDataTag> ListUsuarios = entities.GetUserDataTags.ToList();
                if (ListUsuarios.Count == 0)
                {
                    ListUsuarios.Add(new GetUserDataTag());
                }
                return Ok(ListUsuarios);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
              
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Ok(mensaje);
            }
        }

        [HttpGet]
        [Route("getUsuariosAll")]
        public IHttpActionResult getUsuariosAll(String Status)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<GetUserDataTag> ListUsuarios = entities.GetUserDataTags.Where(s => s.Status == Status).ToList();
                if (ListUsuarios.Count > 0)
                {
                    return Ok(ListUsuarios);
                }
                else
                {
                    ListUsuarios.Add(new GetUserDataTag());
                    return Ok(ListUsuarios);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Ok(mensaje);
            }
        }

        [HttpGet]
        [Route("getUsers")]
        public IHttpActionResult getUsers(String Id)
        {
            try
            {
                int Ids = Int32.Parse(Id);
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<GetUserDataTag> ListUsuarios = entities.GetUserDataTags.Where(s => s.ID == Ids).ToList();
                if (ListUsuarios.Count > 0)
                {
                    return Json(ListUsuarios);
                }
                else
                {
                    ListUsuarios.Add(new GetUserDataTag());
                    return Json(ListUsuarios);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult  GetAll()
        {
            try
            {
                Models.GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Usuario> ListUsuarios = entities.Usuarios.ToList();
                if (ListUsuarios.Count == 0)
                {
                    ListUsuarios.Add(new Usuario());
                }
                return Ok(ListUsuarios);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return null;
            }
        }
        public static string DecryptStringAES(string cipherText)
        {
            var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings 
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform. 
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption. 
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream 
                                // and place them in a string. 
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }
        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(Usuario usuario)
        {
            try
            {
                DataResponse data = new DataResponse();
                if (usuario.UserName != null && usuario.UserName != "")
                {
                    GLOBAL_BMW1 entities = new GLOBAL_BMW1();

                    var passs = DecryptStringAES(usuario.Password);
                    var user = DecryptStringAES(usuario.UserName);

                    List<Usuario> ListUsuarios = entities.Usuarios.Where(s => s.UserName == user && s.Password == passs).OrderByDescending(s => s.ID).ToList();
                    if (ListUsuarios.Count > 0)
                    {
                        ListUsuarios[0].Password = "";
                        data.lstUsuarios = ListUsuarios;
                        data.Message = "LOGIN OK";
                        data.Status = "OK";
                        return Json(data);
                    }
                    else
                    {
                        data.Message = "USER NO FOUND";
                        data.Status = "ERROR";
                        return Json(data);
                    }
                }
                else
                {
                    data.Message = "USER NO FOUND";
                    data.Status = "ERROR";
                    return Json(data);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("LoginTag")]
        public IHttpActionResult LoginTag(String Tag, int IDSystem, int IDBuilding)
        {
            try
            {

                List<DatosLoginTag_Perfiles> lstPerfiles = new List<DatosLoginTag_Perfiles>();

                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<GetUserDataTag> ListUsuarios = entities.GetUserDataTags.Where(s => s.Tag1 == Tag).ToList();
                if (ListUsuarios.Count > 0)
                {
                    List<DatosLoginTag_Perfiles> lstPerfiles_userTag = new List<DatosLoginTag_Perfiles>();
                    var x = ListUsuarios[0].IDPerfilUsuario;
                    int IdPerfil = Int32.Parse(x.ToString());
                    DatosLoginTag_Perfiles Perfil1 = new DatosLoginTag_Perfiles();
                    Perfil1.ListGetUserData_Tag = ListUsuarios;
                    Perfil1.ID = ListUsuarios[0].ID;
                    Perfil1.PerfilName = ListUsuarios[0].NombrePerfil;

                    //BUSCAR EL SISTEMA
                    List<GetAllPerfil_Modulos> ListPerfil_Modulos;
                    var sistemas = entities.GetAllPerfil_Sistemas.Where(s => s.IDPerfil == IdPerfil && s.IDSistema == IDSystem).ToList();
                    if (sistemas.Count > 0)
                    {
                        //BUSCAS EL BUILDING
                        var edificios = entities.GetAllPerfil_Edificios.Where(s => s.IDPerfil == IdPerfil && s.IDEdificio == IDBuilding).ToList();
                        if (edificios.Count > 0)
                        {
                            ListPerfil_Modulos = entities.GetAllPerfil_Modulos.Where(s => s.IDPerfil == IdPerfil).ToList();
                            Perfil1.ListPerfil_Modulos = ListPerfil_Modulos;
                            lstPerfiles.Add(Perfil1);
                        }

                    }

                    else
                    {
                        lstPerfiles.Add(Perfil1);
                    }

                    return Json(lstPerfiles);
                    //Usuarios/LoginTag?Tag=12345&IDSystem=3&IDBuilding=1
                }
                else
                {

                    return Json(lstPerfiles);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }

        [HttpPost]
        [Route("LoginTag2")]
        public IHttpActionResult LoginTag2(DataHMI data)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Usuario> ListUsuarios = entities.Usuarios.Where(s => s.Tag1 == data.Tag || s.Tag2 == data.Tag).ToList();
                if (ListUsuarios.Count > 0)
                {
                    return Json(ListUsuarios);
                }
                else
                {
                    ListUsuarios.Add(new Usuario());
                    return Json(ListUsuarios);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("AddSessionLog")]
        public IHttpActionResult AddSessionLog(SessionLog sessionLog)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    entities.SessionLogs.Add(sessionLog);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("AddPerfilSystem")]
        public IHttpActionResult AddPerfilSystem(Perfil_Sistemas perfil_Sistemas)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    entities.Perfil_Sistemas.Add(perfil_Sistemas);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("AddPerfilEdificios")]
        public IHttpActionResult AddPerfilEdificios(Perfil_Edificios perfil_Edificios)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    entities.Perfil_Edificios.Add(perfil_Edificios);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("AddPerfilModulos")]
        public IHttpActionResult AddPerfilModulos(Perfil_Modulos perfil_Modulos)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    entities.Perfil_Modulos.Add(perfil_Modulos);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("AddPerfilUsuario")]
        public IHttpActionResult AddPerfilUsuario(Perfil perfilesUsuario)
        {
            int DataAfected = 1;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities1 = new GLOBAL_BMW1())
                {
                    using (var dbContextTransaction = entities1.Database.BeginTransaction())
                    {
                        try
                        {
                            entities1.PerfilesUsuarios.Add(new PerfilesUsuario { PerfilName = perfilesUsuario.PerfilName, Status = "true" });
                            var res = 0;
                            res = entities1.SaveChanges();

                            if (res >= 1)
                            {
                                List<PerfilesUsuario> ListItems = entities1.PerfilesUsuarios.Where(s => s.PerfilName == perfilesUsuario.PerfilName).ToList();
                                foreach (var item in perfilesUsuario.ListPerfil_Edificios)
                                {
                                    entities1.Perfil_Edificios.Add(new Perfil_Edificios { IDPerfil = ListItems[0].ID, EdificioName = item.EdificioName, IDEdificio = item.IDEdificio });
                                }

                                foreach (var item in perfilesUsuario.ListPerfil_Sistemas)
                                {
                                    entities1.Perfil_Sistemas.Add(new Perfil_Sistemas { IDPerfil = ListItems[0].ID, SistemaName = item.SistemaName, IDSistema = item.IDSistema });
                                }

                                foreach (var item in perfilesUsuario.ListPerfil_Modulos)
                                {
                                    entities1.Perfil_Modulos.Add(new Perfil_Modulos { IDPerfil = ListItems[0].ID, ModuloName = item.ModuloName, IDModulo = item.IDModulo, Nivel = item.Nivel });
                                }

                                var res2 = entities1.SaveChanges();
                                if (res2 <= 0)
                                {
                                    //hacemos rollback si fallo 
                                    data.Message = "FAILED INSERT";
                                    data.Status = "ERROR";
                                    dbContextTransaction.Rollback();
                                }
                                else
                                {
                                    //Hacemos commit de todos los datos
                                    data.Message = "SUCCESSFUL INSERT";
                                    data.Status = "OK";
                                    dbContextTransaction.Commit();
                                }
                            }
                            else
                            {
                                //hacemos rollback si fallo 
                                dbContextTransaction.Rollback();
                                data.Message = "FAILED INSERT";
                                data.Status = "ERROR";
                            }
                        }
                        catch (Exception ex)
                        {
                            //hacemos rollback si hay excepción
                            dbContextTransaction.Rollback();
                            data.Message = "FAILED INSERT";
                            data.Status = "ERROR";
                        }
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdatePerfilUsuario")]
        public IHttpActionResult UpdatePerfilUsuario(Perfil _perfilesUsuario)
        {
            int DataAfected = 1;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities1 = new GLOBAL_BMW1())
                {
                    using (var dbContextTransaction = entities1.Database.BeginTransaction())
                    {
                        try
                        {
                            var res = 0;
                            var result = entities1.PerfilesUsuarios.SingleOrDefault(b => b.ID == _perfilesUsuario.ID);
                            if (result != null)
                            {
                                result.PerfilName = _perfilesUsuario.PerfilName;
                                res = entities1.SaveChanges();
                                if (res >= 0)
                                {
                                    entities1.Perfil_Edificios.RemoveRange(entities1.Perfil_Edificios.Where(b => b.IDPerfil == result.ID));
                                    entities1.Perfil_Sistemas.RemoveRange(entities1.Perfil_Sistemas.Where(b => b.IDPerfil == result.ID));
                                    entities1.Perfil_Modulos.RemoveRange(entities1.Perfil_Modulos.Where(b => b.IDPerfil == result.ID));
                                    var res2 = entities1.SaveChanges();
                                    if (res2 <= 0)
                                    {
                                        //hacemos rollback si fallo 
                                        data.Message = "FAILED INSERT";
                                        data.Status = "ERROR";
                                        dbContextTransaction.Rollback();
                                    }
                                    else
                                    {
                                        //ahora insertamos los nuevos
                                        foreach (var item in _perfilesUsuario.ListPerfil_Edificios)
                                        {
                                            entities1.Perfil_Edificios.Add(new Perfil_Edificios { IDPerfil = result.ID, EdificioName = item.EdificioName, IDEdificio = item.IDEdificio });
                                        }
                                        foreach (var item in _perfilesUsuario.ListPerfil_Sistemas)
                                        {
                                            entities1.Perfil_Sistemas.Add(new Perfil_Sistemas { IDPerfil = result.ID, SistemaName = item.SistemaName, IDSistema = item.IDSistema });
                                        }
                                        foreach (var item in _perfilesUsuario.ListPerfil_Modulos)
                                        {
                                            entities1.Perfil_Modulos.Add(new Perfil_Modulos { IDPerfil = result.ID, ModuloName = item.ModuloName, IDModulo = item.IDModulo, Nivel = item.Nivel });
                                        }
                                        var res3 = entities1.SaveChanges();
                                        if (res3 <= 0)
                                        {
                                            //hacemos rollback si fallo 
                                            data.Message = "FAILED INSERT";
                                            data.Status = "ERROR";
                                            dbContextTransaction.Rollback();
                                        }
                                        else
                                        {
                                            //Hacemos commit de todos los datos
                                            data.Message = "SUCCESSFUL INSERT";
                                            data.Status = "OK";
                                            dbContextTransaction.Commit();
                                        }
                                    }
                                }
                                else
                                {
                                    //hacemos rollback si fallo 
                                    dbContextTransaction.Rollback();
                                    data.Message = "FAILED INSERT";
                                    data.Status = "ERROR";
                                }
                            }
                            else
                            {
                                data.Message = "NO FOUND  USER PROFILE";
                                data.Status = "ERROR";
                            }
                        }
                        catch (Exception ex)
                        {
                            //hacemos rollback si hay excepción
                            dbContextTransaction.Rollback();
                            data.Message = "FAILED INSERT";
                            data.Status = "ERROR";
                        }
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para add un nuevo usuario
        [HttpPost]
        [Route("AddUsuario")]
        public IHttpActionResult AddUsuario(Usuario usuario)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    entities.Usuarios.Add(usuario);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("validarTag")]
        public IHttpActionResult validarTag()
        {
            try
            {
                Models.GLOBAL_BMW1 entities = new Models.GLOBAL_BMW1();
                List<GetUsersTag> ListUsuariosTags = entities.GetUsersTags.ToList();
                if (ListUsuariosTags.Count == 0)
                {
                    ListUsuariosTags.Add(new GetUsersTag());
                    return Json(ListUsuariosTags);
                }
                else
                {
                    return Json(ListUsuariosTags);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("updateStatusUusarios")]
        public IHttpActionResult updateStatusUusarios(Usuario usuario)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    //int Ids = Int32.Parse(Id);
                    Usuario updateStatusUsers = (from c in entities.Usuarios where c.ID == usuario.ID select c).FirstOrDefault();
                    updateStatusUsers.Status = usuario.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para add un nuevo usuario
        [HttpPost]
        [Route("UpdateUsuario")]
        public IHttpActionResult UpdateUsuario(Usuario usuario)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                   
                   

                    Usuario updatedusuario = (from c in entities.Usuarios where c.ID == usuario.ID select c).FirstOrDefault();

                    updatedusuario.Nombre = usuario.Nombre;
                    updatedusuario.LastName = usuario.LastName;
                    updatedusuario.SecondLastName = usuario.SecondLastName;
                    updatedusuario.UserName = usuario.UserName;
                    updatedusuario.Password = usuario.Password;
                    updatedusuario.IdRol = usuario.IdRol;
                    updatedusuario.IdIdioma = usuario.IdIdioma;
                    updatedusuario.Telefono = usuario.Telefono;
                    updatedusuario.Correo = usuario.Correo;
                    updatedusuario.Status = usuario.Status;
                    updatedusuario.Compania = usuario.Compania;
                    updatedusuario.Departamento = usuario.Departamento;
                    updatedusuario.Manager = usuario.Manager;
                    updatedusuario.Tag1 = usuario.Tag1;
                    updatedusuario.Tag2 = usuario.Tag2;
                    updatedusuario.IDPerfilUsuario = usuario.IDPerfilUsuario;
                    updatedusuario.IDArea = usuario.IDArea;
                    updatedusuario.TimeOut = usuario.TimeOut;
                    updatedusuario.Token = usuario.Token;

                    
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("AddLogger")]
        public IHttpActionResult AddLogger(Logger logger)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    entities.Loggers.Add(logger);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("getAllLogger")]
        public IHttpActionResult getAllLogger(Logger logger)
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<GetAllLogger> ListLogger = entities.GetAllLoggers.ToList();
                if (ListLogger.Count > 0)
                {
                    return Json(ListLogger);
                }
                else
                {
                    ListLogger.Add(new GetAllLogger());
                    return Json(ListLogger);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("AutocompleteDepartamento")]
        public IHttpActionResult AutocompleteDepartamento(string itemB)
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Departamento> ListItems;
                List<autocompletes> ListDepartamentos = new List<autocompletes>();
                if (itemB != null && itemB != "")
                {
                    ListItems = entities.Departamentos.Where(h => h.DepartamentName.Contains(itemB) && h.Status == "true").ToList();
                    if (ListItems.Count > 0)
                    {
                        foreach (var item in ListItems)
                        {
                            ListDepartamentos.Add(new autocompletes() { ID = item.ID, item = item.DepartamentName });
                        }
                        return Json(ListDepartamentos);
                    }
                    else
                    {
                        ListDepartamentos.Add(new autocompletes());
                        return Json(ListDepartamentos);
                    }
                }
                else
                {
                    return Json(new autocompletes());
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }

        [HttpGet]
        [Route("AutocompleteCompany")]
        public IHttpActionResult AutocompleteCompany(string itemB)
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Compania> ListItems;
                List<autocompletes> ListCompanys = new List<autocompletes>();
                if (itemB != null && itemB != "")
                {
                    ListItems = entities.Companias.Where(h => h.CompaniaName.Contains(itemB) && h.Status == "true").ToList();
                    if (ListItems.Count > 0)
                    {
                        foreach (var item in ListItems)
                        {
                            ListCompanys.Add(new autocompletes() { ID = item.ID, item = item.CompaniaName });
                        }
                        return Json(ListCompanys);
                    }
                    else
                    {
                        ListCompanys.Add(new autocompletes());
                        return Json(ListCompanys);
                    }
                }
                else
                {
                    return Json(new autocompletes());
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("getAllPerfilesUsuarios")]
        public IHttpActionResult getAllPerfilesUsuarios()
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<PerfilesUsuario> ListItems = entities.PerfilesUsuarios.Where(s => s.Status == "true").ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new PerfilesUsuario());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("getAllPerfiles")]
        public IHttpActionResult getAllPerfiles(string All)
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Perfil> lstPerfiles = new List<Perfil>();
                List<PerfilesUsuario> ListPerfilesUsuarios;
                if (All=="1")
                {
                    ListPerfilesUsuarios = entities.PerfilesUsuarios.Where(s => s.Status == "true" || s.Status == "false").OrderByDescending(s => s.ID).ToList();
                }
                else
                {
                   ListPerfilesUsuarios = entities.PerfilesUsuarios.Where(s => s.Status == "true").OrderByDescending(s => s.ID).ToList();
                }
               
                foreach (var item in ListPerfilesUsuarios)
                {
                    Perfil Perfil1 = new Perfil();
                    Perfil1.ID = item.ID;
                    Perfil1.PerfilName = item.PerfilName;
                    Perfil1.Status = item.Status;

                    Perfil1.ListPerfil_Modulos = entities.GetAllPerfil_Modulos.Where(s => s.IDPerfil == item.ID).ToList();
                    Perfil1.ListPerfil_Sistemas = entities.GetAllPerfil_Sistemas.Where(s => s.IDPerfil == item.ID).ToList();
                    Perfil1.ListPerfil_Edificios = entities.GetAllPerfil_Edificios.Where(s => s.IDPerfil == item.ID).ToList();
                    lstPerfiles.Add(Perfil1);
                }
                return Json(lstPerfiles);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("getPerfilesBy")]
        public IHttpActionResult getPerfilesBy(int id)
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Perfil> lstPerfiles = new List<Perfil>();
                List<PerfilesUsuario> ListPerfilesUsuarios = entities.PerfilesUsuarios.Where(s => s.Status == "true" && s.ID == id).OrderByDescending(s => s.ID).ToList();
                foreach (var item in ListPerfilesUsuarios)
                {
                    Perfil Perfil1 = new Perfil();
                    Perfil1.ID = item.ID;
                    Perfil1.PerfilName = item.PerfilName;
                    Perfil1.ListPerfil_Modulos = entities.GetAllPerfil_Modulos.Where(s => s.IDPerfil == item.ID).ToList();
                    Perfil1.ListPerfil_Sistemas = entities.GetAllPerfil_Sistemas.Where(s => s.IDPerfil == item.ID).ToList();
                    Perfil1.ListPerfil_Edificios = entities.GetAllPerfil_Edificios.Where(s => s.IDPerfil == item.ID).ToList();
                    lstPerfiles.Add(Perfil1);
                }
                return Json(lstPerfiles);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //ws de Gerardoooooooooooooooo para systems
        [HttpGet]
        [Route("getAllBuilding")]
        public IHttpActionResult getAllBuilding()
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Edificio> ListItems = entities.Edificios.Where(s => s.Status == "true").ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Edificio());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("getAllSystems")]
        public IHttpActionResult getAllSystems(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<viewSystem> ListItems;
                if (All == "1")
                {
                    ListItems = entities.viewSystems.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.viewSystems.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new viewSystem());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de un sistema deoendiendo de un id
        [HttpPost]
        [Route("getSystem")]
        public IHttpActionResult getSystem(Sistema sistema)
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<viewSystem> ListItems = entities.viewSystems.Where(s => s.ID == sistema.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new viewSystem());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save system
        [HttpPost]
        [Route("AddSystem")]
        public IHttpActionResult AddSystem(Sistema sistema)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Sistema> ListItems;
                ListItems = entities.Sistemas.Where(h => h.SistemaName == sistema.SistemaName && h.IdBuilding == sistema.IdBuilding).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Sistemas.Add(sistema);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateSystem")]
        public IHttpActionResult UpdateSystem(Sistema _sistema)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Sistema> ListItems;
                ListItems = entities.Sistemas.Where(h => h.SistemaName == _sistema.SistemaName && h.IdBuilding == _sistema.IdBuilding).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Sistema updateSys = (from c in entities.Sistemas where c.ID == _sistema.ID select c).FirstOrDefault();
                    updateSys.IdBuilding = _sistema.IdBuilding;
                    updateSys.SistemaName = _sistema.SistemaName;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar system
        [HttpPost]
        [Route("DeleteSystem")]
        public IHttpActionResult DeleteSystem(Sistema sistema)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Sistema updateSys = (from c in entities.Sistemas where c.ID == sistema.ID select c).FirstOrDefault();
                    updateSys.Status = sistema.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //ws de Gerardoooooooooooooooo para MODULESSSSSSSSSSSSSSSSSSSSSSSSSSS
        [HttpGet]
        [Route("getAllModules")]
        public IHttpActionResult getAllModules(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Modulo> ListItems;
                if (All == "1")
                {
                    ListItems = entities.Modulos.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.Modulos.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Modulo());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de un sistema deoendiendo de un id
        [HttpPost]
        [Route("getModule")]
        public IHttpActionResult getModule(Modulo module)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Modulo> ListItems = entities.Modulos.Where(s => s.ID == module.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Modulo());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save system
        [HttpPost]
        [Route("AddModule")]
        public IHttpActionResult AddModule(Modulo module)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Modulo> ListItems;
                ListItems = entities.Modulos.Where(h => h.ModuloName == module.ModuloName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Modulos.Add(module);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateModule")]
        public IHttpActionResult UpdateModule(Modulo module)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Modulo> ListItems;
                ListItems = entities.Modulos.Where(h => h.ModuloName == module.ModuloName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Modulo updateModule = (from c in entities.Modulos where c.ID == module.ID select c).FirstOrDefault();
                    updateModule.ModuloName = module.ModuloName;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar system
        [HttpPost]
        [Route("DeleteModule")]
        public IHttpActionResult DeleteModule(Modulo module)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Modulo updateModule = (from c in entities.Modulos where c.ID == module.ID select c).FirstOrDefault();
                    updateModule.Status = module.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //web service para edificios edificios edificios edificios 
        [HttpGet]
        [Route("getAllEdificios")]
        public IHttpActionResult getAllEdificios(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Edificio> ListItems;
                if (All == "1")
                {
                    ListItems = entities.Edificios.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.Edificios.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Edificio());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de un edificio dependiendo de un id
        [HttpPost]
        [Route("getEdificios")]
        public IHttpActionResult getEdificios(Edificio Edificios)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Edificio> ListItems = entities.Edificios.Where(s => s.ID == Edificios.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Edificio());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save system
        [HttpPost]
        [Route("AddEdificios")]
        public IHttpActionResult AddEdificios(Edificio Edificios)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Edificio> ListItems;
                ListItems = entities.Edificios.Where(h => h.EdificioName == Edificios.EdificioName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Edificios.Add(Edificios);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateEdificios")]
        public IHttpActionResult UpdateEdificios(Edificio Edificios)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Edificio> ListItems;
                ListItems = entities.Edificios.Where(h => h.EdificioName == Edificios.EdificioName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Edificio updateEdificio = (from c in entities.Edificios where c.ID == Edificios.ID select c).FirstOrDefault();
                    updateEdificio.EdificioName = Edificios.EdificioName;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar system
        [HttpPost]
        [Route("DeleteEdificios")]
        public IHttpActionResult DeleteEdificios(Edificio Edificios)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Edificio updateEdificio = (from c in entities.Edificios where c.ID == Edificios.ID select c).FirstOrDefault();
                    updateEdificio.Status = Edificios.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //web service para compania compania compania compania
        [HttpGet]
        [Route("getAllCompanias")]
        public IHttpActionResult getAllCompanias(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Compania> ListItems;
                if (All == "1")
                {
                    ListItems = entities.Companias.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.Companias.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Compania());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de un sistema deoendiendo de un id
        [HttpPost]
        [Route("getCompanias")]
        public IHttpActionResult getCompanias(Compania companias)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Compania> ListItems = entities.Companias.Where(s => s.ID == companias.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Compania());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save companias
        [HttpPost]
        [Route("AddCompanias")]
        public IHttpActionResult AddCompanias(Compania companias)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Compania> ListItems;
                ListItems = entities.Companias.Where(h => h.CompaniaName == companias.CompaniaName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Companias.Add(companias);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateCompanias")]
        public IHttpActionResult UpdateCompanias(Compania companias)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Compania> ListItems;
                ListItems = entities.Companias.Where(h => h.CompaniaName == companias.CompaniaName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Compania updateCompania = (from c in entities.Companias where c.ID == companias.ID select c).FirstOrDefault();
                    updateCompania.CompaniaName = companias.CompaniaName;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar system
        [HttpPost]
        [Route("DeleteCompanias")]
        public IHttpActionResult DeleteCompanias(Compania companias)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Compania updateCompania = (from c in entities.Companias where c.ID == companias.ID select c).FirstOrDefault();
                    updateCompania.Status = companias.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //web service para Area area area area area area area
        [HttpGet]
        [Route("getAllAreas")]
        public IHttpActionResult getAllAreas(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Area> ListItems;
                if (All == "1")
                {
                    ListItems = entities.Areas.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.Areas.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Area());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de una Area deoendiendo de un id
        [HttpPost]
        [Route("getAreas")]
        
        public IHttpActionResult getAreas(Area areas)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Area> ListItems = entities.Areas.Where(s => s.ID == areas.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Area());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save areas
        [HttpPost]
        [Route("AddArea")]
        public IHttpActionResult AddArea(Area areas)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Area> ListItems;
                ListItems = entities.Areas.Where(h => h.AreaName == areas.AreaName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Areas.Add(areas);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateAreas")]
        public IHttpActionResult UpdateAreas(Area areas)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Area> ListItems;
                ListItems = entities.Areas.Where(h => h.AreaName == areas.AreaName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Area updateArea = (from c in entities.Areas where c.ID == areas.ID select c).FirstOrDefault();
                    updateArea.AreaName = areas.AreaName;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar areas
        [HttpPost]
        [Route("DeleteAreas")]
        public IHttpActionResult DeleteAreas(Area areas)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Area updateArea = (from c in entities.Areas where c.ID == areas.ID select c).FirstOrDefault();
                    updateArea.Status = areas.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //web service department department department department 
        [HttpGet]
        [Route("getAllDepartments")]
        public IHttpActionResult getAllDepartments(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Departamento> ListItems;
                if (All == "1")
                {
                    ListItems = entities.Departamentos.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.Departamentos.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Departamento());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de una department deoendiendo de un id
        [HttpPost]
        [Route("getDepartment")]
        public IHttpActionResult getDepartment(Departamento departments)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Departamento> ListItems = entities.Departamentos.Where(s => s.ID == departments.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new Departamento());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save areas
        [HttpPost]
        [Route("AddDepartments")]
        public IHttpActionResult AddDepartments(Departamento departments)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Departamento> ListItems;
                ListItems = entities.Departamentos.Where(h => h.DepartamentName == departments.DepartamentName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Departamentos.Add(departments);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateDepartments")]
        public IHttpActionResult UpdateDepartments(Departamento departments)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Departamento> ListItems;
                ListItems = entities.Departamentos.Where(h => h.DepartamentName == departments.DepartamentName).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Departamento updateDepartamento = (from c in entities.Departamentos where c.ID == departments.ID select c).FirstOrDefault();
                    updateDepartamento.DepartamentName = departments.DepartamentName;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar areas
        [HttpPost]
        [Route("DeleteDepartments")]
        public IHttpActionResult DeleteDepartments(Departamento departments)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Departamento updateDepartamneto = (from c in entities.Departamentos where c.ID == departments.ID select c).FirstOrDefault();
                    updateDepartamneto.Status = departments.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //Vista   para los logers
        [HttpGet]
        [Route("getAllViewLogger")]
        public IHttpActionResult getAllViewLogger()
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<viewLogger> ListItems = entities.viewLoggers.ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new viewLogger());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Vista   para los loger de sesion
        [HttpGet]
        [Route("getAllViewSessionLog")]
        public IHttpActionResult getAllViewSessionLog()
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<viewSesionLog> ListItems = entities.viewSesionLogs.ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new viewSesionLog());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        [HttpGet]
        [Route("getAllRoles")]
        public IHttpActionResult getAllRoles(string All)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<viewRole> ListItems;
                if (All == "1")
                {
                    ListItems = entities.viewRoles.Where(s => s.Status == "true" || s.Status == "false").ToList();
                }
                else
                {
                    ListItems = entities.viewRoles.Where(s => s.Status == "true").ToList();
                }
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new viewRole());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //WS para obtener los datos de una department deoendiendo de un id
        [HttpPost]
        [Route("getRoles")]
        public IHttpActionResult getRoles(Role roles)
        {
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<viewRole> ListItems = entities.viewRoles.Where(s => s.ID == roles.ID).ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new viewRole());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }
        //Web service  save areas
        [HttpPost]
        [Route("AddRoles")]
        public IHttpActionResult AddRoles(Role roles)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Role> ListItems;
                ListItems = entities.Roles.Where(h => h.RolName == roles.RolName && h.IDPerfilUsuario == roles.IDPerfilUsuario).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    entities.Roles.Add(roles);
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL ADD";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED INSERT";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        [HttpPost]
        [Route("UpdateRoles")]
        public IHttpActionResult UpdateRoles(Role roles)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<Role> ListItems;
                ListItems = entities.Roles.Where(h => h.RolName == roles.RolName && h.IDPerfilUsuario == roles.IDPerfilUsuario).ToList();
                if (ListItems.Count > 0)
                {
                    data.Message = "ALREADY EXISTS";
                    data.Status = "DUPLICATE";
                }
                else
                {
                    Role updateRole = (from c in entities.Roles where c.ID == roles.ID select c).FirstOrDefault();
                    updateRole.RolName = roles.RolName;
                    updateRole.IDPerfilUsuario = roles.IDPerfilUsuario;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para eliminar areas
        [HttpPost]
        [Route("DeleteRoles")]
        public IHttpActionResult DeleteRoles(Role roles)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    Role updateRole = (from c in entities.Roles where c.ID == roles.ID select c).FirstOrDefault();
                    updateRole.Status = roles.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }
        //WS para devolver todos los perfiles de usuario
        [HttpGet]
        [Route("getAllPerfilesUsuario")]
        public IHttpActionResult getAllPerfilesUsuario()
        {
            try
            {
                //optiene todos los logs de operacion de una vista
                GLOBAL_BMW1 entities = new GLOBAL_BMW1();
                List<PerfilesUsuario> ListItems = entities.PerfilesUsuarios.Where(s => s.Status == "true").ToList();
                if (ListItems.Count > 0)
                {
                    return Json(ListItems);
                }
                else
                {
                    ListItems.Add(new PerfilesUsuario());
                    return Json(ListItems);
                }
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return Json(mensaje);
            }
        }


        //WS para eliminar areas
        [HttpPost]
        [Route("DeletePerfil")]
        public IHttpActionResult DeletePerfil(PerfilesUsuario perfil)
        {
            int DataAfected = 0;
            DataResponse data = new DataResponse();
            try
            {
                using (GLOBAL_BMW1 entities = new GLOBAL_BMW1())
                {
                    PerfilesUsuario updateperfil = (from c in entities.PerfilesUsuarios where c.ID == perfil.ID select c).FirstOrDefault();
                    updateperfil.Status = perfil.Status;
                    DataAfected = entities.SaveChanges();
                    if (DataAfected > 0)
                    {
                        data.Message = "SUCCESSFUL UPDATED";
                        data.Status = "OK";
                    }
                    else
                    {
                        data.Message = "FAILED UPDATED";
                        data.Status = "ERROR";
                    }
                }
                return Json(data);
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                //AgregaLog("Ha ocurrido un error en la consulta al insertar un modelo de vehiculo en la bd local");
                return Json(mensaje);
            }
        }


    }
    public class Perfil
    {
        public int ID { get; set; }
        public string PerfilName { get; set; }
        public string Status { get; set; }
        public List<GetAllPerfil_Modulos> ListPerfil_Modulos { get; set; }
        public List<GetAllPerfil_Sistemas> ListPerfil_Sistemas { get; set; }
        public List<GetAllPerfil_Edificios> ListPerfil_Edificios { get; set; }
    }
    public class DatosLoginTag
    {
        public List<GetUserDataTag> ListGetUserData_Tag { get; set; }
        public int ID { get; set; }
        public string PerfilName { get; set; }
        public List<GetAllPerfil_Modulos> ListPerfil_Modulos { get; set; }
        public List<GetAllPerfil_Sistemas> ListPerfil_Sistemas { get; set; }
        public List<GetAllPerfil_Edificios> ListPerfil_Edificios { get; set; }
    }
    public class DatosLoginTag_Perfiles
    {
        public List<GetUserDataTag> ListGetUserData_Tag { get; set; }
        public int ID { get; set; }
        public string PerfilName { get; set; }
        public List<GetAllPerfil_Modulos> ListPerfil_Modulos { get; set; }
    }
    public class DataHMI
    {
        public string Tag { get; set; }
    }
    public class autocompletes
    {
        public int ID { get; set; }
        public string item { get; set; }
    }
    public class Mensajes
    {
        public string Status { get; set; }
        public string Mensaje { get; set; }
    }

    public class Areamia
    {
        public int ID { get; set; }
        public string AreaName { get; set; } = "";
        public string Status { get; set; } = "";

    }
}