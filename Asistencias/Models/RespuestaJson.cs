using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asistencias.Models
{
    public enum EstatusRespuesta
    {
        Ok = 1, Error = 2, Invalido = 3
    }
    public class RespuestaJson
    {
        public EstatusRespuesta Estatus { get; set; } = EstatusRespuesta.Error;
        public string Mensaje { get; set; } = string.Empty;
        public object Data { get; set; } = null;

        public T GetObject<T>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(Data));
        }
    }
}