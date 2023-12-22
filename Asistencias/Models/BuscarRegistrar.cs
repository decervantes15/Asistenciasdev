using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asistencias.Models
{
    public class BuscarRegistrar
    {
        public string username { get;set; }
        public string password { get;set; }
        public string tipo_registro { get;set; }
        public string fecha_inicio { get;set; }
        public string fecha_fin { get; set; }
        public string ip { get; set; }
    }
}