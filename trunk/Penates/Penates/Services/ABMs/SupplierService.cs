using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.ABMs;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Services.ABMs {
    public class SupplierService : ISupplierService {

        ISupplierRepository repository = new SupplierRepository();

        /// <summary> Guarda los datos de un proveedor en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como SupplierViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(SupplierViewModel supplier) {
            long value = this.repository.Save(supplier); //Capturo el ID o Errores del Sp
            switch (value) {
                case -1:
                    var ex = new IDNotFoundException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.SupplierWArt, supplier.SupplierID), String.Format(Resources.ExceptionMessages.IDNotFoundException,
                        Resources.Resources.SupplierWArt ,supplier.SupplierID));
                    ex.atributeName = ReflectionExtension.getVarName(() => supplier.SupplierID);
                    throw ex;
            }
            if (value < -1) {//Si es otro error
                throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.SupplierWArt), String.Format(Resources.ExceptionMessages.IDNotFoundException,
                    supplier.SupplierID));
            }
            return value;
        }

        public bool Delete(long id) {
            return this.repository.Delete(id);
        }

        public SupplierViewModel getData(long id) {
            Supplier supplier = this.repository.getData(id, false);
            if (supplier == null) {
                return null;
            }
            SupplierViewModel aux = new SupplierViewModel();
            aux.SupplierID = supplier.SupplierID;
            aux.Name = supplier.Name;
            aux.Address = supplier.Address;
            aux.Email = supplier.Email;
            aux.Phone = supplier.Phone;

            return aux;
        }

        public List<List<string>> getDisplayData(int start, int length, int sortColumnIndex, string sortDirection, ref long total) {
            Sorts sort = this.toSort(sortColumnIndex, sortDirection);
            return this.getDisplayData(start, length, sort, ref total);
        }

        public List<List<string>> getDisplayData(int start, int length, Sorts sort, ref long total) {
            var result = this.getData(null, start, length, sort, ref total);
            return this.toJsonArray(result);
        }

        
        public List<List<string>> toJsonArray(IQueryable<Supplier> query){
            List<Supplier> list = query.ToList();
            List<List<string>> result = new List<List<string>>();
            List<string> aux;
            foreach (Supplier sup in list) {
                aux = new List<string>() {sup.SupplierID.ToString(), sup.Name, sup.Address, sup.Email};
                result.Add(aux);
            }
            return result;
        }





        public List<List<string>> getDisplayData(string search, int start, int length, int sortColumnIndex, string sortDirection, ref long total) {
            Sorts sort = this.toSort(sortColumnIndex, sortDirection);
            return this.getDisplayData(search, start, length, sort, ref total);
        }


        public List<List<string>> getDisplayData(string search, int start, int length, Sorts sort, ref long total) {
            var result = this.getData(search, start, length, sort, ref total);
            return this.toJsonArray(result);
        }

        public List<ConstraintViewModel> getConstrains(long supplierID){
            List<ConstraintViewModel> constraintsList = new List<ConstraintViewModel>();

            ISupplierRepository repo = new SupplierRepository(Properties.Settings.Default.nConstrainsToView);
            ConstraintViewModel constraint;
            long constraintsCount = 0;

            constraintsCount = repo.getCommercialAgreementsConstraintsNumber(supplierID);
            if (constraintsCount > 0) {
                constraint = new ConstraintViewModel(
                    String.Format(Resources.Constraints.ConstraintTitle, Resources.Resources.CommercialAgreements),
                    String.Format(Resources.Constraints.ConstraintMessage, Resources.Resources.CommercialAgreements, Resources.Resources.Suppliers));
                constraint.Count = constraintsCount;
                constraint.TableWithConstrain = "CommercialAgreements";
                constraint.constraints = repo.getCommercialAgreementsConstraints(supplierID);
                constraintsList.Add(constraint);
            }
            constraintsCount = repo.getOrderConstraintsNumber(supplierID);
            if (constraintsCount > 0) {
                constraint = new ConstraintViewModel(
                    String.Format(Resources.Constraints.ConstraintTitle, Resources.Resources.Receptions),
                    Resources.Constraints.OrderConstraintMessage);
                constraint.Count = constraintsCount;
                constraint.TableWithConstrain = "SupplierOrders";
                constraint.constraints = repo.getOrderConstraints(supplierID);
                constraintsList.Add(constraint);
            }
            return constraintsList;
        }

        public IQueryable<Product> getProducts(long? id) {
            if (id.HasValue) {
                return this.repository.getProducts(id.Value);
            } else {
                throw new IDNotFoundException(String.Format(Resources.ExceptionMessages.IDNotFoundException, id));
            }
        }

        public IQueryable<Supplier> getAutocomplete(string search, long? productID) {
            return this.repository.getAutocomplete(search, productID);
        }

        public IQueryable<SupplierOrder> getOrdersAutocomplete(string search, long? supplierID)
        {
            return this.repository.getOrdersAutocomplete(search, supplierID);
        }

        public IQueryable<Supplier> searchAndRank(string search) {
            return this.searchAndRank(search);
        }

        public IQueryable<Supplier> searchAndRank(IQueryable<Supplier> query, string search) {
            return this.repository.searchAndRank(query, search);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Supplier> query) {
            List<Supplier> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Supplier sup in list) {
                aux = new AutocompleteItem() {ID = new { SupplierID = sup.SupplierID } , Label = sup.Name, Description = sup.Address };
                result.Add(aux);
            }
            return result;
        }


        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<SupplierOrder> query)
        {
            List<SupplierOrder> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (SupplierOrder sup in list)
            {
                aux = new AutocompleteItem() { ID = new { SupplierID = sup.SupplierOrderID }, Label = sup.SupplierOrderID, Description = sup.Supplier.Name };
                result.Add(aux);
            }
            return result;
        }

        public Sorts toSort(int sortColumnIndex, string sortDirection) {

            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.NAME;
                    } else {
                        return Sorts.NAME_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.ADDRESS;
                    } else {
                        return Sorts.ADDRESS_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.EMAIL;
                    } else {
                        return Sorts.EMAIL_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException,"Col: " + sortColumnIndex);
            }
        }

        public IQueryable<Supplier> search(IQueryable<Supplier> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Supplier> sort(IQueryable<Supplier> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query; //Si no hay sort no hago nada
            }
        }

        public IQueryable<Supplier> sort(IQueryable<Supplier> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        private IQueryable<Supplier> getData(string search, int start, int length, ref long total) {
            IQueryable<Supplier> result;
            if (search == null) {
                result = repository.getData(start, length, ref total);
            } else {
                search = search.Trim();
                if (search.Equals("")) {
                    result = repository.getData(start, length, ref total);
                } else {
                    result = repository.getData(search, start, length, ref total);
                }
                if (result == null) {
                    result = new List<Supplier>().AsQueryable();
                }
            }
            return result;
        }

        private IQueryable<Supplier> getData(string search, int start, int length, Sorts sort, ref long total) {
            IQueryable<Supplier> result;
            if (search == null) {
                result = repository.getData(start, length, sort, ref total);
            } else {
                search = search.Trim();
                if (search == null || search.Equals("")) {
                    result = repository.getData(start, length, sort, ref total);
                } else {
                    result = repository.getData(search, start, length, sort, ref total);
                }
                if (result == null) {
                    result = new List<Supplier>().AsQueryable();
                }
            }
            return result;
        }
    }
}