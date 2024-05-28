using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class AuthorizationController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        // GET: Authorization
        [HttpGet]
        public JsonResult UserNV()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var User = (ApiAccount)Session["user"];
                if (User.ApiRoles.Any(role => role.RoleName == "Admin") || User.ApiRoles.Any(role => role.RoleName == "Manager"))
                {
                    return Json(new { code = 300, }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CheckStock()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var User = (ApiAccount)Session["user"];
                var checkRole = new SetRole();
                //if(User.DepartmentName == "BỘ PHẬN KHO XƯỞNG" || User.DepartmentName == "KHO VP HCM" || User.DepartmentName == "KHO - GIAO NHẬN (NX)" || User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "CheckStock"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (headFunction.status == true)
                        {
                            if (checkRole.checkAdminRole(User.UserName, "CheckStock"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult PurchaseManager()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var User = (ApiAccount)Session["user"];
                var checkRole = new SetRole();
                //if (User.DepartmentName == "CUNG ỨNG - XUẤT NHẬP KHẨU" || User.ApiRoles.Any(role=>role.RoleName == "Admin" || role.RoleName == "BillManager" || role.RoleName == "Manager"))
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "PurchaseManager"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (headFunction.status == true)
                        {
                            if (checkRole.checkAdminRole(User.UserName, "PurchaseManager"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai!!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult WarehouseManagement()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var checkRole = new SetRole();
                var User = (ApiAccount)Session["user"];
                //if(User.DepartmentName == "BỘ PHẬN KHO XƯỞNG" || User.DepartmentName == "KHO VP HCM" || User.DepartmentName == "KHO - GIAO NHẬN (NX)" || User.ApiRoles.Any(role => role.RoleName =="Admin" || role.RoleName== "Manager"))
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "WarehouseManagement"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (headFunction.status == true)
                        {
                            if(checkRole.checkAdminRole(User.UserName, "WarehouseManagement"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SalesManager()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var checkRole = new SetRole();
                var User = (ApiAccount)Session["user"];
                //if (User.DepartmentName == "KINH DOANH BARCODE END USER" || User.DepartmentName == "KINH DOANH BARCODE DEALER" || User.DepartmentName == "KINH DOANH HÓA CHẤT END USER" || User.DepartmentName== "KINH DOANH HÓA CHẤT DEALER" 
                //   || User.DepartmentName == "SALE ADMIN" || User.DepartmentName == "SALE ADMIN ĐỆ NHẤT" || User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "BillManager" ||  role.RoleName == "Manager"))
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "SalesManager"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (headFunction.status == true)
                        {
                            if (checkRole.checkAdminRole(User.UserName, "SalesManager"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SystemManagement()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var checkRole = new SetRole();
                var User = (ApiAccount)Session["user"];
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "SystemManagement"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (headFunction.status == true)
                        {
                            if (checkRole.checkAdminRole(User.UserName, "AccountManager"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult AccountManager()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var User = (ApiAccount)Session["user"];
                var checkRole = new SetRole();
                //if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "AccountManager"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (headFunction.status == true)
                        {
                            if (checkRole.checkAdminRole(User.UserName, "AccountManager"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ManageMainCategories()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var User = (ApiAccount)Session["user"];
                var checkRole = new SetRole();
                //if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 300 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var headFunction = (from i in db.HeadFunctions
                                        where i.name == "ManageMainCategories"
                                        select i).FirstOrDefault();
                    if (headFunction == null)
                    {
                        return Json(new { code = 500 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if(headFunction.status == true)
                        {
                            if (checkRole.checkAdminRole(User.UserName, "ManageMainCategories"))
                            {
                                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckRole()
        {
            var User = (ApiAccount)Session["user"];
            //if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
            var href = Request.Form["href"];
            var id = Request.Form["id"];
            if (String.IsNullOrEmpty(href))
            {
                return Json(new { code = 400 });
            }
            else
            {
                if (User.ApiRoles.Any(role => role.RoleName == "Admin" || role.RoleName == "Manager"))
                {
                    return Json(new { code = 200, success = true });
                }
                else
                {
                    var userId = User.UserName;
                    var userRole = (from i in db.Roles
                                    where i.Username == userId
                                    select i).FirstOrDefault();
                    if (userRole != null)
                    {
                        var checkRole = new SetRole();
                        var nameFunction = Request.Form["function"];
                        if (checkRole.checkRole(userRole.Username, nameFunction))
                        {
                            return Json(new { code = 200, success = true });
                        }
                        else
                        {
                            return Json(new { code = 200, success = false });
                        }
                    }
                    else
                    {
                        return Json(new { code = 500 });
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult CheckAPIFunction()
        {
            try
            {
                var api = WebConfigurationManager.AppSettings["ApiHostname"].ToString();
                var name = Request.Form["name"];
                var f = (from i in db.Functions
                         where i.Id == name
                         select i).FirstOrDefault();
                if (f != null)
                {
                    if (f.Status == true)
                    {
                        var function = new Function();
                        function.Id = f.Id;
                        function.IdMenu = f.IdMenu;
                        function.NameReturnJson = f.NameReturnJson;
                        function.NameFunctionAPI = f.NameFunctionAPI;
                        function.StatusReturnJson = f.StatusReturnJson;
                        function.ReturnJson = f.ReturnJson;
                        function.Status = f.Status;
                        function.Method = f.Method;
                        function.ReturnValue = f.ReturnValue;
                        //var nameFunctionAPI = f.NameFunctionAPI;
                        //var returnJson = f.ReturnJson;
                        //var statusReturnJson = f.StatusReturnJson;
                        //var nameReturnJson = f.NameReturnJson;

                        var paramsF = (from i in db.ParamFunctions
                                      where i.IdFunction == function.Id
                                      select new
                                      {
                                          idFuntion = i.IdFunction,
                                          name = i.Name,
                                          dataType = i.DataTypes
                                      }).ToList();

                        return Json(new { code = 200, status = true, apiHost = api, apiFunction = function, paramsF = paramsF});
                    }
                    else
                    {
                        return Json(new { code = 200, status = false });
                    }
                }
                else
                {
                    return Json(new { code = 400 });
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}