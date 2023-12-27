using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asistencias.Models
{
    public class ReporteAsistencia
    {
        public string porcentaje { get; set; }
        public string nombre { get; set; }
        public string no_empleado { get; set; }
        public string area { get; set; }
        public string puesto { get; set; }
        public string activo { get; set; }
        public string email { get; set; }
        public string mes { get; set; }
        public string semana { get; set; }
        public string laborado { get; set; }
        public string entrada { get; set; }
        public string comida_inicio { get; set; }
        public string comida_fin { get; set; }
        public string salida { get; set; }
        public char semaforo { get; set; }
    }
    public class BuscarReporte
    {
        public string area { get; set; } = "";
        public string mes { get; set; } = "";
        public string semana { get; set; } = "";
        public string no_empleado { get; set; } = "";
    }
}