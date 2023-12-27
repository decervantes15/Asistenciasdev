using Asistencias.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
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

        [HttpPost]
        [Route("api/v1/admin/recursos")]
        public RespuestaJson ObtenerRecursos()
        {
            string ip = HttpContext.Current.Request.UserHostAddress.ToString();

            ConexionBD conexion = new ConexionBD();
            RespuestaBD respuesta = conexion.Consultar("EXEC [dbo].[ListarRecursos]", null);
            RespuestaJson respuestaJson = new RespuestaJson();

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                List<Recursos> recursos = new List<Recursos>();
                foreach (DataRow dataRow in respuesta.Data.Rows)
                {
                    recursos.Add(new Recursos
                    {
                        no_empleado = dataRow["no_empleado"].ToString(),
                        email = dataRow["email"].ToString(),
                        area = dataRow["area"].ToString(),
                        direccion = dataRow["direccion"].ToString(),
                        equipo = dataRow["equipo"].ToString(),
                        ip = dataRow["ip"].ToString(),
                        puesto = dataRow["puesto"].ToString(),
                        recurso = dataRow["recurso"].ToString(),
                        rol = dataRow["rol"].ToString(),
                    });
                }
                respuestaJson.Estatus = EstatusRespuesta.Ok;
                respuestaJson.Data = recursos;
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return respuestaJson;
        }

        [HttpPost]
        [Route("api/v1/admin/areas")]
        public RespuestaJson ObtenerAreas()
        {
            string ip = HttpContext.Current.Request.UserHostAddress.ToString();

            ConexionBD conexion = new ConexionBD();
            RespuestaBD respuesta = conexion.Consultar("EXEC [dbo].[ListarAreas]", null);
            RespuestaJson respuestaJson = new RespuestaJson();

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                List<Area> areas = new List<Area>();
                foreach (DataRow dataRow in respuesta.Data.Rows)
                {
                    areas.Add(new Area
                    {
                        id = dataRow["id"].ToString(),
                        area = dataRow["area"].ToString(),
                    });
                }
                respuestaJson.Estatus = EstatusRespuesta.Ok;
                respuestaJson.Data = areas;
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return respuestaJson;
        }

        [HttpPost]
        [Route("api/v1/admin/info")]
        public async Task<RespuestaJson> ObtenerInfo()
        {
            string json = await Request.Content.ReadAsStringAsync();
            BuscarReporte reporte = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarReporte>(json);

            string ip = HttpContext.Current.Request.UserHostAddress.ToString();

            ConexionBD conexion = new ConexionBD();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@mes", Convert.ToInt32( reporte.mes)),
                new SqlParameter("@semana", Convert.ToInt32( reporte.semana)),
                new SqlParameter("@area", Convert.ToInt32( reporte.area)),
                new SqlParameter("@no_empleado", reporte.no_empleado),
            };
            RespuestaBD respuesta = conexion.Consultar("EXEC [dbo].[GetReporte] @mes, @semana, @area, @no_empleado", parameters);
            RespuestaJson respuestaJson = new RespuestaJson();

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                List<ReporteAsistencia> info = new List<ReporteAsistencia>();
                foreach (DataRow dataRow in respuesta.Data.Rows)
                {
                    info.Add(new ReporteAsistencia
                    {
                        comida_inicio = dataRow["comida_inicio"].ToString(),
                        comida_fin = dataRow["comida_fin"].ToString(),
                        email = dataRow["email"].ToString(),
                        entrada = dataRow["entrada"].ToString(),
                        laborado = dataRow["laborado"].ToString(),
                        mes = dataRow["mes"].ToString(),
                        salida = dataRow["salida"].ToString(),
                        semana = dataRow["semana"].ToString(),
                    }); ;
                }
                respuestaJson.Estatus = EstatusRespuesta.Ok;
                respuestaJson.Data = info;
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return respuestaJson;
        }

        [HttpPost]
        [Route("api/v1/admin/info_mensual")]
        public async Task<RespuestaJson> ObtenerInfoMensual()
        {
            string json = await Request.Content.ReadAsStringAsync();
            BuscarReporte reporte = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarReporte>(json);

            string ip = HttpContext.Current.Request.UserHostAddress.ToString();

            ConexionBD conexion = new ConexionBD();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@anio", DateTime.Now.Year),
                new SqlParameter("@mes", Convert.ToInt32( reporte.mes)),
                new SqlParameter("@semana", Convert.ToInt32( reporte.semana)),
                new SqlParameter("@no_empleado", reporte.no_empleado),
                new SqlParameter("@area", Convert.ToInt32( reporte.area)),
                new SqlParameter("@tipo_reporte", 1),
            };
            RespuestaBD respuesta = conexion.ConsultaMultiple("EXEC [dbo].[GetReporte] @anio, @mes, @semana, @no_empleado, @area, @tipo_reporte", parameters);
            RespuestaJson respuestaJson = new RespuestaJson();

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                List<ReporteAsistencia> info = new List<ReporteAsistencia>();
                if (respuesta.Datas != null)
                {
                    DataTable recursos = respuesta.Datas[0];
                    DataTable porcentajes = respuesta.Datas[1] ?? new DataTable();

                    foreach (DataRow dataRow in recursos.Rows)
                    {
                        info.Add(new ReporteAsistencia
                        {
                            email = dataRow["email"].ToString(),
                            nombre = dataRow["nombre"].ToString(),
                            no_empleado = dataRow["no_empleado"].ToString(),
                            area = dataRow["area"].ToString(),
                            puesto = dataRow["puesto"].ToString(),
                            activo = dataRow["activo"].ToString(),
                            porcentaje = "0"
                        });
                    }

                    foreach (DataRow dataRow in porcentajes.Rows)
                    {
                        string email = dataRow["email"].ToString();
                        ReporteAsistencia item = info.Where(x => x.email == email).FirstOrDefault();
                        if (item != null)
                        {
                            string porcentaje = dataRow["laborado"].ToString();
                            double porciento = Convert.ToDouble(porcentaje);
                            item.porcentaje = $"{porciento:0.00} %";
                            if (porciento >= 90)
                                item.semaforo = 'v';
                            else if (porciento < 90 && porciento >= 84)
                                item.semaforo = 'a';
                            else if (porciento < 84)
                                item.semaforo = 'r';
                            else
                                item.semaforo = 'n';
                        }
                    }
                }
                respuestaJson.Estatus = EstatusRespuesta.Ok;
                respuestaJson.Data = info;
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return respuestaJson;
        }

        [HttpPost]
        [Route("api/v1/admin/info_semanal")]
        public async Task<RespuestaJson> ObtenerInfoSemanal()
        {
            string json = await Request.Content.ReadAsStringAsync();
            BuscarReporte reporte = Newtonsoft.Json.JsonConvert.DeserializeObject<BuscarReporte>(json);

            string ip = HttpContext.Current.Request.UserHostAddress.ToString();

            ConexionBD conexion = new ConexionBD();
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@anio", DateTime.Now.Year),
                new SqlParameter("@mes", Convert.ToInt32( reporte.mes)),
                new SqlParameter("@semana", Convert.ToInt32( reporte.semana)),
                new SqlParameter("@area", Convert.ToInt32( reporte.area)),
                new SqlParameter("@no_empleado", reporte.no_empleado),
                new SqlParameter("@tipo_reporte", 2),
            };
            RespuestaBD respuesta = conexion.Consultar("EXEC [dbo].[GetReporte] @anio, @mes, @semana, @no_empleado, @area, @tipo_reporte", parameters);
            RespuestaJson respuestaJson = new RespuestaJson();

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                List<ReporteAsistencia> info = new List<ReporteAsistencia>();
                foreach (DataRow dataRow in respuesta.Data.Rows)
                {
                    info.Add(new ReporteAsistencia
                    {
                        comida_inicio = dataRow["comida_inicio"].ToString(),
                        comida_fin = dataRow["comida_fin"].ToString(),
                        email = dataRow["email"].ToString(),
                        entrada = dataRow["entrada"].ToString(),
                        laborado = dataRow["laborado"].ToString(),
                        mes = dataRow["mes"].ToString(),
                        salida = dataRow["salida"].ToString(),
                        semana = dataRow["semana"].ToString(),
                    }); ;
                }
                respuestaJson.Estatus = EstatusRespuesta.Ok;
                respuestaJson.Data = info;
            }
            else
            {
                respuestaJson.Mensaje = respuesta.Mensaje;
            }

            return respuestaJson;
        }
    }
}