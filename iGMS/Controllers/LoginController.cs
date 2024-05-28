using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class LoginController : Controller
    {
        private WMSEntities db = new WMSEntities();
        // GET: Login
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        public ActionResult Login()
        {
            try
            {
                var api = WebConfigurationManager.AppSettings["ApiHostname"].ToString();
                var name = "/Login/Login";
                var f = (from i in db.Functions
                         where i.Id == name
                         select i).FirstOrDefault();
                if(f != null)
                {
                    var function = new Function();
                    function.Id = f.Id;
                    function.IdMenu = f.IdMenu;
                    function.NameReturnJson = f.NameReturnJson;
                    function.NameFunctionAPI = f.NameFunctionAPI;
                    function.StatusReturnJson = f.StatusReturnJson;
                    function.Status = f.Status;
                    function.Method = f.Method;
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
                    ViewBag.Function = function;
                    ViewBag.Params = paramsF;
                }
                else
                {
                    ViewBag.Function = null;
                }
            }catch(Exception ex)
            {
                throw new Exception();
            }
            return View();
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

                        return Json(new { code = 200, status = true, apiHost = api, apiFunction = function, paramsF = paramsF });
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


        //Chính
        [HttpGet]
        public JsonResult LoginiGMS()
        {
            try
            {
                if (Request.Params.Count > 0)
                {
                    var user = Request.Params["username"];
                    var pass = Request.Params["password"];
                    //var checkUser = db.Users.SingleOrDefault(x => x.User1 == user);
                    var checkUser = (from i in db.Users
                                     where i.User1 == user
                                     select i).FirstOrDefault();
                    if (checkUser != null)
                    {
                        if (checkUser.FailPass == 5)
                        {
                            return Json(new { code = 500, msg = "Tài Khoản Đã Bị Khóa, Liên Hệ Quản Trị Viên Để Mở Khóa!!!" }, JsonRequestBehavior.AllowGet);
                        }
                        if (checkUser.Pass == Encode.ToMD5(pass))
                        {
                            checkUser.LoginTime = DateTime.Now;
                            checkUser.FailPass = 0;
                            db.SaveChanges();

                            var apiAccount = new ApiAccount();
                            if(checkUser.Id == "NV0000001")
                            {
                                apiAccount.FullName = checkUser.Name;
                                apiAccount.UserName = checkUser.User1;
                                apiAccount.DepartmentId = 2;
                                apiAccount.DepartmentName = "Kinh Doanh";
                                List<ApiRole> listApiRole = new List<ApiRole>();
                                var apiRole = new ApiRole();
                                apiRole.RoleId = 1;
                                apiRole.RoleName = "Admin";
                                apiRole.RoleDes = "Admin";
                                listApiRole.Add(apiRole);
                                apiAccount.ApiRoles = listApiRole;
                            }
                            else
                            {
                                apiAccount.FullName = checkUser.Name;
                                apiAccount.UserName = checkUser.User1;
                                apiAccount.DepartmentId = 2;
                                apiAccount.DepartmentName = "Kinh Doanh";
                                List<ApiRole> listApiRole = new List<ApiRole>();
                                var apiRole = new ApiRole();
                                apiRole.RoleId = 4;
                                apiRole.RoleName = "Staff";
                                apiRole.RoleDes = "Nhân viên";
                                listApiRole.Add(apiRole);
                                apiAccount.ApiRoles = listApiRole;
                            }
                            Session["user"] = apiAccount;
                            Session["roleadmin"] = db.RoleAdmins.Find(checkUser.RoleAdmin);
                            Session["role"] = db.Roles.Find(checkUser.Role);
                            return Json(new { code = 200, msg = "Đăng Nhập Thành Công", url = "/Home/Index" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            checkUser.FailPass += 1;
                            if (checkUser.FailPass == 5) { checkUser.Status = false; };
                            db.SaveChanges();
                            return Json(new
                            {
                                code = 500,
                                msg = checkUser.FailPass != 5 ? "Mật Khẩu Không Đúng !!!\nTài Khoản Đã Nhập Sai Mật Khẩu " + checkUser.FailPass + " Lần, Bị Khóa Sau " + (5 - checkUser.FailPass) + " Lần Nhập Sai!"
                                                                                : "Tài Khoản Đã Bị Khóa Vì Nhập Mật Khẩu Sai 5 Lần Liên Tiếp!!!"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Tài Khoản Không Tồn Tại !!!", data= Request.Params }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { code = 500, msg = "Không Có Dữ Liệu !!!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //Phụ
        /*[HttpPost]
        public JsonResult LoginiGMS()
        {
            try
            {
                if (Request.Form.Count > 0)
                {
                    var user = Request.Form["username"];
                    var pass = Request.Form["password"];
                    var checkUser = db.Users.SingleOrDefault(x => x.User1 == user);
                    if (checkUser != null)
                    {
                        if (checkUser.FailPass == 5)
                        {
                            return Json(new { code = 500, msg = "Tài Khoản Đã Bị Khóa, Liên Hệ Quản Trị Viên Để Mở Khóa!!!" }, JsonRequestBehavior.AllowGet);
                        }
                        if (checkUser.Pass == Encode.ToMD5(pass))
                        {
                            checkUser.LoginTime = DateTime.Now;
                            checkUser.FailPass = 0;
                            db.SaveChanges();
                            Session["user"] = checkUser;
                            return Json(new { code = 200, msg = "Đăng Nhập Thành Công", url = "/Home/Index" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            checkUser.FailPass += 1;
                            if (checkUser.FailPass == 5) { checkUser.Status = false; };
                            db.SaveChanges();
                            return Json(new
                            {
                                code = 500,
                                msg = checkUser.FailPass != 5 ? "Mật Khẩu Không Đúng !!!\nTài Khoản Đã Nhập Sai Mật Khẩu " + checkUser.FailPass + " Lần, Bị Khóa Sau " + (5 - checkUser.FailPass) + " Lần Nhập Sai!"
                                                                                : "Tài Khoản Đã Bị Khóa Vì Nhập Mật Khẩu Sai 5 Lần Liên Tiếp!!!"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Tài Khoản Không Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { code = 500, msg = "Không Có Dữ Liệu !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }*/
        [HttpGet]
        public JsonResult LoginApi(ApiAccount apiAccount, string ApiRolesString)
        {
            try
            {
                List<ApiRole> ApiRoles = JsonConvert.DeserializeObject<List<ApiRole>>(ApiRolesString);

                // Gán danh sách ApiRoles vào đối tượng ApiAccount
                apiAccount.ApiRoles = ApiRoles;
                var username = apiAccount.UserName;

                var roleUser = (from i in db.Roles
                               where i.Username == username
                               select i).FirstOrDefault();
                var roleAdmin = (from i in db.RoleAdmins
                                 where i.Username == username
                                 select i).FirstOrDefault();
                var setNewRole = new SetRole();
                if (roleUser == null)
                {
                    if (setNewRole.SetNewRole(username))
                    {
                        if (roleAdmin == null)
                        {
                            if (setNewRole.SetNewAdminRole(username) == false)
                            {
                                return Json(new { code = 500, msg = "Chưa tạo được !" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Chưa tạo được !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (roleAdmin == null)
                    {
                        if (setNewRole.SetNewAdminRole(username) == false)
                        {
                            return Json(new { code = 500, msg = "Chưa tạo được !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                Session["user"] = apiAccount;
                return Json(new { code = 200, msg = " Tài Khoản Hợp Lệ !", data = Session["user"] }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Scan(string id)
        {
            try
            {
                var a = db.Users.SingleOrDefault(x => x.Id==id);
                var user = a.User1;
                var pass = a.Pass;
                if (a != null)
                {
                    a.LoginTime = DateTime.Now;
                    db.SaveChanges();
                    a = db.Users.SingleOrDefault(x => x.Id == id);
                    Session["user"] = a;
                    return Json(new { code = 200, Url = "/Home/Index", user= user, pass= pass }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 300, msg = "Tài Khoản Hoặc Mật Khẩu không Đúng !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        } 
    }
}