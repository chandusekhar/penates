using System;
namespace Penates.Interfaces.Models {
    public interface ICookieUser {
        void ActualizarValor(string clave, string valor);
        string Valor(string clave);
    }
}
