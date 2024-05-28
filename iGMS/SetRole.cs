using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;

namespace WMS
{
    public class SetRole
    {
        private WMSEntities db = new WMSEntities();
        public Boolean SetNewRole(string username)
        {
            try
            {
                var newRole = new Role();
                newRole.Username = (username);
                newRole.AddWareHouse = true;
                newRole.EditWareHouse = true;
                newRole.DeleteWareHouse = true;
                newRole.AddGoods = true;
                newRole.EditGoods = true;
                newRole.DeleteGoods = true;
                newRole.AddCustomer = true;
                newRole.EditCustomer = true;
                newRole.DeleteCustomer = true;
                newRole.AddTypeCustomer = true;
                newRole.EditTypeCustomer = true;
                newRole.DeleteTypeCustomer = true;
                newRole.AddUser = true;
                newRole.EditUser = true;
                newRole.DeleteUser = true;
                newRole.EditPassWord = false;
                newRole.BackUpSQL = true;
                newRole.AddGroupGoods = true;
                newRole.EditGroupGoods = true;
                newRole.DeleteGroupGoods = true;

                db.Roles.Add(newRole);
                db.SaveChanges();
                return true;
            }catch (Exception e)
            {
                return false;
            }
        }

        public Boolean SetNewAdminRole(string username)
        {
            try
            {
                var newRole = new RoleAdmin();
                newRole.Username = (username);
                newRole.ManageMainCategories = true;
                newRole.PurchaseManager = true;
                newRole.SalesManager = true;
                newRole.WarehouseManagement = true;
                newRole.SystemManagement = false;
                newRole.AccountManager = true;

                db.RoleAdmins.Add(newRole);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Boolean checkAdminRole(string username, string role)
        {
            var check = false;
            try
            {
                var checkAdmin = (from i in db.RoleAdmins
                           where i.Username == username
                           select i).FirstOrDefault();
                if(checkAdmin == null)
                {
                    check = false;
                }
                else
                {
                    switch(role)
                    {
                        case "ManageMainCategories":
                            if (checkAdmin.ManageMainCategories == true)
                            {
                                check = true;
                            }
                            break;
                        case "PurchaseManager":
                            if (checkAdmin.PurchaseManager == true)
                            {
                                check = true;
                            }
                            break;
                        case "SalesManager":
                            if (checkAdmin.SalesManager == true)
                            {
                                check = true;
                            }
                            break;
                        case "WarehouseManagement":
                            if (checkAdmin.WarehouseManagement == true)
                            {
                                check = true;
                            }
                            break;
                        case "AccountManager":
                            if (checkAdmin.AccountManager == true)
                            {
                                check = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                check = false;
            }
            return check;
        }

        public Boolean checkRole(int roleId, string role)
        {
            var check = false;
            try
            {
                var checkUser = (from i in db.Roles
                                  where i.Id == roleId
                                  select i).FirstOrDefault();
                if (checkUser == null)
                {
                    check = false;
                }
                else
                {
                    switch (role)
                    {
                        case "AddWareHouse":
                            if (checkUser.AddWareHouse == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditWareHouse":
                            if (checkUser.EditWareHouse == true)
                            {
                                check = true;
                            }
                            break;
                        case "DeleteWareHouse":
                            if (checkUser.DeleteWareHouse == true)
                            {
                                check = true;
                            }
                            break;
                        case "AddGoods":
                            if (checkUser.AddGoods == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditGoods":
                            if (checkUser.EditGoods == true)
                            {
                                check = true;
                            }
                            break;
                        case "DeleteGoods":
                            if (checkUser.DeleteGoods == true)
                            {
                                check = true;
                            }
                            break;
                        case "AddCustomer":
                            if (checkUser.AddCustomer == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditCustomer":
                            if (checkUser.EditCustomer == true)
                            {
                                check = true;
                            }
                            break;
                        case "DeleteCustomer":
                            if (checkUser.DeleteCustomer == true)
                            {
                                check = true;
                            }
                            break;
                        case "AddTypeCustomer":
                            if (checkUser.AddTypeCustomer == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditTypeCustomer":
                            if (checkUser.EditTypeCustomer == true)
                            {
                                check = true;
                            }
                            break;
                        case "DeleteTypeCustomer":
                            if (checkUser.DeleteTypeCustomer == true)
                            {
                                check = true;
                            }
                            break;
                        case "AddUser":
                            if (checkUser.AddUser == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditUser":
                            if (checkUser.EditUser == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditPassWord":
                            if (checkUser.EditPassWord == true)
                            {
                                check = true;
                            }
                            break;
                        case "DeleteUser":
                            if (checkUser.DeleteUser == true)
                            {
                                check = true;
                            }
                            break;
                        case "BackUpSQL":
                            if (checkUser.BackUpSQL == true)
                            {
                                check = true;
                            }
                            break;
                        case "AddGroupGoods":
                            if (checkUser.AddGroupGoods == true)
                            {
                                check = true;
                            }
                            break;
                        case "EditGroupGoods":
                            if (checkUser.EditGroupGoods == true)
                            {
                                check = true;
                            }
                            break;
                        case "DeleteGroupGoods":
                            if (checkUser.DeleteGroupGoods == true)
                            {
                                check = true;
                            }
                            break;
                        case "Unit":
                            check = true;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                check = false;
            }
            return check;
        }
    }
}