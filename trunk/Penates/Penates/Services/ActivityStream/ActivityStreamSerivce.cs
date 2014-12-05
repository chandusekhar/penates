using Penates.Database;
using Penates.Interfaces.Repositories;
using Penates.Repositories.ActivityStream;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace Penates.Services.ActivityStream
{
    public class ActivityStreamSerivce
    {
        private IActivityStreamRepository repository = new ActivityStreamRepository();

        public IQueryable<Sale> getSales(string UserName)
        {
            return this.repository.getSales(UserName);
        }

        public IQueryable<Transfer> getTransfers(string UserName)
        {
            return this.repository.getTransfers(UserName);
        }

        public IQueryable<Reception> getReceptions(string UserName)
        {
            return this.repository.getReceptions(UserName);
        }

        public List<ActivityStreamTableJson> salesToActivityStream(IQueryable<Sale> query) {
            try {
                return this.salesToActivityStream(query.ToList());
            } catch (EntityException) {
                return new List<ActivityStreamTableJson>();
            }
        }

        public List<ActivityStreamTableJson> salesToActivityStream(ICollection<Sale> list) {
            List<ActivityStreamTableJson> result = new List<ActivityStreamTableJson>();
            ActivityStreamTableJson aux;
            foreach (Sale sale in list) {
                aux = new ActivityStreamTableJson() {
                    Date = sale.SaleDate.ToShortDateString() + " - " + sale.SaleDate.ToLongTimeString(),
                    Message = String.Format(Resources.Messages.SaleActivity, sale.Boxes.Count, sale.Client.Name)
                };

                result.Add(aux);
            }
            return result;
        }

        public List<ActivityStreamTableJson> transfersToActivityStream(IQueryable<Transfer> query) {
            try {
                return this.transfersToActivityStream(query.ToList());
            }catch(EntityException){
                return new List<ActivityStreamTableJson>(); ;
            }
        }

        public List<ActivityStreamTableJson> transfersToActivityStream(ICollection<Transfer> list) {
            List<ActivityStreamTableJson> result = new List<ActivityStreamTableJson>();
            ActivityStreamTableJson aux;
            foreach (Transfer transfer in list) {
                aux = new ActivityStreamTableJson() {
                    Date = transfer.TransferDepartureDate.ToShortDateString() + " - " + transfer.TransferDepartureDate.ToLongTimeString(),
                    Message = String.Format(Resources.Messages.TransferActivity, transfer.Boxes.Count, transfer.DistributionCenter.Address, transfer.DistributionCenterSend.Address)
                };

                result.Add(aux);
            }
            return result;

        }

        public List<ActivityStreamTableJson> receptionsToActivityStream(IQueryable<Reception> query) {
            try {
                return this.receptionsToActivityStream(query.ToList());
            } catch (EntityException) {
                return new List<ActivityStreamTableJson>();
            }
        }

        public List<ActivityStreamTableJson> receptionsToActivityStream(ICollection<Reception> list) {
            List<ActivityStreamTableJson> result = new List<ActivityStreamTableJson>();
            ActivityStreamTableJson aux;
            foreach (Reception reception in list) {
                aux = new ActivityStreamTableJson() {
                    Date = reception.ReceivingDate.ToShortDateString() + " - " + reception.ReceivingDate.ToLongTimeString(),
                    Message = String.Format(Resources.Messages.ReceptionActivity, reception.IDSupplierOrder, reception.DistributionCenter.Address)
                };

                result.Add(aux);
            }
            return result;
        }
    }
}