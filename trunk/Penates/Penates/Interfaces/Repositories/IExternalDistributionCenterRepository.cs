using Penates.Database;
using Penates.Models.ViewModels.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IExternalDistributionCenterRepository {

        /// <summary>Obtiene los datos de un Centro de Distribucion Externo. Omite los ya eliminados</summary>
        /// <param name="id">Id del Centro de Distribucion</param>
        /// <returns></returns>
        ExternalDistributionCenter getData(long id);

        /// <summary>Obtiene los datos de un Centro de Distribucion Externo</summary>
        /// <param name="id">Id del CD</param>
        /// <param name="includeDeleted">True para incluir a los ya eliminados, de lo contrario false</param>
        /// <returns></returns>
        ExternalDistributionCenter getData(long id, bool includeDeleted);

        /// <summary>Obtiene todos los centros de Distribuciones Externos</summary>
        /// <returns></returns>
        IQueryable<ExternalDistributionCenter> getData();

        /// <summary> Guarda los datos de un Centro de Distribucion en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del Centro</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(ExternalDistributionCenterViewModel dc);

        /// <summary>Obtiene los depositos de un Centro de Distribuciones</summary>
        /// <param name="id">Id del Centro de Distribucion a Buscar</param>
        /// <returns>Collection de Depositos</returns>
        ICollection<ExternalBox> getBoxes(long id);

        /// <summary>Obtiene la cantidad de cajas que impiden que se elimine el CD</summary>
        /// <param name="centerID"></param>
        /// <returns></returns>
        long getBoxesConstraintsNumber(long centerID);

        long getProductTypesQuantity(long DistributionCenterID);

        /// <summary>Aumenta el espacio a incrementar o disminuye dependiendo si es + o -</summary>
        /// <param name="dcID">Centro de Distribuciones Externo a Editar</param>
        /// <param name="spaceToIncrement">Espacio a incrementar</param>
        /// <returns>true si lo logra, false si no</returns>
        bool setUsedSpace(long dcID, decimal spaceToIncrement);

        void setExternalDCUsableUsedSpace(long DistributionCenterID, decimal Space);

        long getExternalDistributionCenterProductsQuantity(long ExternalDistributionCenterID);
    }
}
