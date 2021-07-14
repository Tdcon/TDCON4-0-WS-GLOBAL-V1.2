using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WSGlobal.Models;
using WSGlobal.Security;
namespace WSGlobal.Controllers
{

    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        BDD BD = new BDD();



        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            if (identity.IsAuthenticated)
            {
                return Ok(new DataResult { Status = "false", Mensaje = "Usuario Autentificado" });
            }
            else
            {
                return Ok(new DataResult { Status = "false", Mensaje = "Usuario No Autentificado" });
            }
        }
        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {

            
            Usuario usuario = new Usuario();
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);


            //TODO: This code is only for demo - extract method in new class & validate correctly in your application !!
            //usuario.UserName = login.Username;
            //usuario.Password = login.Password;
            var passs = login.Password = DecryptStringAES(login.Password);
            var user = login.Username = DecryptStringAES(login.Username);



            usuario.UserName = user;
            usuario.Password = passs;
           
            var respuestaValida = ValidarLogin(usuario);

            if (respuestaValida != null)
            {
                //var isAdminValid = (login.Username == "admin" && login.Password == "123456");
                if (respuestaValida.lstUsuarios[0].UserName != null)
                {
                    var rolename = "Administrator";
                    respuestaValida.lstUsuarios[0].Token = TokenGenerator.GenerateTokenJwt(login.Username, rolename);
                    respuestaValida.lstUsuarios[0].TimeOut = DateTime.Now.AddMinutes(15).ToString("MM/dd/yyyy HH:mm:ss");

                    return Ok(respuestaValida);
                }
                else
                {
                    return Ok(respuestaValida);
                }
            }
            else
            {
                return Ok(new DataResult { Status = "ERROR", Mensaje = "Error: res null" });
            }

            

            // Unauthorized access 
            //return Unauthorized();
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

        [HttpGet]
        public DataResponse ValidarLogin(Usuario usuario)
        {
            var respuesta = false;
            DataResponse data = new DataResponse();
            Usuario users = new Usuario();
           
            try
            {
               
                if (usuario.UserName != null && usuario.UserName != "")
                {
                    GLOBAL_BMW1 entities = new GLOBAL_BMW1();

                    var passs = DecryptStringAES(usuario.Password);
                    var user = DecryptStringAES(usuario.UserName);

                    List<Usuario> ListUsuarios = entities.Usuarios.Where(s => s.UserName == user && s.Password == passs).OrderByDescending(s => s.ID).ToList();
                    if (ListUsuarios.Count > 0)
                    {
                        respuesta = true;
                        ListUsuarios[0].Password = "";
                        data.lstUsuarios = ListUsuarios;
                        data.Message = "LOGIN OK";
                        data.Status = "OK";
                        //users.Password = ListUsuarios[0].Password;
                        //users.UserName = ListUsuarios[0].UserName;



                    }
                    else
                    {
                        respuesta = false;
                        data.Message = "USER NO FOUND";
                        data.Status = "ERROR";
                        //users.Password = null;
                        //users.UserName = null;
                    }
                }
                else
                {
                    respuesta = false;
                    data.Message = "USER NO FOUND";
                    data.Status = "ERROR";
                    
                }
                return data;
            }
            catch (Exception e)
            {
                var mensaje = new Mensajes();
                mensaje.Mensaje = e.Message.ToString();
                mensaje.Status = "ERROR";
                return null;

            }
        }





    }
    public class DataResult
    {
        public string Status { get; set; }
        public string Mensaje { get; set; } = "";
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";

    }
}