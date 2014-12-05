using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Users;
using Penates.Repositories.Users;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Services.Users {
    public class RoleService : IRoleService {

        RoleRepository repository = new RoleRepository();

        public IQueryable<Role> getData() {
            return this.repository.getData();
        }

        public Status DeleteRole(int roleID) {
            return repository.DeleteRole(roleID);
        }

        public Roles GetRole(long roleID) {
            return repository.GetRole(roleID);
        }

        public Status EditRole(Roles role) {
            return repository.EditRole(role);
        }

        /// <summary>Obtiene la lista de roles</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getRolesList() {
            return this.getRolesList(false);
        }

        /// <summary>Obtiene la lista de roles</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getRolesList(bool includeAll) {
            var countries = this.getRoles();
            if (includeAll) {
                countries.Insert(0, new Role { RoleID = -1, Description = Resources.Resources.All });
                return new SelectList(countries, "RoleID", "Description", -1);
            }
            return new SelectList(countries, "RoleID", "Description");
        }

        public List<RoleTableJson> toJsonArray(IQueryable<Role> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<RoleTableJson> toJsonArray(ICollection<Role> list) {
            List<RoleTableJson> result = new List<RoleTableJson>();
            RoleTableJson aux;
            foreach (Role role in list) {
                aux = new RoleTableJson() {
                    RoleID = role.RoleID,
                    Description = role.Description
                };
                result.Add(aux);
            }
            return result;
        }

        /// <summary>Obtiene los paises existentes para mostrar</summary>
        /// <returns>SelectList: Lista de las categorias</returns>
        /// <exception cref="DatabaseException"
        private List<Role> getRoles() {
            try {
                return this.repository.getData().ToList();
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }
    }
}