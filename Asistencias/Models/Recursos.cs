using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asistencias.Models
{
    public class Recursos
    {
        public string no_empleado { get; set; }
        public string email { get; set; }
        public string recurso { get; set; }
        public string ip { get; set; }
        public string direccion { get; set; }
        public string area { get; set; }
        public string equipo { get; set; }
        public string rol { get; set; }
        public string puesto { get; set; }
    }

    public class Area
    {
        public string id { get; set; }
        public string area { get; set; }
    }
}