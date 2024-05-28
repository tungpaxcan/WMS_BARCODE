using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;


namespace WMS.Controllers
{
    public class HomeController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        //Chính
        public ActionResult Index()
        {
            ///--
            ViewBag.Admin = Session["roleadmin"];

            var c = Session["user"];
            SesionUser s = new SesionUser();
            s.InventoryNotification();
            s.PurchaseNotification();
            s.SaleOrderNotification();
            return View(c);

            //var c = (User)Session["user"];
            //Chat MC = new Chat();
            //// Check if the event is already subscribed
            //if (!MC.IsSqlDepEventSubscribed)
            //{
            //    MC.RegisterNotification(c.Id);
            //}
            //SesionUser s = new SesionUser();
            //// Check if the event is already subscribed
            //if (!s.IsSqlDepEventSubscribed)
            //{
            //    s.RegisterNotification();
            //}
            //return View();
        }
        //Phụ
        /*public ActionResult Index()
        {
            //var c = (ApiAccount)Session["user"];
            //return View(c);


            return View();
        }*/
        public ActionResult Logout()
        {
            Session.Remove("user");
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public JsonResult UserSession()
        {
            try
            {
                var user = (Models.User)Session["user"];
                // hiển thị tên máy chủ và tên csdl
                var server = "Không Có Kết Nối";
                var database = "Không Có Kết Nối";
                string connectionString = ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString;
                EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(connectionString);
                server = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString).DataSource;
                database = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString).InitialCatalog;
                return Json(new { code = 200, user, server, database }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi !!!" + ex.Message }, JsonRequestBehavior.AllowGet); ;

            }
            //try
            //{
            //    var c = (User)Session["user"];
            //    var server = "Không có kết nối";
            //    var database = "Không có kết nối";
            //    string loginTime = QueryDateVsTime.Date(c.LoginTime.Value);
            //    string connectionString = ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString;
            //    EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(connectionString);
            //    // Hiển thị tên máy chủ và tên cơ sở dữ liệu
            //    server = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString).DataSource;
            //    database = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString).InitialCatalog;
            //    return Json(new { code = 200, loginTime = loginTime, user = c.User1, server, database, email = c.Email }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception e)
            //{
            //    return Json(new { code = 500, msg = "Lỗi !!!" + e.Message }, JsonRequestBehavior.AllowGet); ;
            //}
        }

