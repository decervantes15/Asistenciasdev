using Asistencias.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Asistencias.Controllers
{
    [Authorize]
    public class AdministracionController :BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Asistencias()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ExportarConsulta(string username, string inicio, string fin)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var data = new
                {
                    username,
                    fecha_inicio = inicio,
                    fecha_fin = fin
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    string url = $"{Request.Url.Scheme}://{Request.Url.Authority}";
                    using (HttpResponseMessage response = httpClient.PostAsync(url + "/api/v1/asistencias", content).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string responseJson = response.Content.ReadAsStringAsync().Result;
                            RespuestaJson respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaJson>(responseJson);

                            if (respuesta.Estatus == EstatusRespuesta.Ok)
                            {
                                List<AsistenciasModel> asistencias = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AsistenciasModel>>(Newtonsoft.Json.JsonConvert.SerializeObject(respuesta.Data));
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
                            }
                            else
                            {
                                return Content("<script>function cerrar(){ alert('" + respuesta.Mensaje + "'); window.close(); } window.onload = cerrar();</script>");
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }

            return Content("<script>function cerrar(){ alert('Ocurrió un error al generar el reporte'); window.close(); } window.onload = cerrar();</script>");
        }
    }
}