using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class CustomerController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Adds()
        {
            return View();
        }
        public ActionResult Edits(string id)
        {
            Data data = new Data();
            data.InitializeData();
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.membershipCardValue = customer.MembershipCard == null ? "Chọn Thẻ Thành Viên" : data.membershipCard.SingleOrDefault(x => x.Key == customer.MembershipCard).Value;
            ViewBag.membershipCardKey = customer.MembershipCard == null ? "-1" : data.membershipCard.SingleOrDefault(x => x.Key == customer.MembershipCard).Key;
            ViewBag.customerResourcesValue = customer.CustomerResources == null ? "Chọn Nguồn KH" : data.customerResources.SingleOrDefault(x => x.Key == customer.CustomerResources).Value;
            ViewBag.customerResourcesKey = customer.CustomerResources == null ? "-1" : data.customerResources.SingleOrDefault(x => x.Key == customer.CustomerResources).Key;
            ViewBag.careerValue = customer.Career == null ? "Chọn Ngành Nghề" : data.career.SingleOrDefault(x => x.Key == customer.Career).Value;
            ViewBag.careerKey = customer.Career == null ? "-1" : data.career.SingleOrDefault(x => x.Key == customer.Career).Key;
            ViewBag.paymentMethodsValue = customer.PaymentMethods == null ? "Chọn Phương Thức TT" : data.paymentMethods.SingleOrDefault(x => x.Key == customer.PaymentMethods).Value;
            ViewBag.paymentMethodsKey = customer.PaymentMethods == null ? "-1" : data.paymentMethods.SingleOrDefault(x => x.Key == customer.PaymentMethods).Key;
            ViewBag.accountingAccountCodeValue = customer.AccountingAccountCode == null ? "Chọn Mã TK Hạch Toán" : data.accountingAccountCode.SingleOrDefault(x => x.Key == customer.AccountingAccountCode).Value;
            ViewBag.accountingAccountCodeKey = customer.AccountingAccountCode == null ? "-1" : data.accountingAccountCode.SingleOrDefault(x => x.Key == customer.AccountingAccountCode).Key;
            ViewBag.NameUser = customer.IdUser == null ? "Chọn NV Phụ Trách" : db.Users.Find(customer.IdUser).Name;
            ViewBag.IdUser = customer.IdUser == null ? "-1" : db.Users.Find(customer.IdUser).Id;
            return View(customer);
        }

        public ActionResult Details(string id)
        {
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.Customers
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                             des = b.Description,
                             createDate = b.CreateDate,
                             createBy = b.CreateBy,
                             modifyDate = b.ModifyDate,
                             modifyBy = b.ModifyBy
                         }).ToList().Where(x => x.id.ToLower().Contains(seach) || x.name.ToLower().Contains(seach));
                var pages = a.Count() % pageSize == 0 ? a.Count() / pageSize : a.Count() / pageSize + 1;
                var c = a.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = a.Count();
                return Json(new { code = 200, c = c, pages = pages, count = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Detail(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var customer = (from cus in db.Customers
                                    where cus.Id == id
                                    select new
                                    {
                                        cus.Name,
                                        cus.AddRess
                                    }).ToList().LastOrDefault();
                    return Json(new { code = 200, customer }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("Khách Hàng Không Tồn Tại").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Data()
        {
            try
            {
                Data data = new Data();
                data.InitializeData();
                return Json(new
                {
                    code = 200,
                    membershipCard = data.membershipCard,
                    customerResources = data.customerResources,
                    career = data.career,
                    paymentMethods = data.paymentMethods,
                    accountingAccountCode = data.accountingAccountCode
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Add()
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                var date = DateTime.Now;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Request.Form["name"]);
                string base64String = Convert.ToBase64String(bytes);
                var a = Request.Form["radios18"];
                Customer customer = new Customer();
                customer.TaxCode = Request.Form["taxcode"];
                customer.Id = base64String + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
                customer.Name = Request.Form["name"];
                customer.AddRess = Request.Form["address"];
                customer.Website = Request.Form["website"];
                customer.IdTypeCustomer = Request.Form["typeCustomer"] == "-1" ? null : Request.Form["typeCustomer"];
                customer.Fax = Request.Form["fax"];
                customer.Phone = Request.Form["phone"];
                customer.Email = Request.Form["email"];
                customer.Description = Request.Form["des"];
                customer.IdUser = Request.Form["idUser"] == "-1" ? null : Request.Form["idUser"];
                customer.MembershipCard = Request.Form["membershipCard"] == "-1" ? null : Request.Form["membershipCard"];
                customer.CustomerResources = Request.Form["customerResources"] == "-1" ? null : Request.Form["customerResources"];
                customer.Career = Request.Form["career"] == "-1" ? null : Request.Form["career"];
                customer.DebtLimit = float.Parse(Request.Form["debtLimit"]);
                customer.NumberDaysOwed = float.Parse(Request.Form["numberDaysOwed"]);
                customer.PaymentMethods = Request.Form["paymentMethods"] == "-1" ? null : Request.Form["paymentMethods"];
                customer.AccountingAccountCode = Request.Form["accountingAccountCode"] == "-1" ? null : Request.Form["accountingAccountCode"];
                customer.STK1 = Request.Form["stk1"];
                customer.Bank1 = Request.Form["infoStk1"];
                customer.STK2 = Request.Form["stk2"];
                customer.Bank2 = Request.Form["infoStk2"];
                customer.Status = Request.Form["status"] == "on" || Request.Form["status"] == "true" ? true : false;
                customer.CreateDate = DateTime.Now;
                customer.ModifyDate = DateTime.Now;
                customer.CreateBy = user.FullName;
                customer.ModifyBy = user.FullName;
                customer.Type = a == "1" ? "KHDN" : "KHCN";
                db.Customers.Add(customer);
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("CreateSucess") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Edit()
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                var a = Request.Form["radios18"];
                var id = Request.Form["id"];
                var customer = db.Customers.Find(id);
                customer.TaxCode = Request.Form["taxcode"];
                customer.Name = Request.Form["name"];
                customer.AddRess = Request.Form["address"];
                customer.Website = Request.Form["website"];
                customer.IdTypeCustomer = Request.Form["typeCustomer"] == "-1" ? null : Request.Form["typeCustomer"];
                customer.Fax = Request.Form["fax"];
                customer.Phone = Request.Form["phone"];
                customer.Email = Request.Form["email"];
                customer.Description = Request.Form["des"];
                customer.IdUser = Request.Form["idUser"];
                customer.MembershipCard = Request.Form["membershipCard"] == "-1" ? null : Request.Form["membershipCard"];
                customer.CustomerResources = Request.Form["customerResources"] == "-1" ? null : Request.Form["customerResources"];
                customer.Career = Request.Form["career"] == "-1" ? null : Request.Form["career"];
                customer.DebtLimit = float.Parse(Request.Form["debtLimit"]);
                customer.NumberDaysOwed = float.Parse(Request.Form["numberDaysOwed"]);
                customer.PaymentMethods = Request.Form["paymentMethods"] == "-1" ? null : Request.Form["paymentMethods"];
                customer.AccountingAccountCode = Request.Form["accountingAccountCode"] == "-1" ? null : Request.Form["accountingAccountCode"];
                customer.STK1 = Request.Form["stk1"];
                customer.Bank1 = Request.Form["infoStk1"];
                customer.STK2 = Request.Form["stk2"];
                customer.Bank2 = Request.Form["infoStk2"];
                customer.Status = Request.Form["status"] == "on" || Request.Form["status"] == "true" ? true : false;
                customer.CreateDate = DateTime.Now;
                customer.ModifyDate = DateTime.Now;
                customer.CreateBy = user.FullName;
                customer.ModifyBy = user.FullName;
                customer.Type = a == "1" ? "KHDN" : "KHCN";
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("SucessEdit") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                var customer = db.Customers.Find(id);
                db.Customers.Remove(customer);
                db.SaveChanges();
                return Json(new { code = 200, msg =rm.GetString("SucessDelete"), }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GroupGoods()
        {
            try
            {
                var c = (from b in db.GroupGoods
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
