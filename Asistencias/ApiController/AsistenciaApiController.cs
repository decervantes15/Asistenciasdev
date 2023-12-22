using Asistencias.App_Start;
using Asistencias.Models;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Asistencias.Controllers
{
    public class AsistenciaApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/v1/usuarios")]
        public async Task<IHttpActionResult> Usuarios()
        {
            string json = await Request.Content.ReadAsStringAsync();
            BuscarRegistrar registro = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarRegistrar>(json);
            RespuestaJson respuestaJson = new RespuestaJson();
            ConexionBD conexion = new ConexionBD();
            List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@username", Value = registro.username }
                };
            RespuestaBD respuesta = conexion.Consultar("EXEC [dbo].[GetRecurso] @username", parameters);

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                if (respuesta.Data.Rows.Count == 1)
                {
                    respuestaJson.Estatus = EstatusRespuesta.Ok;

                    var context = HttpContext.Current.Request.GetOwinContext();
                    var auth = context.Authentication;
                    var claims = new List<Claim>
                        {
                            new Claim("usuario", registro.username),
                            new Claim("token", Token.Generar())
                        };
                    var identy = new ClaimsIdentity(claims, ConfigurationManager.AppSettings["AppCookie"]);
                    auth.SignIn(identy);

                }
                else if (respuesta.Data.Rows.Count == 0)
                {
                    respuestaJson.Mensaje = "No se encontró el usuario";
                }
                else
                {
                    respuestaJson.Mensaje = "Se encontraron varios usuarios";
                }
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return Ok(respuestaJson);
        }

        [HttpPost]
        [Route("api/v1/login")]
        public async Task<IHttpActionResult> Login()
        {

            

            string json = await Request.Content.ReadAsStringAsync();



            BuscarRegistrar registro = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarRegistrar>(json);
            RespuestaJson respuestaJson = new RespuestaJson();
            BuscarActiveDirectory(registro, respuestaJson);
            registro.ip = HttpContext.Current.Request.UserHostAddress.ToString();
            string tipo_acceso = respuestaJson.Estatus == EstatusRespuesta.Ok ? "user" : "fail";
            respuestaJson.Estatus = EstatusRespuesta.Error;
            ConexionBD conexion = new ConexionBD();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@email", Value = registro.username },
                new SqlParameter { ParameterName = "@ip", Value = registro.ip },
                new SqlParameter { ParameterName = "@tipo_acceso", Value = tipo_acceso },
            };
            RespuestaBD respuesta = conexion.Consultar("EXEC [dbo].[GuardaAcceso] @email, @ip, @tipo_acceso", parameters);

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                respuestaJson.Estatus = EstatusRespuesta.Error;

                if (respuesta.Data.Rows.Count == 1)
                {
                    DataRow dataRow = respuesta.Data.Rows[0];
                    string error = dataRow["Error"].ToString();
                    respuestaJson.Mensaje = dataRow["Mensaje"].ToString();
                    if (error == "0")
                    {
                        respuestaJson.Estatus = EstatusRespuesta.Ok;

                        IOwinContext context = HttpContext.Current.Request.GetOwinContext();
                        IAuthenticationManager auth = context.Authentication;
                        List<Claim> claims = new List<Claim>
                            {
                                new Claim("usuario", registro.username)
                            };
                        ClaimsIdentity identy = new ClaimsIdentity(claims, ConfigurationManager.AppSettings["AppCookie"]);
                        auth.SignIn(identy);
                    }
                }
                else if (respuesta.Data.Rows.Count == 0)
                {
                    respuestaJson.Mensaje = "No se encontró el usuario";
                }
                else
                {
                    respuestaJson.Mensaje = "Se encontraron varios usuarios";
                }
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return Ok(respuestaJson);
        }

        [HttpPost]
        [Route("api/v1/registrar")]
        public async Task<IHttpActionResult> Registrar()
        {
            string json = await Request.Content.ReadAsStringAsync();
            BuscarRegistrar registro = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarRegistrar>(json);
            RespuestaJson respuestaJson = new RespuestaJson();
            BuscarActiveDirectory(registro, respuestaJson);

            if (respuestaJson.Estatus == EstatusRespuesta.Ok)
            {
                respuestaJson.Estatus = EstatusRespuesta.Error;
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter{ ParameterName = "@usuario", Value = registro.username },
                    new SqlParameter{ ParameterName = "@ip", Value = registro.ip },
                    new SqlParameter{ ParameterName = "@tipo_registro", Value = registro.tipo_registro },
                };
                ConexionBD conexionBD = new ConexionBD();
                RespuestaBD respuestaBD = conexionBD.Consultar("EXEC [dbo].[GuardaRegistro] @usuario, @ip, @tipo_registro", parameters);

                if (respuestaBD.Estatus == EstatusRespuesta.Ok)
                {
                    if (respuestaBD.Data != null)
                    {
                        DataRow row = respuestaBD.Data.Rows[0];
                        respuestaJson.Mensaje = row["Mensaje"].ToString();

                        string codigo_error = row["Error"].ToString();
                        string fecha = row["Fecha"].ToString();
                        if (codigo_error == "0")
                        {
                            respuestaJson.Estatus = EstatusRespuesta.Ok;
                            respuestaJson.Data = Convert.ToDateTime(fecha);
                        }
                    }
                }
            }

            return Ok(respuestaJson);
        }

        [HttpPost]
        [Route("api/v1/asistencias")]
        public async Task<RespuestaJson> Asistencias()
        {

            System.Net.Http.Headers.AuthenticationHeaderValue authorizationHeader = Request.Headers.Authorization;

            Console.WriteLine("");

            string json = await Request.Content.ReadAsStringAsync();

            BuscarRegistrar buscar = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarRegistrar>(json);
            RespuestaJson respuestaJson = new RespuestaJson();

            ConexionBD conexionBD = new ConexionBD();
            string ip = HttpContext.Current.Request.UserHostAddress.ToString();

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@username", Value = buscar.username },
                new SqlParameter{ ParameterName = "@inicio", Value = Convert.ToDateTime( buscar.fecha_inicio)},
                new SqlParameter{ ParameterName = "@fin", Value = Convert.ToDateTime( buscar.fecha_fin) },
            };
            RespuestaBD respuestaBD = conexionBD.Consultar($"EXEC [dbo].[GetAsistencias] @username, @inicio, @fin", parameters);

            if (respuestaBD.Estatus == EstatusRespuesta.Ok)
            {
                List<AsistenciasModel> info = new List<AsistenciasModel>();
                if (respuestaBD.Data != null)
                {
                    if (respuestaBD.Data.Rows.Count > 0)
                    {
                        foreach (DataRow row in respuestaBD.Data.Rows)
                        {
                            info.Add(new AsistenciasModel
                            {
                                area = row["area"].ToString(),
                                comentarios = row["comentarios"].ToString(),
                                comida_fin = row["comida_fin"].ToString(),
                                comida_inicio = row["comida_inicio"].ToString(),
                                email = row["email"].ToString(),
                                entrada = row["entrada"].ToString(),
                                equipo = row["equipo"].ToString(),
                                ip = row["ip"].ToString(),
                                no_empleado = row["no_empleado"].ToString(),
                                puesto = row["puesto"].ToString(),
                                recurso = row["recurso"].ToString(),
                                rol = row["rol"].ToString(),
                                salida = row["salida"].ToString(),
                            });
                        }
                        respuestaJson.Estatus = EstatusRespuesta.Ok;
                        respuestaJson.Data = info;
                    }
                    else
                    {
                        respuestaJson.Mensaje = "Sin información";
                    }
                }
            }
            return respuestaJson;
        }

        public static void BuscarActiveDirectory(BuscarRegistrar registro, RespuestaJson respuestaJson)
        {
            try
            {
                string activeDirectory = ConfigurationManager.AppSettings["dominio"];
                DirectoryEntry dirEntry = new DirectoryEntry($"LDAP://{activeDirectory}", registro.username, registro.password);
                DirectorySearcher searcher = new DirectorySearcher(dirEntry, $"sAMAccountName={registro.username}");
                SearchResult encontrado = searcher.FindOne();
                if (encontrado != null)
                {
                    registro.ip = HttpContext.Current.Request.UserHostAddress.ToString();
                    respuestaJson.Estatus = EstatusRespuesta.Ok;
                }
                else
                {
                    respuestaJson.Mensaje = "Usuario y/o contraseña son inválidos";
                }
            }
            catch (Exception ex)
            {
                respuestaJson.Mensaje = ex.Message;
            }
        }
    }
}