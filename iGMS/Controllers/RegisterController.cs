using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using System.Data.Entity.ModelConfiguration;
using System.Text.RegularExpressions;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;
using System.Drawing.Printing;
using Microsoft.Ajax.Utilities;
//using Microsoft.SqlServer.Management.Smo;

namespace WMS.Controllers
{
    public class RegisterController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListUser()
        {
            return View();
        }
        public ActionResult EditUser(string id)
        {
            if (id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        /*public ActionResult EditAuth(string id)
        {
            if (id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var username = "";
            if (!id.Contains("-"))
            {
                ViewBag.Username = id;
                username = id;
            }
            else
            {
                var splitUsername = Regex.Split(id, "-");
                for(var i = 0; i < splitUsername.Length; i++)
                {
                    if(i == splitUsername.Length - 1)
                    {
                        username += splitUsername[i];
                        break;
                    }
                    username += splitUsername[i];
                    username += ".";
                }
            }
            ViewBag.Username = username;
            var role = (from i in db.Roles
                        where i.Username == username
                        select i).FirstOrDefault();
            if(role == null)
            {
                role = new Role();
                role.AddCustomer = false;
                role.EditCustomer = false;
                role.DeleteCustomer = false;
                role.AddGroupGoods = false;
                role.EditGroupGoods = false;
                role.DeleteGroupGoods = false;
                role.AddGoods = false;
                role.EditGoods = false;
                role.DeleteGoods = false;
                role.AddWareHouse = false;
                role.EditWareHouse = false;
                role.DeleteWareHouse = false;
                role.AddTypeCustomer = false;
                role.EditTypeCustomer = false;
                role.DeleteTypeCustomer = false;
                role.AddUser = false;
                role.EditUser = false;
                role.EditPassWord = false;
                role.DeleteUser = false;
                role.BackUpSQL = false;
                role.Username = id;
            }
            var roleAdmin = (from i in db.RoleAdmins
                             where i.Username == username
                             select i).FirstOrDefault();
            if(roleAdmin == null)
            {
                roleAdmin = new RoleAdmin();
                roleAdmin.ManageMainCategories = false;
                roleAdmin.PurchaseManager = false;
                roleAdmin.SalesManager = false;
                roleAdmin.WarehouseManagement = false;
                roleAdmin.SystemManagement = false;
                roleAdmin.AccountManager = false;
                roleAdmin.Username = id;
            }

            ViewBag.Role = role;
            ViewBag.RoleAdmin = roleAdmin;
            //User user = db.Users.Find(id);
            //if (user == null)
            //{
                //return HttpNotFound();
            //}
            return View();
        }*/

        public ActionResult EditPass(string id)
        {
            if (id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpGet]
        public JsonResult List(int pagenum,int page,string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.Users.Where(x => x.Id.Length>0)
                         join uc in db.Users on b.CreateBy equals uc.Id
                         join um in db.Users on b.ModifyBy equals um.Id
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                             user = b.User1,
                             createDate = b.CreateDate,
                             createBy = uc.Name,
                             modifyDate = b.ModifyDate,
                             modifyBy = um.Name,
                             ManageMainCategories = b.RoleAdmin1.ManageMainCategories == true ? "Quản Lý Danh Mục Sản Phẩm, " : "",
                             PurchaseManager = b.RoleAdmin1.PurchaseManager == true ? "Quản Lý Mua Hàng, " : "",
                             SalesManager = b.RoleAdmin1.SalesManager == true ? "Quản Lý Bán Hàng, " : "",
                             WarehouseManagement= b.RoleAdmin1.WarehouseManagement == true? "Quản Lý Danh Mục Kho, ":"",
                             SystemManagement = b.RoleAdmin1.SystemManagement == true? "Quản Lý Hệ Thống, ":"",
                             AccountManager = b.RoleAdmin1.AccountManager == true? "Quản Lý Tài Khoản, ":"",
                             AddWarehouse = b.Role1.AddWareHouse == true ? "Thêm Kho, ": "",
                             EditWarehouse = b.Role1.EditWareHouse == true? "Sửa Kho, ":"",
                             DeleteWarehouse = b.Role1.DeleteWareHouse == true? "Xóa Kho, ": "",
                             AddGoods = b.Role1.AddGoods == true? "Thêm Sản Phẩm, ": "",
                             EditGoods = b.Role1.EditGoods == true? "Sửa Sản Phẩm, ":"",
                             DeleteGoods = b.Role1.DeleteGoods == true? "Xóa Sản Phẩm, ":"",
                             AddCustomer = b.Role1.AddCustomer == true? "Thêm Khách Hàng, ": "",
                             EditCustomer = b.Role1.EditCustomer == true? "Sửa Khách Hàng, ":"",
                             DeleteCustomer = b.Role1.DeleteCustomer == true? "Xóa Khách Hàng, ":"",
                             AddTypeCustomer = b.Role1.AddTypeCustomer == true? "Thêm Loại Khách Hàng, ":"",
                             EditTypeCustomer = b.Role1.EditTypeCustomer == true? "Sửa Loại Khách Hàng, ": "",
                             DeleteTypeCustomer = b.Role1.DeleteTypeCustomer == true? "Xóa Loại Khách Hàng, ": "",
                             AddUser = b.Role1.AddUser == true? "Thêm Người Dùng, ":"",
                             EditUser = b.Role1.EditUser == true? "Sửa Người Dùng, ":"",
                             EditPassWord = b.Role1.EditPassWord == true? "Sửa Mật Khẩu User, ": "",
                             DeleteUser = b.Role1.DeleteUser == true? "Xóa Người Dùng, ":"",
                             BackupSql = b.Role1.BackUpSQL ==true? "Sao Lưu Dữ Liệu, ": "",
                             AddGroupGoods =b.Role1.AddGroupGoods == true? "Thêm Nhóm Hàng, ":"",
                             EditGroupGoods = b.Role1.EditGroupGoods == true? "Sửa Nhóm Hàng, ": "",
                             DeleteGroupGoods = b.Role1.DeleteGroupGoods == true? "Xóa Nhóm Hàng, ":"",
                         }).ToList().Where(x=>x.id.ToLower().Contains(seach)||x.name.ToLower().Contains(seach));
                var pages = a.Count() % pageSize == 0 ? a.Count() / pageSize : a.Count() / pageSize + 1;
                var c = a.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = a.Count();
                return Json(new { code = 200,c = c,pages = pages,count =count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ListApi()
        {
            var listUser = Request.Form["listUser"];
            var getList = JsonConvert.SerializeObject(listUser);
            if (!String.IsNullOrEmpty(listUser))
            {
                var role = (from i in db.Roles
                            select new
                            {
                                username = i.Username,
                                AddWarehouse = i.AddWareHouse == true ? "Thêm Kho, " : "",
                                EditWarehouse = i.EditWareHouse == true ? "Sửa Kho, " : "",
                                DeleteWarehouse = i.DeleteWareHouse == true ? "Xóa Kho, " : "",
                                AddGoods = i.AddGoods == true ? "Thêm Sản Phẩm, " : "",
                                EditGoods = i.EditGoods == true ? "Sửa Sản Phẩm, " : "",
                                DeleteGoods = i.DeleteGoods == true ? "Xóa Sản Phẩm, " : "",
                                AddCustomer = i.AddCustomer == true ? "Thêm Khách Hàng, " : "",
                                EditCustomer = i.EditCustomer == true ? "Sửa Khách Hàng, " : "",
                                DeleteCustomer = i.DeleteCustomer == true ? "Xóa Khách Hàng, " : "",
                                AddTypeCustomer = i.AddTypeCustomer == true ? "Thêm Loại Khách Hàng, " : "",
                                EditTypeCustomer = i.EditTypeCustomer == true ? "Sửa Loại Khách Hàng, " : "",
                                DeleteTypeCustomer = i.DeleteTypeCustomer == true ? "Xóa Loại Khách Hàng, " : "",
                                AddUser = i.AddUser == true ? "Thêm Người Dùng, " : "",
                                EditUser = i.EditUser == true ? "Sửa Người Dùng, " : "",
                                EditPassWord = i.EditPassWord == true ? "Sửa Mật Khẩu User, " : "",
                                DeleteUser = i.DeleteUser == true ? "Xóa Người Dùng, " : "",
                                BackupSql = i.BackUpSQL == true ? "Sao Lưu Dữ Liệu, " : "",
                                AddGroupGoods = i.AddGroupGoods == true ? "Thêm Nhóm Hàng, " : "",
                                EditGroupGoods = i.EditGroupGoods == true ? "Sửa Nhóm Hàng, " : "",
                                DeleteGroupGoods = i.DeleteGroupGoods == true ? "Xóa Nhóm Hàng, " : "",
                            }).ToList();
                var roleAdmin = (from i in db.RoleAdmins
                                 select new
                                 {
                                     username = i.Username,
                                     ManageMainCategories = i.ManageMainCategories == true ? "Quản Lý Danh Mục Sản Phẩm, " : "",
                                     PurchaseManager = i.PurchaseManager == true ? "Quản Lý Mua Hàng, " : "",
                                     SalesManager = i.SalesManager == true ? "Quản Lý Bán Hàng, " : "",
                                     WarehouseManagement = i.WarehouseManagement == true ? "Quản Lý Danh Mục Kho, " : "",
                                     SystemManagement = i.SystemManagement == true ? "Quản Lý Hệ Thống, " : "",
                                     AccountManager = i.AccountManager == true ? "Quản Lý Tài Khoản, " : "",
                                 }).ToList();
                return Json(new { code = 200, data = getList, role = role, roleAdmin = roleAdmin});
            }
            else
            {
                return Json(new {code = 500, data= getList});
            }
        }

        [HttpPost]
        public JsonResult getRole()
        {
            try
            {
                var username = (Request.Form["username"]);
                var role = (from i in db.Roles
                            join u in db.Users on i.Username equals u.User1
                            where i.Username == username
                           select new
                           {
                               username = username,
                               AddWarehouse = i.AddWareHouse == true ? "Thêm Kho, " : "",
                               EditWarehouse = i.EditWareHouse == true ? "Sửa Kho, " : "",
                               DeleteWarehouse = i.DeleteWareHouse == true ? "Xóa Kho, " : "",
                               AddGoods = i.AddGoods == true ? "Thêm Sản Phẩm, " : "",
                               EditGoods = i.EditGoods == true ? "Sửa Sản Phẩm, " : "",
                               DeleteGoods = i.DeleteGoods == true ? "Xóa Sản Phẩm, " : "",
                               AddCustomer = i.AddCustomer == true ? "Thêm Khách Hàng, " : "",
                               EditCustomer = i.EditCustomer == true ? "Sửa Khách Hàng, " : "",
                               DeleteCustomer = i.DeleteCustomer == true ? "Xóa Khách Hàng, " : "",
                               AddTypeCustomer = i.AddTypeCustomer == true ? "Thêm Loại Khách Hàng, " : "",
                               EditTypeCustomer = i.EditTypeCustomer == true ? "Sửa Loại Khách Hàng, " : "",
                               DeleteTypeCustomer = i.DeleteTypeCustomer == true ? "Xóa Loại Khách Hàng, " : "",
                               AddUser = i.AddUser == true ? "Thêm Người Dùng, " : "",
                               EditUser = i.EditUser == true ? "Sửa Người Dùng, " : "",
                               EditPassWord = i.EditPassWord == true ? "Sửa Mật Khẩu User, " : "",
                               DeleteUser = i.DeleteUser == true ? "Xóa Người Dùng, " : "",
                               BackupSql = i.BackUpSQL == true ? "Sao Lưu Dữ Liệu, " : "",
                               AddGroupGoods = i.AddGroupGoods == true ? "Thêm Nhóm Hàng, " : "",
                               EditGroupGoods = i.EditGroupGoods == true ? "Sửa Nhóm Hàng, " : "",
                               DeleteGroupGoods = i.DeleteGroupGoods == true ? "Xóa Nhóm Hàng, " : "",
                           }).ToList();

                var roleAdmin = (from i in db.RoleAdmins
                                 where i.Username == username
                                 select new
                                 {
                                     username = username,
                                     ManageMainCategories = i.ManageMainCategories == true ? "Quản Lý Danh Mục Sản Phẩm, " : "",
                                     PurchaseManager = i.PurchaseManager == true ? "Quản Lý Mua Hàng, " : "",
                                     SalesManager = i.SalesManager == true ? "Quản Lý Bán Hàng, " : "",
                                     WarehouseManagement = i.WarehouseManagement == true ? "Quản Lý Danh Mục Kho, " : "",
                                     SystemManagement = i.SystemManagement == true ? "Quản Lý Hệ Thống, " : "",
                                     AccountManager = i.AccountManager == true ? "Quản Lý Tài Khoản, " : "",
                                 }).ToList();
                return Json(new { code = 200, role = role, roleAdmin = roleAdmin});
            }
            catch
            {
                return Json(new { code = 500 });
            }
        }

        [HttpPost]
        public JsonResult Register()
        {
            try
            {
                var session = (User)Session["user"];
                var roleuser = session.Role1.AddUser;
                if(roleuser == false|| roleuser == null)
                {
                    return Json(new { code = 500, msg = "Bạn Không Có Quyền !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Request.Form.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(Request.Form["User"]) || Request.Form["User"].Length < 5)
                        {
                            return Json(new { code = 500, msg = "Tài Khoản Phải Hơn 5 Kí Tự !!!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (!IsValidInput(Request.Form["User"]))
                        {
                            return Json(new { code = 500, msg = "Tài Khoản Chỉ Chứa Chữ Cái Và Số !!!" }, JsonRequestBehavior.AllowGet);
                        }
                        if (string.IsNullOrWhiteSpace(Request.Form["Pass"]) || Request.Form["Pass"].Length <= 6)
                        {
                            return Json(new { code = 500, msg = "Mật Khẩu Phải Hơn 6 Kí Tự !!!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (!IsValidInput(Request.Form["Pass"]))
                        {
                            return Json(new { code = 500, msg = "Mật Khẩu Chỉ Chứa Chữ Cái Và Số !!!" }, JsonRequestBehavior.AllowGet);
                        }
                        var us = Request.Form["User"];
                        var manageMaincategories = string.IsNullOrWhiteSpace(Request.Form["ManageMainCategories"]) ? false : true;
                        var purchasemanager = string.IsNullOrWhiteSpace(Request.Form["PurchaseManager"]) ? false : true;
                        var salesManager = string.IsNullOrWhiteSpace(Request.Form["SalesManager"]) ? false : true;
                        var warehousemanagement = string.IsNullOrWhiteSpace(Request.Form["WarehouseManagement"]) ? false : true;
                        var systemmanagement = string.IsNullOrWhiteSpace(Request.Form["SystemManagement"]) ? false : true;
                        var accountmanager = string.IsNullOrWhiteSpace(Request.Form["AccountManager"]) ? false : true;
                        var checkRoleAdmin = db.RoleAdmins.SingleOrDefault(x => x.ManageMainCategories == manageMaincategories
                                                                                && x.PurchaseManager == purchasemanager
                                                                                && x.SalesManager == salesManager
                                                                                && x.WarehouseManagement == warehousemanagement
                                                                                && x.SystemManagement == systemmanagement
                                                                                && x.AccountManager == accountmanager);


                        var addwarehouse = string.IsNullOrWhiteSpace(Request.Form["addwarehouse"]) ? false : true;
                        var editwarehouse = string.IsNullOrWhiteSpace(Request.Form["editwarehouse"]) ? false : true;
                        var deletewarehouse = string.IsNullOrWhiteSpace(Request.Form["deletewarehouse"]) ? false : true;
                        var addgoods = string.IsNullOrWhiteSpace(Request.Form["addgoods"]) ? false : true;
                        var editgoods = string.IsNullOrWhiteSpace(Request.Form["editgoods"]) ? false : true;
                        var deletegoods = string.IsNullOrWhiteSpace(Request.Form["deletegoods"]) ? false : true;
                        var addcustomer = string.IsNullOrWhiteSpace(Request.Form["addcustomer"]) ? false : true;
                        var editcustomer = string.IsNullOrWhiteSpace(Request.Form["editcustomer"]) ? false : true;
                        var deletecustomer = string.IsNullOrWhiteSpace(Request.Form["deletecustomer"]) ? false : true;
                        var addtypecustomer = string.IsNullOrWhiteSpace(Request.Form["addtypecustomer"]) ? false : true;
                        var edittypecustomer = string.IsNullOrWhiteSpace(Request.Form["edittypecustomer"]) ? false : true;
                        var deletetypecustomer = string.IsNullOrWhiteSpace(Request.Form["deletetypecustomer"]) ? false : true;
                        var adduser = string.IsNullOrWhiteSpace(Request.Form["adduser"]) ? false : true;
                        var edituser = string.IsNullOrWhiteSpace(Request.Form["edituser"]) ? false : true;
                        var editpassword = string.IsNullOrWhiteSpace(Request.Form["editpassword"]) ? false : true;
                        var deleteuser = string.IsNullOrWhiteSpace(Request.Form["deleteuser"]) ? false : true;
                        var backupsql = string.IsNullOrWhiteSpace(Request.Form["backupsql"]) ? false : true;
                        var addgroupgoods = string.IsNullOrWhiteSpace(Request.Form["addgroupgoods"]) ? false : true;
                        var editgroupgoods = string.IsNullOrWhiteSpace(Request.Form["editgroupgoods"]) ? false : true;
                        var deletegroupgoods = string.IsNullOrWhiteSpace(Request.Form["deletegroupgoods"]) ? false : true;


                        var checkrole = db.Roles.SingleOrDefault(x => x.AddWareHouse == addwarehouse && x.EditWareHouse == editwarehouse && x.DeleteWareHouse == deletewarehouse
                                                                && x.AddGoods == addgoods && x.EditGoods == editgoods && x.DeleteGoods == deletegoods
                                                                && x.AddCustomer == addcustomer && x.EditCustomer == editcustomer && x.DeleteCustomer == deletecustomer
                                                                && x.AddTypeCustomer == addtypecustomer && x.EditTypeCustomer == edittypecustomer && x.DeleteTypeCustomer == deletetypecustomer
                                                                && x.AddUser == adduser && x.EditUser == edituser && x.EditPassWord == editpassword && x.DeleteUser == deleteuser
                                                                && x.BackUpSQL == backupsql && x.AddGroupGoods == addgroupgoods && x.EditGroupGoods == editgroupgoods && x.DeleteGroupGoods == deletegroupgoods);

                        var idRoleAdmin = (int?)null;
                        if (checkRoleAdmin == null)
                        {
                            RoleAdmin roleAdmin = new RoleAdmin()
                            {
                                ManageMainCategories = manageMaincategories,
                                PurchaseManager = purchasemanager,
                                SalesManager = salesManager,
                                WarehouseManagement = warehousemanagement,
                                SystemManagement = systemmanagement,
                                AccountManager = accountmanager,
                            };
                            db.RoleAdmins.Add(roleAdmin);
                            db.SaveChanges();
                            idRoleAdmin = db.RoleAdmins.OrderBy(x => x.Id).ToList().LastOrDefault().Id;
                        }
                        else
                        {
                            idRoleAdmin = checkRoleAdmin.Id;
                        }

                        var idrole = (int?)null;
                        // Nếu quyền rolewarehouse đã được chọn, kiểm tra xem có quyền role đã được tạo hay chưa
                        if (checkrole == null)
                        {
                            Role role = new Role()
                            {
                                AddWareHouse = addwarehouse,
                                EditWareHouse = editwarehouse,
                                DeleteWareHouse = deletewarehouse,
                                AddGoods = addgoods,
                                EditGoods = editgoods,
                                DeleteGoods = deletegoods,
                                AddCustomer = addcustomer,
                                EditCustomer = editcustomer,
                                DeleteCustomer = deletecustomer,
                                AddTypeCustomer = addtypecustomer,
                                EditTypeCustomer = edittypecustomer,
                                DeleteTypeCustomer = deletetypecustomer,
                                AddUser = adduser,
                                EditUser = edituser,
                                EditPassWord = editpassword,
                                DeleteUser = deleteuser,
                                BackUpSQL = backupsql,
                                AddGroupGoods = addgroupgoods,
                                EditGroupGoods = editgroupgoods,
                                DeleteGroupGoods = deletegroupgoods,
                            };
                            db.Roles.Add(role);
                            db.SaveChanges();
                            idrole = db.Roles.OrderBy(x => x.Id).ToList().LastOrDefault().Id;
                        }
                        else
                        {
                            idrole = checkrole.Id;
                        }

                        var checkUser = db.Users.SingleOrDefault(x => x.User1 == us);
                        if (checkUser == null)
                        {
                            var date = DateTime.Now;
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(us);
                            string base64String = Convert.ToBase64String(bytes);
                            User user = new User()
                            {
                                Id = base64String + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond,
                                User1 = us,
                                Pass = Encode.ToMD5(Request.Form["Pass"]),
                                Name = Request.Form["Name"],
                                AddRess = Request.Form["AddRess"],
                                Email = Request.Form["Email"],
                                Birth = string.IsNullOrWhiteSpace(Request.Form["Birth"]) ? (DateTime?)null : DateTime.Parse(Request.Form["Birth"]),
                                Phone = string.IsNullOrWhiteSpace(Request.Form["Phone"]) ? null : Request.Form["Phone"],
                                Description = Request.Form["Description"],
                                RoleAdmin = idRoleAdmin,
                                Role = idrole,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                CreateBy = session.Id,
                                ModifyBy = session.Id,
                                FailPass = 0,
                                Status = true,
                            };
                            db.Users.Add(user);
                            db.SaveChanges();
                            return Json(new { code = 200, msg = "Tạo Tài Khoản Thành Công" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { code = 500, msg = "Tài Khoản Đã Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Không Có Dữ Liệu" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Edit()
        {
            try
            {
                var session = (User)Session["user"];
                var roleuser = session.Role1.EditUser;
                if(roleuser == false || roleuser == null)
                {
                    return Json(new { code = 500, msg = "Bạn Không Có Quyền !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Request.Form.Count > 0)
                    {
                        var manageMaincategories = string.IsNullOrWhiteSpace(Request.Form["ManageMainCategories"]) ? false : true;
                        var purchasemanager = string.IsNullOrWhiteSpace(Request.Form["PurchaseManager"]) ? false : true;
                        var salesManager = string.IsNullOrWhiteSpace(Request.Form["SalesManager"]) ? false : true;
                        var status = string.IsNullOrWhiteSpace(Request.Form["Status"]) ? false : true;
                        var warehousemanagement = string.IsNullOrWhiteSpace(Request.Form["WarehouseManagement"]) ? false : true;
                        var systemmanagement = string.IsNullOrWhiteSpace(Request.Form["SystemManagement"]) ? false : true;
                        var accountmanager = string.IsNullOrWhiteSpace(Request.Form["AccountManager"]) ? false : true;
                        var checkRoleAdmin = db.RoleAdmins.SingleOrDefault(x => x.ManageMainCategories == manageMaincategories
                                                                                && x.PurchaseManager == purchasemanager
                                                                                && x.SalesManager == salesManager
                                                                                && x.WarehouseManagement == warehousemanagement
                                                                                && x.SystemManagement == systemmanagement
                                                                                && x.AccountManager == accountmanager);

                        var addwarehouse = string.IsNullOrWhiteSpace(Request.Form["addwarehouse"]) ? false : true;
                        var editwarehouse = string.IsNullOrWhiteSpace(Request.Form["editwarehouse"]) ? false : true;
                        var deletewarehouse = string.IsNullOrWhiteSpace(Request.Form["deletewarehouse"]) ? false : true;
                        var addgoods = string.IsNullOrWhiteSpace(Request.Form["addgoods"]) ? false : true;
                        var editgoods = string.IsNullOrWhiteSpace(Request.Form["editgoods"]) ? false : true;
                        var deletegoods = string.IsNullOrWhiteSpace(Request.Form["deletegoods"]) ? false : true;
                        var addcustomer = string.IsNullOrWhiteSpace(Request.Form["addcustomer"]) ? false : true;
                        var editcustomer = string.IsNullOrWhiteSpace(Request.Form["editcustomer"]) ? false : true;
                        var deletecustomer = string.IsNullOrWhiteSpace(Request.Form["deletecustomer"]) ? false : true;
                        var addtypecustomer = string.IsNullOrWhiteSpace(Request.Form["addtypecustomer"]) ? false : true;
                        var edittypecustomer = string.IsNullOrWhiteSpace(Request.Form["edittypecustomer"]) ? false : true;
                        var deletetypecustomer = string.IsNullOrWhiteSpace(Request.Form["deletetypecustomer"]) ? false : true;
                        var adduser = string.IsNullOrWhiteSpace(Request.Form["adduser"]) ? false : true;
                        var edituser = string.IsNullOrWhiteSpace(Request.Form["edituser"]) ? false : true;
                        var editpassword = string.IsNullOrWhiteSpace(Request.Form["editpassword"]) ? false : true;
                        var deleteuser = string.IsNullOrWhiteSpace(Request.Form["deleteuser"]) ? false : true;
                        var backupsql = string.IsNullOrWhiteSpace(Request.Form["backupsql"]) ? false : true;
                        var addgroupgoods = string.IsNullOrWhiteSpace(Request.Form["addgroupgoods"]) ? false : true;
                        var editgroupgoods = string.IsNullOrWhiteSpace(Request.Form["editgroupgoods"]) ? false : true;
                        var deletegroupgoods = string.IsNullOrWhiteSpace(Request.Form["deletegroupgoods"]) ? false : true;


                        var checkrole = db.Roles.SingleOrDefault(x => x.AddWareHouse == addwarehouse && x.EditWareHouse == editwarehouse && x.DeleteWareHouse == deletewarehouse
                                                                && x.AddGoods == addgoods && x.EditGoods == editgoods && x.DeleteGoods == deletegoods
                                                                && x.AddCustomer == addcustomer && x.EditCustomer == editcustomer && x.DeleteCustomer == deletecustomer
                                                                && x.AddTypeCustomer == addtypecustomer && x.EditTypeCustomer == edittypecustomer && x.DeleteTypeCustomer == deletetypecustomer
                                                                && x.AddUser == adduser && x.EditUser == edituser && x.EditPassWord == editpassword && x.DeleteUser == deleteuser
                                                                && x.BackUpSQL == backupsql && x.AddGroupGoods == addgroupgoods && x.EditGroupGoods == editgroupgoods && x.DeleteGroupGoods == deletegroupgoods);

                        var idRoleAdmin = (int?)null;
                        var Id = Request.Form["Id"];
                        if (checkRoleAdmin == null)
                        {
                            RoleAdmin roleAdmin = new RoleAdmin()
                            {
                                ManageMainCategories = manageMaincategories,
                                PurchaseManager = purchasemanager,
                                SalesManager = salesManager,
                                WarehouseManagement = warehousemanagement,
                                SystemManagement = systemmanagement,
                                AccountManager = accountmanager,

                            };
                            db.RoleAdmins.Add(roleAdmin);
                            db.SaveChanges();
                            idRoleAdmin = db.RoleAdmins.OrderBy(x => x.Id).ToList().LastOrDefault().Id;
                        }
                        else
                        {
                            idRoleAdmin = checkRoleAdmin.Id;
                        }

                        var idrole = (int?)null;
                        // Nếu quyền rolewarehouse đã được chọn, kiểm tra xem có quyền role đã được tạo hay chưa
                        if (checkrole == null)
                        {
                            Role role = new Role()
                            {
                                AddWareHouse = addwarehouse,
                                EditWareHouse = editwarehouse,
                                DeleteWareHouse = deletewarehouse,
                                AddGoods = addgoods,
                                EditGoods = editgoods,
                                DeleteGoods = deletegoods,
                                AddCustomer = addcustomer,
                                EditCustomer = editcustomer,
                                DeleteCustomer = deletecustomer,
                                AddTypeCustomer = addtypecustomer,
                                EditTypeCustomer = edittypecustomer,
                                DeleteTypeCustomer = deletetypecustomer,
                                AddUser = adduser,
                                EditUser = edituser,
                                EditPassWord = editpassword,
                                DeleteUser = deleteuser,
                                BackUpSQL = backupsql,
                                AddGroupGoods = addgroupgoods,
                                EditGroupGoods = editgroupgoods,
                                DeleteGroupGoods = deletegroupgoods,
                            };
                            db.Roles.Add(role);
                            db.SaveChanges();
                            idrole = db.Roles.OrderBy(x => x.Id).ToList().LastOrDefault().Id;
                        }
                        else
                        {
                            idrole = checkrole.Id;
                        }

                        var checkUser = db.Users.Find(Id);
                        if (checkUser != null)
                        {
                            checkUser.Name = Request.Form["Name"];
                            checkUser.AddRess = Request.Form["AddRess"];
                            checkUser.Email = Request.Form["Email"];
                            checkUser.Birth = string.IsNullOrWhiteSpace(Request.Form["Birth"]) ? (DateTime?)null : DateTime.Parse(Request.Form["Birth"]);
                            checkUser.Phone = string.IsNullOrWhiteSpace(Request.Form["Phone"]) ? null : Request.Form["Phone"];
                            checkUser.Description = Request.Form["Description"];
                            checkUser.RoleAdmin = idRoleAdmin;
                            checkUser.Role = idrole;
                            checkUser.ModifyDate = DateTime.Now;
                            checkUser.ModifyBy = session.Id;
                            checkUser.Status = status;
                            checkUser.FailPass = status == true ? 0 : checkUser.FailPass;
                            db.SaveChanges();
                            // Chỉ cập nhật phiên đăng nhập khi tự sửa user cho mình
                            if (checkUser.Id == session.Id)
                            {
                                Session["user"] = checkUser;
                            }
                            return Json(new { code = 200, msg = "Cập Nhật Tài Khoản Thành Công" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { code = 500, msg = "Tài Khoản Không Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Không Có Dữ Liệu" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /*[HttpPost]
        public JsonResult EditAuth()
        {
            try
            {
                //var session = (User)Session["user"];
                var user = (ApiAccount)Session["user"];
                //var roleuser = session.Role1.EditUser;
                var checkRole = new SetRole();
                if (!checkRole.checkRole(user.UserName, "EditUser"))
                //if(roleuser == false || roleuser == null)
                {
                    return Json(new { code = 500, msg = "Bạn Không Có Quyền !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Request.Form.Count > 0)
                    {
                        var username = Request.Form["Username"];

                        var manageMaincategories = string.IsNullOrWhiteSpace(Request.Form["ManageMainCategories"]) ? false : true;
                        var purchasemanager = string.IsNullOrWhiteSpace(Request.Form["PurchaseManager"]) ? false : true;
                        var salesManager = string.IsNullOrWhiteSpace(Request.Form["SalesManager"]) ? false : true;
                        var status = string.IsNullOrWhiteSpace(Request.Form["Status"]) ? false : true;
                        var warehousemanagement = string.IsNullOrWhiteSpace(Request.Form["WarehouseManagement"]) ? false : true;
                        var systemmanagement = string.IsNullOrWhiteSpace(Request.Form["SystemManagement"]) ? false : true;
                        var accountmanager = string.IsNullOrWhiteSpace(Request.Form["AccountManager"]) ? false : true;

                        var addwarehouse = string.IsNullOrWhiteSpace(Request.Form["addwarehouse"]) ? false : true;
                        var editwarehouse = string.IsNullOrWhiteSpace(Request.Form["editwarehouse"]) ? false : true;
                        var deletewarehouse = string.IsNullOrWhiteSpace(Request.Form["deletewarehouse"]) ? false : true;
                        var addgoods = string.IsNullOrWhiteSpace(Request.Form["addgoods"]) ? false : true;
                        var editgoods = string.IsNullOrWhiteSpace(Request.Form["editgoods"]) ? false : true;
                        var deletegoods = string.IsNullOrWhiteSpace(Request.Form["deletegoods"]) ? false : true;
                        var addcustomer = string.IsNullOrWhiteSpace(Request.Form["addcustomer"]) ? false : true;
                        var editcustomer = string.IsNullOrWhiteSpace(Request.Form["editcustomer"]) ? false : true;
                        var deletecustomer = string.IsNullOrWhiteSpace(Request.Form["deletecustomer"]) ? false : true;
                        var addtypecustomer = string.IsNullOrWhiteSpace(Request.Form["addtypecustomer"]) ? false : true;
                        var edittypecustomer = string.IsNullOrWhiteSpace(Request.Form["edittypecustomer"]) ? false : true;
                        var deletetypecustomer = string.IsNullOrWhiteSpace(Request.Form["deletetypecustomer"]) ? false : true;
                        var adduser = string.IsNullOrWhiteSpace(Request.Form["adduser"]) ? false : true;
                        var edituser = string.IsNullOrWhiteSpace(Request.Form["edituser"]) ? false : true;
                        var editpassword = string.IsNullOrWhiteSpace(Request.Form["editpassword"]) ? false : true;
                        var deleteuser = string.IsNullOrWhiteSpace(Request.Form["deleteuser"]) ? false : true;
                        var backupsql = string.IsNullOrWhiteSpace(Request.Form["backupsql"]) ? false : true;
                        var addgroupgoods = string.IsNullOrWhiteSpace(Request.Form["addgroupgoods"]) ? false : true;
                        var editgroupgoods = string.IsNullOrWhiteSpace(Request.Form["editgroupgoods"]) ? false : true;
                        var deletegroupgoods = string.IsNullOrWhiteSpace(Request.Form["deletegroupgoods"]) ? false : true;


                        var checkRoleAdmin = db.RoleAdmins.SingleOrDefault(x => x.Username == username);

                        var checkrole = db.Roles.SingleOrDefault(x => x.Username == username);

                        //var idRoleAdmin = (int?)null;
                        //var Id = Request.Form["Id"];
                        if (checkRoleAdmin == null)
                        {
                            RoleAdmin roleAdmin = new RoleAdmin()
                            {
                                ManageMainCategories = manageMaincategories,
                                PurchaseManager = purchasemanager,
                                SalesManager = salesManager,
                                WarehouseManagement = warehousemanagement,
                                SystemManagement = systemmanagement,
                                AccountManager = accountmanager,
                                Username = username
                            };
                            db.RoleAdmins.Add(roleAdmin);
                            db.SaveChanges();
                            //idRoleAdmin = db.RoleAdmins.OrderBy(x => x.Id).ToList().LastOrDefault().Id;
                        }
                        else
                        {
                            checkRoleAdmin.SalesManager = salesManager;
                            checkRoleAdmin.PurchaseManager = purchasemanager;
                            checkRoleAdmin.ManageMainCategories = manageMaincategories;
                            checkRoleAdmin.WarehouseManagement = warehousemanagement;
                            checkRoleAdmin.AccountManager = accountmanager;
                            db.SaveChanges();
                            //idRoleAdmin = checkRoleAdmin.Id;
                        }

                        //var idrole = (int?)null;
                        // Nếu quyền rolewarehouse đã được chọn, kiểm tra xem có quyền role đã được tạo hay chưa
                        if (checkrole == null)
                        {
                            Role role = new Role()
                            {
                                AddWareHouse = addwarehouse,
                                EditWareHouse = editwarehouse,
                                DeleteWareHouse = deletewarehouse,
                                AddGoods = addgoods,
                                EditGoods = editgoods,
                                DeleteGoods = deletegoods,
                                AddCustomer = addcustomer,
                                EditCustomer = editcustomer,
                                DeleteCustomer = deletecustomer,
                                AddTypeCustomer = addtypecustomer,
                                EditTypeCustomer = edittypecustomer,
                                DeleteTypeCustomer = deletetypecustomer,
                                AddUser = adduser,
                                EditUser = edituser,
                                EditPassWord = editpassword,
                                DeleteUser = deleteuser,
                                BackUpSQL = backupsql,
                                AddGroupGoods = addgroupgoods,
                                EditGroupGoods = editgroupgoods,
                                DeleteGroupGoods = deletegroupgoods,
                                Username = username
                            };
                            db.Roles.Add(role);
                            db.SaveChanges();
                            //idrole = db.Roles.OrderBy(x => x.Id).ToList().LastOrDefault().Id;
                        }
                        else
                        {
                            checkrole.AddWareHouse = addwarehouse;
                            checkrole.EditWareHouse = editwarehouse;
                            checkrole.DeleteWareHouse = deletewarehouse;
                            checkrole.AddGoods = addgoods;
                            checkrole.EditGoods = editgoods;
                            checkrole.DeleteGoods = deletegoods;
                            checkrole.AddCustomer = addcustomer;
                            checkrole.EditCustomer = editcustomer;
                            checkrole.DeleteCustomer = deletecustomer;
                            checkrole.AddTypeCustomer = addtypecustomer;
                            checkrole.EditTypeCustomer = edittypecustomer;
                            checkrole.DeleteTypeCustomer = deletetypecustomer;
                            checkrole.AddUser = adduser;
                            checkrole.EditUser = edituser;
                            checkrole.EditPassWord = editpassword;
                            checkrole.DeleteUser = deleteuser;
                            checkrole.BackUpSQL = backupsql;
                            checkrole.AddGroupGoods = addgroupgoods;
                            checkrole.EditGroupGoods = editgroupgoods;
                            checkrole.DeleteGroupGoods = deletegroupgoods;
                            db.SaveChanges();
                            //idrole = checkrole.Id;
                        }
                        return Json(new { code = 200, msg = "Cập Nhật Tài Khoản Thành Công" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { code = 500, msg = "Không Có Dữ Liệu" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }*/

        [HttpPost]
        public JsonResult EditPassword()
        {
            try
            {
                var session = (User)Session["user"];
                var roleuser = session.Role1.EditPassWord;
                if (roleuser ==false || roleuser == null)
                {
                    return Json(new { code = 500, msg = "Bạn Không Có Quyền !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Request.Form.Count > 0)
                    {
                        var id = Request.Form["Id"];
                        var user = db.Users.Find(id);
                        if (user != null)
                        {
                            if (user.FailPass == 5)
                            {
                                return Json(new { code = 500, msg = "Tài Khoản Đã Bị Khóa, Liên Hệ Quản Trị Viên Để Mở Khóa!!!" }, JsonRequestBehavior.AllowGet);
                            }
                            var passOld = Request.Form["Pass"];
                            if (Encode.ToMD5(passOld) == user.Pass)
                            {
                                if (Request.Form["PassNew"] == Request.Form["PassNewAgain"])
                                {
                                    user.Pass = Encode.ToMD5(Request.Form["PassNew"]);
                                    user.FailPass = 0;
                                    db.SaveChanges();
                                    return Json(new { code = 200, msg = "Đổi mật Khẩu Thành Công" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { code = 500, msg = "Nhập Lại Mật Khẩu Mới Không khớp!!!" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                user.FailPass += 1;
                                if (user.FailPass == 5) { user.Status = false; };
                                db.SaveChanges();
                                return Json(new
                                {
                                    code = 500,
                                    msg = user.FailPass != 5 ? "Mật Khẩu Không Đúng !!!\nTài Khoản Đã Nhập Sai Mật Khẩu " + user.FailPass + " Lần, Bị Khóa Sau " + (5 - user.FailPass) + " Lần Nhập Sai!"
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
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                var session = (User)Session["user"];
                var roleuser = session.Role1.DeleteUser;
                if (roleuser == false || roleuser == null)
                {
                    return Json(new { code = 500, msg = "Bạn Không Có Quyền !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var d = db.Users.Find(id);
                    db.Users.Remove(d);
                    db.SaveChanges();
                    return Json(new { code = 200, msg = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = 500,msg ="Xóa Thất Bại"  }, JsonRequestBehavior.AllowGet);
            }
        }
        static bool IsValidInput(string input)
        {
            // Biểu thức chính quy để kiểm tra xem chuỗi có chứa ký tự đặc biệt hay không
            // Trong trường hợp này, chỉ cho phép các ký tự chữ cái và số, có thể tùy chỉnh theo yêu cầu của bạn.
            Regex regex = new Regex("^[a-zA-Z0-9!@#$%^&*()-_+=]+$");

            // Kiểm tra sự khớp của chuỗi với biểu thức chính quy
            return regex.IsMatch(input);
        }
    }
}