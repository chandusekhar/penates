using Penates.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Users {
    public class RoleRepository : IRoleRepository
    {

        PenatesEntities db = new PenatesEntities();

        public IQueryable<Role> getData() {
            var db = new PenatesEntities();

            return db.Roles;
        }

        public Status DeleteRole(int RoleID) {
            try {
                var db = new PenatesEntities();

                db.Roles.Where(x => x.RoleID == RoleID);
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        public Roles GetRole(long RoleID) 
        {
            var db = new PenatesEntities();

            var role = db.Roles.FirstOrDefault(x => x.RoleID == RoleID);

            var result = new Roles();
            result.RoleId = role.RoleID;
            result.RoleDesciption = role.Description;

            return result;
        }

        public Status EditRole(Roles Role) {
            try 
            {
                var db = new PenatesEntities();

                var role = db.Roles.FirstOrDefault(x => x.RoleID == Role.RoleId);

                role.Description = Role.RoleDesciption;
                
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }
    }
}