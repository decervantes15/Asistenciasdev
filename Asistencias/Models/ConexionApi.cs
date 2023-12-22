using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Asistencias.Models
{
    public class ConexionApi
    {
        public static async Task<RespuestaJson> Post(string url, object obj)
        {
            RespuestaJson respuesta = new RespuestaJson
            {
                Estatus = EstatusRespuesta.Error,
                Mensaje = "Problemas de conexión con el servidor",
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        using (HttpResponseMessage response = await client.PostAsync(url, content))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaJson>(await response.Content.ReadAsStringAsync());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return respuesta;
        }

        public static async Task<RespuestaJson> Post(string url, object obj, string token)
        {
            RespuestaJson respuesta = new RespuestaJson
            {
                Estatus = EstatusRespuesta.Error,
                Mensaje = "Problemas de conexión con el servidor"
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        using (HttpResponseMessage response = await client.PostAsync(url, content))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaJson>(await response.Content.ReadAsStringAsync());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return respuesta;
        }
    }
}