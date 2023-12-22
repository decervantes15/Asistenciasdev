using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asistencias.Models
{
    public class AsistenciasModel
    {
        public string no_empleado { get; set; }
        public string email { get; set; }
        public string recurso { get; set; }
        public string ip { get; set; }
        public string area { get; set; }
        public string equipo { get; set; }
        public string rol { get; set; }
        public string puesto { get; set; }
        public string comentarios { get; set; }
        public string entrada { get; set; }
        public string comida_inicio { get; set; }
        public string comida_fin { get; set; }
        public string salida { get; set; }
    }
}