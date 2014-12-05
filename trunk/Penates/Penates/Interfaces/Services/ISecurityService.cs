using Penates.Models.ViewModels.Users;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface ISecurityService {

        /// <summary>Trae los parametros de seguridad para mostrarlos</summary>
        /// <returns>SecurityParametersViewModel</returns>
        SecurityParametersViewModel getParameters();

        /// <summary>Guarda las Settings de la applicacion</summary>
        /// <param name="parameters">SecurityParametersViewModel</param>
        /// <returns>Status</returns>
        Status setParameters(SecurityParametersViewModel parameters);

        int? getSessionTimeout();

        int? getUsernameMaxLenght();

        int? getUsernameMinLenght();

        long? getUserNumberOfLowerCase();

        long? getUserNumberOfUpperCase();

        long? getUserNumberOfDigits();

        long? getUserNumberOfSymbols();

        int? getPasswordMaxLenght();

        int? getPasswordMinLenght();

        long? getPasswordNumberOfLowerCase();

        long? getPasswordNumberOfUpperCase();

        long? getPasswordNumberOfDigits();

        long? getPasswordNumberOfSymbols();
    }
}
