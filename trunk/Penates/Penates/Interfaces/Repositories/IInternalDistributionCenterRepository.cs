using Penates.Database;
using Penates.Models.ViewModels.DC;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Penates.Interfaces.Repositories {
    public interface IInternalDistributionCenterRepository {

        /// <summary>Obtiene los datos de un Centro de Distribucion Interno. Omite los ya eliminados</summary>
        /// <param name="id">Id del Centro de Distribucion</param>
        /// <returns></returns>
        InternalDistributionCenter getData(long id);

        /// <summary>Obtiene los datos de un Centro de Distribucion Interno</summary>
        /// <param name="id">Id del CD</param>
        /// <param name="includeDeleted">True para incluir a los ya eliminados, de lo contrario false</param>
        /// <returns></returns>
        InternalDistributionCenter getData(long id, bool includeDeleted);

        /// <summary>Obtiene todos los centros de Distribuciones Internos</summary>
        /// <returns></returns>
        IQueryable<InternalDistributionCenter> getData();

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(InternalDistributionCenterViewModel dc);

        /// <summary>Obtiene los depositos de un Centro de Distribuciones</summary>
        /// <param name="id">Id del Centro de Distribucion a Buscar</param>
        /// <returns>Collection de Depositos</returns>
        ICollection<Deposit> getDeposits(long id);

        /// <summary>Obtiene los depositos temporales de un Centro de Distribuciones</summary>
        /// <param name="id">Id del Centro de Distribucion a Buscar</param>
        /// <returns>Collection de Depositos Temporales</returns>
        ICollection<TemporaryDeposit> getTemporalDeposits(long id);

        long getDepositsConstraintsNumber(long centerID);

        List<Constraint> getDepositsConstraints(long centerID);

        long getTemporaryDepositsConstraintsNumber(long centerID);

        List<Constraint> getTemporaryDepositsConstraints(long centerID);

        IQueryable<InternalDistributionCenter> getAutocomplete(string search);

        long getProductTypesQuantity(long DistributionCenterID);

        InternalDistributionCenterDetails getInternalDistributionCenterDetails (long DistributionCenterCD);

        void updateTempDepositUsedSpace(long DistributionCenterID, long TempDepositID, decimal UsedSpaceInCm3);
    }
}
