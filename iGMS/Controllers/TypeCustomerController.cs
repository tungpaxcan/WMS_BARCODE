using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class TypeCustomerController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: TypeCustomer
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
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeCustomer typeCustomer = db.TypeCustomers.Find(id);
            if (typeCustomer == null)
            {
                return HttpNotFound();
            }
            return View(typeCustomer);
        }
        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.TypeCustomers.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                             des = b.Des,
                             createDate = b.CreateDate,
                             createBy = b.CreateBy,
                             modifyDate = b.ModifyDate,
                             modifyBy = b.ModifyBy
                         }).ToList().Where(x => x.id.ToLower().Contains(seach) || x.name.ToLower().Contains(seach));
                var pages = a.Count() % pageSize == 0 ? a.Count() / pageSize : a.Count() / pageSize + 1;
                var c = a.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = a.Count();
                return Json(new { code = 200, c = c, pages = pages, count = count,}, JsonRequestBehavior.AllowGet);
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
                TypeCustomer typeCustomer = new TypeCustomer()
                {
                    Id = Request.Form["Id"],
                    Name = Request.Form["Name"],
                    Des = Request.Form["Des"],
                    CreateDate = DateTime.Now,
                    CreateBy = user.FullName,
                    ModifyDate = DateTime.Now,
                    ModifyBy = user.FullName,
                    Status = true
                };
                db.TypeCustomers.Add(typeCustomer);
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
                var typeCustomer = db.TypeCustomers.Find(Request.Form["Id"]);
                typeCustomer.Name = Request.Form["Name"];
                typeCustomer.Des = Request.Form["Des"];
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("SucessEdit") }, JsonRequestBehavior.AllowGet);        
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete()
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                string id = Request.Form["Id"];
                var typeCustomer = db.TypeCustomers.Find(id);
                var customer = db.Customers.Where(x => x.IdTypeCustomer == id).ToList();
                db.Customers.RemoveRange(customer);
                db.TypeCustomers.Remove(typeCustomer);
                bool saveFailed;
                do
                {
                    saveFailed = false;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;

                        // Update the values of the entity that failed to save from the store
                        ex.Entries.Single().Reload();
                    }

                } while (saveFailed);
                return Json(new { code = 200, msg = rm.GetString("SucessDelete") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}