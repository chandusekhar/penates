using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.ABMs {
    public class ClientRepository : IClientRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(ClientViewModel client) {
            try {
                Client aux = db.Clients.Find(client.ClientID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_Client_Add(client.Email, client.Phone, client.Address, client.Name, client.ContactName, client.CUIT, client.CityID).SingleOrDefault();
                } else {
                    val = db.SP_Client_Edit(client.ClientID, client.Email, client.Phone, client.Address, client.Name, client.ContactName, client.CUIT, client.CityID).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.PackWArt));
            }
        }

        public Status Deactivate(long id) {
            //Se supone que restrinjo para que siempre lo encuentre
            Client client = db.Clients.Find(id);
            if (client == null) {
                return new Status() {
                    Success = false,
                    Message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.ClientWArt, id)
                };
            }
            var tran = this.db.Database.BeginTransaction();
            try {
                string message;
                if (client.Deleted) {
                    client.Deleted = false;
                    message = String.Format(Resources.Messages.ActivatedItem, Resources.Resources.ClientWArt, id);
                } else {
                    client.Deleted = true;
                    message = String.Format(Resources.Messages.DeactivatedItem, Resources.Resources.ClientWArt, id);
                }
                this.db.Clients.Attach(client);
                var entry = db.Entry(client);
                entry.Property(e => e.Deleted).IsModified = true;
                db.SaveChanges();
                tran.Commit();
                return new Status() {
                    Success = true,
                    Message = message
                };
            } catch (Exception e) {
                tran.Rollback();
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.ClientWArt, id), e.Message);
            }
        }

        public Client getData(long id) {
            try {
                return db.Clients.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Client> getData() {
            try {
                return this.db.Clients.Where(x => x.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Client> getData(bool includeDeleted) {
            try {
                if (includeDeleted) {
                    return this.db.Clients;
                } else {
                    return this.getData();
                }
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Client> getAutocomplete(string search) {
            return this.getAutocomplete(search, true);
        }

        public IQueryable<Client> getAutocomplete(string search, bool includeDeleted) {
            try {
                var data = this.searchAndRank(search, includeDeleted);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Client> searchAndRank(string search, bool includeDeleted) {
            var data = this.getData(includeDeleted);
            return this.searchAndRank(data, search);
        }

        public IQueryable<Client> searchAndRank(IQueryable<Client> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Name).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Client> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Client> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.ClientsID).Trim() == item ? 1000 : 0) +
                                ((x.table.Address.Contains(item)) ? item.Length : 0) + ((x.table.Address.StartsWith(item)) ? (item.Length * 2) : 0) +
                                ((x.table.ContactName.Contains(item)) ? item.Length * 2 : 0) + ((x.table.ContactName.StartsWith(item)) ? (item.Length * 4) : 0) +
                                ((x.table.Email.Contains(item)) ? item.Length * 4 : 0) + ((x.table.Email.StartsWith(item)) ? (item.Length * 8) : 0) +
                                ((x.table.CUIT.Contains(item)) ? item.Length * 8 : 0) + ((x.table.CUIT.StartsWith(item)) ? (item.Length * 16) : 0) +
                                ((x.table.Name.Contains(item)) ? item.Length * 8 : 0) + ((x.table.Name.StartsWith(item)) ? (item.Length * 16) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Name)
                    .ThenBy(x => x.table.Email)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Client> search(IQueryable<Client> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Client> search(IQueryable<Client> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Name.Contains(item) || (SqlFunctions.StringConvert((double) p.ClientsID).Trim() == item) ||
                        p.ContactName.Contains(item) || p.Address.Contains(item) || p.Email.Contains(item) || p.CUIT.Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Client> sort(IQueryable<Client> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(p => p.ClientsID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.ClientsID);
                    break;
                case Sorts.CUIT:
                    query = query.OrderBy(p => p.CUIT);
                    break;
                case Sorts.CUIT_DESC:
                    query = query.OrderByDescending(p => p.CUIT);
                    break;
                case Sorts.NAME:
                    query = query.OrderBy(p => p.Name);
                    break;
                case Sorts.NAME_DESC:
                    query = query.OrderByDescending(p => p.Name);
                    break;
                case Sorts.CITY:
                    query = query.OrderBy(p => p.City1.Name);
                    break;
                case Sorts.CITY_DESC:
                    query = query.OrderByDescending(p => p.City1.Name);
                    break;
                case Sorts.EMAIL:
                    query = query.OrderBy(p => p.Email);
                    break;
                case Sorts.EMAIL_DESC:
                    query = query.OrderByDescending(p => p.Email);
                    break;
                default:
                    query = query.OrderBy(p => p.ClientsID);
                    break;
            }
            return query;
        }

        public IQueryable<Client> filterByCity(IQueryable<Client> query, long cityID) {
            return query.Where(x => x.City == cityID);
        }

        public IQueryable<Client> filterByState(IQueryable<Client> query, long provinceID) {
            return query.Where(x => x.City1 != null && x.City1.IDProvince == provinceID);
        }

        public IQueryable<Client> filterByCountry(IQueryable<Client> query, long countryID) {
            return query.Where(x => x.City1 != null && x.City1.Province.IDCountry == countryID);
        }

        public IQueryable<Client> filterByDisabled(IQueryable<Client> query, bool disabled) {
            return query.Where(x => x.Deleted == disabled);
        }
    }
}