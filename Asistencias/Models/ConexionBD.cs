using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Asistencias.Models
{
    public class ConexionBD
    {
        public RespuestaBD Consultar(string query, List<SqlParameter> parametros)
        {
            RespuestaBD respuesta = new RespuestaBD();
            if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Base"].ConnectionString))
                    {
                        try
                        {
                            connection.Open();
                        }
                        catch (Exception)
                        {
                            respuesta.Mensaje = "Error al conectar con el servidor de base de datos.";
                        }

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            if (parametros != null)
                            {
                                command.Parameters.AddRange(parametros.ToArray());
                            }
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                respuesta.Data = new DataTable();
                                adapter.Fill(respuesta.Data);
                                respuesta.Estatus = EstatusRespuesta.Ok;
                                respuesta.Mensaje = "Consulta realizada correctamente";
                            }
                        }
                        try
                        {
                            connection.Close();
                        }
                        catch (Exception)
                        {
                            respuesta.Mensaje = "Error al desconectar con el servidor de base de datos.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Mensaje = ex.Message;
                }
            }
            else
            {
                respuesta.Mensaje = "No se cuenta con instrucción.";
            }
            return respuesta;
        }
    }
    public class RespuestaBD
    {
        public EstatusRespuesta Estatus { get; set; } = EstatusRespuesta.Error;
        public string Mensaje { get; set; } = "";
        public DataTable Data { get; set; } = null;
    }
}