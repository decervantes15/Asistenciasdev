using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Asistencias.Models
{
    public static class Token
    {
        public static int Vigencia = 10;
        public static string Generar()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            var key = Guid.NewGuid().ToString();
            var salt = "T0k3n4s15t3nc1a5";
            byte[] securedKey = Encoding.ASCII.GetBytes(key + salt);
            string token = Convert.ToBase64String(time.Concat(securedKey).ToArray());
            return token;
        }

        public static bool Validar(string token)
        {
            try
            {
                byte[] data = Convert.FromBase64String(token);
                DateTime vigencia = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                
                if (vigencia < DateTime.UtcNow.AddMinutes(-Vigencia))
                {
                    return false;
                }

                return true;
            }
            catch (Exception) { }

            return false;
        }
    }
}