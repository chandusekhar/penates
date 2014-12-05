using Penates.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models
{
    public class CookieUser : ICookieUser
    {
        private const string NombreCookie = "datosUsuario";
        private void Inicializar()
        {
            if (HttpContext.Current.Request.Cookies[NombreCookie] != null)
            {
                var cookie = HttpContext.Current.Request.Cookies[NombreCookie];
                HttpContext.Current.Response.SetCookie(cookie);
            }
            else
            {
                NuevaCookie();
            }
        }

        private static HttpCookie NuevaCookie()
        {
            var nuevacookie = new HttpCookie(NombreCookie);

            nuevacookie.Values["userName"] = "";
            nuevacookie.Values["userRole"] = "";

            HttpContext.Current.Response.Cookies.Remove(NombreCookie);
            HttpContext.Current.Response.SetCookie(nuevacookie);
            return nuevacookie;
        }

        public string Valor(string clave)
        {
            var cookie = HttpContext.Current.Request.Cookies[NombreCookie];
            if (cookie == null)
            {
                cookie = NuevaCookie();
            }
            return cookie.Values[clave];
        }

        public void ActualizarValor(string clave, string valor)
        {
            var cookie = HttpContext.Current.Response.Cookies[NombreCookie];
            if (cookie == null || !cookie.HasKeys)
            {
                Inicializar();
                cookie = HttpContext.Current.Response.Cookies[NombreCookie];
            }
            cookie.Values[clave] = valor;
            HttpContext.Current.Response.Cookies.Remove(NombreCookie);
            HttpContext.Current.Response.SetCookie(cookie);
        }

    }
}