        [HttpPost]
        public JsonResult PurchaseNotify()
        {
            try
            {
                var purchases = db.PurchaseOrders.ToList();
                var notifies = db.PurchaseNotifies.ToList();

                foreach (var purchase in purchases)
                {
                    var idPurchase = purchase.Id;
                    var status = purchase.Status;

                    if (status == false)
                    {
                        var existingNotify = notifies.FirstOrDefault(n => n.IdPurchaseOrder == idPurchase);

                        if (existingNotify == null)
                        {
                            var newNotify = new PurchaseNotify
                            {
                                IdPurchaseOrder = purchase.Id,
                                Message = $"Số Phiếu Nhập {purchase.Id} Chưa Quét.",
                                Status = false
                            };


                            notifies.Add(newNotify);
                            db.PurchaseNotifies.Add(newNotify);
                        }
                        else
                        {
                            existingNotify.Message = $"Số Phiếu Nhập {purchase.Id} Chưa Quét.";
                        }
                    }
                    else
                    {
                        var existingNotify = notifies.FirstOrDefault(n => n.IdPurchaseOrder == idPurchase);
                        if (existingNotify != null)
                        {
                            db.PurchaseNotifies.Remove(existingNotify);
                            notifies.Remove(existingNotify);
                        }
                    }
                }
                db.SaveChanges();

                var numNotify = db.PurchaseOrders.Count(x => x.Status == false);

                return Json(new { code = 200, numNotify }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaleOrderNotify()
        {
            try
            {
                var saleorder = db.SalesOrders.ToList();
                var notifies = db.SaleOrderNotifies.ToList();

                foreach (var saleorders in saleorder)
                {
                    var idsaleorder = saleorders.Id;
                    var status = saleorders.Status;

                    if (status == false)
                    {
                        var existingNotify = notifies.FirstOrDefault(n => n.IdSaleOrder == idsaleorder);

                        if (existingNotify == null)
                        {
                            var newNotify = new SaleOrderNotify
                            {
                                IdSaleOrder = saleorders.Id,
                                Message = $"Số Phiếu Xuất {saleorders.Id} Chưa Quét.",
                                Status = false
                            };

                            notifies.Add(newNotify);
                            db.SaleOrderNotifies.Add(newNotify);
                        }
                        else
                        {
                            existingNotify.Message = $"Số Phiếu Xuất {saleorders.Id} Chưa Quét.";
                        }
                    }
                    else
                    {
                        var existingNotify = notifies.FirstOrDefault(n => n.IdSaleOrder == idsaleorder);
                        if (existingNotify != null)
                        {
                            db.SaleOrderNotifies.Remove(existingNotify);
                            notifies.Remove(existingNotify);
                        }
                    }
                }
                db.SaveChanges();

                var numNotify = db.SalesOrders.Count(x => x.Status == false);

                return Json(new { code = 200, numNotify }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetRecentPage(string PageURL, string titlePage)
        {
            try
            {
                var user = (Models.User)Session["user"];
                var username = user.User1;

                if (!string.IsNullOrWhiteSpace(PageURL))
                {
                    var recentpageExit = db.RecentPages.FirstOrDefault(x => x.UserName == username && x.PageURL == PageURL);

                    if (recentpageExit != null)
                    {
                        recentpageExit.AccessedAt = DateTime.Now;
                    }
                    else
                    {
                        var recentPage = new RecentPage
                        {
                            PageURL = PageURL,
                            UserName = username,
                            Title = titlePage,
                            AccessedAt = DateTime.Now,
                            Status = true,
                        };

                        var userRecentPages = db.RecentPages
                                                .Where(rp => rp.UserName == username)
                                                .OrderByDescending(rp => rp.AccessedAt)
                                                .ToList();

                        if (userRecentPages.Count() >= 3)
                        {
                            db.RecentPages.Remove(userRecentPages.Last());
                        }

                        db.RecentPages.Add(recentPage);
                    }

                    db.SaveChanges();
                }

                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RecentPageShow()
        {
            try
            {
                var user = (Models.User)Session["user"];
                var username = user.User1;

                var recentpage = db.RecentPages.Where(rp => rp.UserName == username).OrderByDescending(rp => rp.AccessedAt).ToList();
                
                return Json(new { code = 200, recentpage },JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        //trả về chuỗi ngôn ngữ được lấy từ key resources
        [HttpGet]
        public JsonResult GetResources(string key)
        {
            var resourceValue = Resources.Resource.ResourceManager.GetString(key);
            return Json(resourceValue, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public JsonResult Chat()
        //{
        //    try
        //    {

        //        var user = (User)Session["user"];
        //        var chat = (from c in db.ChatMes
        //                    where c.IdUserReceipt == user.Id
        //                    join uRe in db.Users on c.IdUserReceipt equals uRe.Id
        //                    join uSe in db.Users on c.IdUserSend equals uSe.Id
        //                    select new
        //                    {
        //                        c.Id,
        //                        UserReceipt = uRe.Name,
        //                        UserSend = uSe.Name,
        //                        IdUserSend = uSe.Id,
        //                        c.CreateDate,
        //                        c.Status,
        //                        c.Text
        //                    }).ToList();
        //        var numNotYetNotice = chat.Where(x => x.Status == false).Count();
        //        var listChat = chat
        //              .GroupBy(x => x.UserSend)
        //              .Select(group => new
        //              {
        //                  UserSend = group.Key,
        //                  LatestMessage = group.OrderByDescending(x => x.CreateDate).FirstOrDefault(),
        //                  Count = group.Count(message => message.Status == false)
        //              })
        //              .ToList();
        //        return Json(new { code = 200, numNotYetNotice, chat, listChat , /*IdSendNew*/ }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpGet]
        //public JsonResult DetailChat(int pageNumber, string id)
        //{
        //    try
        //    {
        //        var statusChat = db.ChatMes.Where(x => x.IdUserSend == id).ToList();
        //        foreach(var s in statusChat)
        //        {
        //            s.Status = true;
        //        };
        //        db.SaveChanges();
        //        int pageSize = 5;
        //        var user = (User)Session["user"];
        //        var chat = (from c in db.ChatMes
        //                    where (c.IdUserReceipt == user.Id&&c.IdUserSend==id)|| (c.IdUserReceipt == id && c.IdUserSend == user.Id)
        //                    join uRe in db.Users on c.IdUserReceipt equals uRe.Id
        //                    join uSe in db.Users on c.IdUserSend equals uSe.Id
        //                    orderby c.CreateDate descending
        //                    select new
        //                    {
        //                        c.Id,
        //                        UserReceipt = uRe.Name,
        //                        UserSend = uSe.Name,
        //                        IdUserSend = uSe.Id,
        //                        statusSend = uSe.Status==true?"hoạt Động":"Không Hoạt Động",
        //                        c.CreateDate,
        //                        c.Status,
        //                        c.Text
        //                    }).ToList();
        //        var pages = chat.Count() % pageSize == 0 ? chat.Count() / pageSize : chat.Count() / pageSize + 1;
        //        var chatNew = chat.FirstOrDefault();
        //        chat = chat.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        return Json(new { code = 200, chat, pages , chatNew }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //} 
        //[HttpPost]
        //public JsonResult Send(string text,string id)
        //{
        //    try
        //    {
        //        var user = (User)Session["user"];
        //        if (!string.IsNullOrWhiteSpace(text))
        //        {
        //            ChatMe chat = new ChatMe() { 
        //                Text = text,
        //                IdUserReceipt = id,
        //                IdUserSend = user.Id,
        //                Status = false,
        //                CreateDate = DateTime.Now

        //            };
        //            db.ChatMes.Add(chat);
        //            db.SaveChanges();
        //        }
        //        return Json(new { code = 200,  }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpPost]
        //public JsonResult ChangeSession()
        //{
        //    try
        //    {
        //        var User = (User)Session["user"];
        //        var newUser = db.Users.Find(User.Id);
        //        Session["user"] = newUser;

        //        return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [System.Web.Mvc.HttpGet]
        public JsonResult Page_Load()
        { // Kiểm tra tính còn hiệu lực của phiên làm việc
            try
            {
                if (Session["user"] == null)
                {
                    return Json(new { code = 401, }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg =  "Lỗi " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}