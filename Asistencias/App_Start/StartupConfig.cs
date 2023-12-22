using Asistencias.App_Start;
using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Security.Principal;

[assembly: OwinStartup(typeof(StartupConfig))]

namespace Asistencias.App_Start
{
    public class StartupConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = ConfigurationManager.AppSettings["AppCookie"],
                LoginPath = new PathString("/"),
                LogoutPath = new PathString("/Salir"),
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });
        }
    }

    public class AppUsuario : ClaimsPrincipal
    {
        public string token => FindFirst("token").Value;
        public string usuario => FindFirst("usuario").Value;
        public AppUsuario(IIdentity identity) : base(identity)
        {
        }
    }
}