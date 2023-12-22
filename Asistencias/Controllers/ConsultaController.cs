using Asistencias.App_Start;
using Asistencias.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Asistencias.Controllers
{
    [Authorize]
    public class ConsultaController : BaseController
    {
        [HttpGet]
        public ActionResult MisAsistencias()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Consulta(string inicio, string fin)
        {
            AppUsuario usuario = new AppUsuario(User.Identity);

            var data = new
            {
                username = usuario.usuario,
                fecha_inicio = inicio,
                fecha_fin = fin
            };

            RespuestaJson respuesta = await ConexionApi.Post($"{HttpContext.Request.Url.Scheme}://{Request.Url.Authority}" + "/api/v1/asistencias", data);
            respuesta.Data = respuesta.GetObject<List<AsistenciasModel>>();

            return Json(respuesta);
        }

        [HttpGet]
        public async Task<ActionResult> ExportarConsulta(string inicio, string fin)
        {
            AppUsuario usuario = new AppUsuario(User.Identity);
            var data = new
            {
                username = usuario.usuario,
                fecha_inicio = inicio,
                fecha_fin = fin
            };

            RespuestaJson respuesta = await ConexionApi.Post($"{HttpContext.Request.Url.Scheme}://{Request.Url.Authority}" + "/api/v1/asistencias", data, usuario.token);

            if (respuesta.Estatus == EstatusRespuesta.Ok)
            {
                List<AsistenciasModel> asistencias = respuesta.GetObject<List<AsistenciasModel>>();
                if (asistencias != null)
                {
                    if (asistencias.Count > 0)
                    {
                        int row = 1;
                        ExcelPackage package = new ExcelPackage();
                        ExcelWorksheet ws = package.Workbook.Worksheets.Add("Reporte");
                        ws.Cells[row, 1].Value = "No.Empleado";
                        ws.Cells[row, 2].Value = "Email";
                        ws.Cells[row, 3].Value = "Recurso";
                        ws.Cells[row, 4].Value = "IP";
                        ws.Cells[row, 5].Value = "Area";
                        ws.Cells[row, 6].Value = "Equipo";
                        ws.Cells[row, 7].Value = "Rol";
                        ws.Cells[row, 8].Value = "Puesto";
                        ws.Cells[row, 9].Value = "Comentarios";
                        ws.Cells[row, 10].Value = "Entrada";
                        ws.Cells[row, 11].Value = "Inicio comida";
                        ws.Cells[row, 12].Value = "Fin comida";
                        ws.Cells[row, 13].Value = "Salida";

                        ws.Cells[row, 1, row, 13].Style.Font.Bold = true;

                        row++;

                        foreach (AsistenciasModel asistencia in asistencias)
                        {
                            ws.Cells[row, 1].Value = asistencia.no_empleado;
                            ws.Cells[row, 2].Value = asistencia.email;
                            ws.Cells[row, 3].Value = asistencia.recurso;
                            ws.Cells[row, 4].Value = asistencia.ip;
                            ws.Cells[row, 5].Value = asistencia.area;
                            ws.Cells[row, 6].Value = asistencia.equipo;
                            ws.Cells[row, 7].Value = asistencia.rol;
                            ws.Cells[row, 8].Value = asistencia.puesto;
                            ws.Cells[row, 9].Value = asistencia.comentarios;
                            ws.Cells[row, 10].Value = asistencia.entrada;
                            ws.Cells[row, 11].Value = asistencia.comida_inicio;
                            ws.Cells[row, 12].Value = asistencia.comida_fin;
                            ws.Cells[row, 13].Value = asistencia.salida;
                            row++;
                        }

                        ws.Cells.AutoFitColumns();

                        return base.File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Consulta {DateTime.Now:yyyyMMdd HHmmss}.xlsx");
                    }
                    else
                    {
                        return Content("<script>function cerrar(){ alert('No cuenta con información'); window.close(); } window.onload = cerrar();</script>");
                    }
                }
                else
                {
                    return Content("<script>function cerrar(){ alert('" + respuesta.Mensaje + "'); window.close(); } window.onload = cerrar();</script>");
                }
            }

            return Content("<script>function cerrar(){ alert('Ocurrió un error al generar el reporte'); window.close(); } window.onload = cerrar();</script>");
        }
    }
}