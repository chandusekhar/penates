using Penates.Database;
using Penates.Exceptions.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Transactions {
    /// <summary>Repositorio para realizar los movimientos de un lugar a otro</summary>
    public class MovementRepository {

        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public MovementRepository() {
            this.itemsPerPage = 50;
        }

        public MovementRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary>Mueve un Container de un Rack a un Deposito Temporal</summary>
        /// <param name="containerID">Container a Mover</param>
        /// <returns>TemporaryDeposit: retorna el Deposito Temporal al cual se tiene que mover </returns>
        /// <exception cref="InsufficientSpaceException"></exception>
        public TemporaryDeposit moveEmptyContainer(long containerID) {
            Container container = this.db.Containers.Find(containerID);
            if (container.InternalBoxes != null && container.InternalBoxes.Count == 0) {
                return null;
            }
            container.UsedSpace = 0;
            if (container.IDTemporalDeposit.HasValue) {
                return container.TemporaryDeposit;
            }
            InternalDistributionCenter dc = container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.InternalDistributionCenter;
            if (!container.ContainerType.Size.HasValue) {
                container.ContainerType.Size = container.ContainerType.Width * container.ContainerType.Depth *
                    container.ContainerType.Height;
            }
            IEnumerable<TemporaryDeposit> deposits = dc.TemporaryDeposits.Where(x => (x.Size.Value - x.UsedSpace) >= container.ContainerType.Size.Value);
            if (deposits.Count() == 0) {
                throw new InsufficientSpaceException(String.Format(Resources.Errors.MovementError, 
                    Resources.Resources.BoxesWArt, Resources.Resources.TemporaryDepositWArt),
                    String.Format(Resources.ExceptionMessages.TemporaryDepositsOutOfSpace, container.ContainerID, container.IDShelfSubdivition));
            }
            TemporaryDeposit td = deposits.FirstOrDefault();

            container.IDShelfSubdivition = null;
            container.IDTemporalDeposit = td.TemporaryDepositID;
            this.db.Containers.Attach(container);
            var entry = db.Entry(container);
            entry.Property(e => e.IDShelfSubdivition).IsModified = true;
            entry.Property(e => e.IDTemporalDeposit).IsModified = true;

            td = this.db.TemporaryDeposits.Find(td.TemporaryDepositID);
            td.UsedSpace = td.UsedSpace + container.ContainerType.Size.Value;
            this.db.TemporaryDeposits.Attach(td);
            var entry2 = db.Entry(td);
            entry2.Property(e => e.UsedSpace).IsModified = true;
            this.db.SaveChanges();
            return td;
        }
    }
}