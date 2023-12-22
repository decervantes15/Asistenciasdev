using Asistencias.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Asistencias.Controllers
{
    public class AdminApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/v1/admin/login")]
        public async Task<RespuestaJson> Login()
        {
            string json = await Request.Content.ReadAsStringAsync();
            BuscarRegistrar registro = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarRegistrar>(json);
            RespuestaJson respuestaJson = new RespuestaJson();
            AsistenciaApiController.BuscarActiveDirectory(registro, respuestaJson);
            registro.ip = HttpContext.Current.Request.UserHostAddress.ToString();

            string tipo_acceso = respuestaJson.Estatus == EstatusRespuesta.Ok ? "admin" : "fail";
            
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
                if (respuesta.Data.Rows.Count == 1)
                {
                    DataRow dataRow = respuesta.Data.Rows[0];
                    string error = dataRow["Error"].ToString();
                    respuestaJson.Mensaje = dataRow["Mensaje"].ToString();
                    if (error == "0")
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

            return respuestaJson;
        }
    }
}