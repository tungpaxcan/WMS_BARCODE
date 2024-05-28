using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Resources;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Windows.Controls.Primitives;
using WebGrease.Css.Ast;
using WMS.Models;

namespace WMS.Controllers
{
    public class UnitController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Adds()
        {
            ViewBag.FunctionId = "/Unit/Adds";
            return View();
        }
        public ActionResult Edits(string id)
        {
            if (id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }
        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.Units.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                         }).ToList().Where(x => x.name.ToLower().Contains(seach));
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
        [HttpPost]
        public JsonResult Add(string name)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { code = 500, msg = rm.GetString("nhập tên đơn vị").ToString() }, JsonRequestBehavior.AllowGet);
                }
                var date = DateTime.Now;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(name);
                string base64String = Convert.ToBase64String(bytes);
                var id = base64String + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
                id = id.Substring(id.Length - 16);
                var ids = db.Units.Where(x => x.Id == id).ToList();
                var checkName = db.Units.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                
                if (checkName != null)
                {
                    return Json(new { code = 500, msg = rm.GetString("trùng tên đơn vị").ToString() }, JsonRequestBehavior.AllowGet);
                }
                
                if (ids.Count == 0)
                {
                    var d = new Unit()
                    {
                        Id = id.Replace("=", ""),
                        Name = name,
                    };
                    db.Units.Add(d);
                    db.SaveChanges();
                    return Json(new { code = 200, msg = rm.GetString("CreateSucess") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 300, msg = rm.GetString("Duplicate") }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg =rm.GetString("false")+ e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Edit(string id,string name)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                var d = db.Units.Find(id);
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { code = 500, msg = rm.GetString("nhập tên đơn vị").ToString() }, JsonRequestBehavior.AllowGet);
                }
                d.Name = name;
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("SucessEdit") }, JsonRequestBehavior.AllowGet);   
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("FailEdit") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var d = db.Units.Find(id);
                db.Units.Remove(d);
                db.SaveChanges();
                return Json(new { code = 200, msg =rm.GetString("SucessDelete") }, JsonRequestBehavior.AllowGet);   
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("FailDelete"), e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //public JsonResult Upload(FormCollection formCollection)
        //{
        //    List<string> logError = new List<string>();
        //    try
        //    {
        //        var session = (ApiAccount)Session["user"];
        //        var nameAdmin = session.UserName;

        //        if (Request != null)
        //        {
        //            HttpPostedFileBase file = Request.Files["UploadedFile"];
        //            if(file.ContentLength == 0)
        //            {
        //                return Json(new { status = 500, msg = "Vui Lòng Chọn File" }, JsonRequestBehavior.AllowGet);
        //            }
        //            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //            {
        //                string fileName = file.FileName;
        //                string fileContentType = file.ContentType;
        //                byte[] fileBytes = new byte[file.ContentLength];
        //                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
        //                using (var package = new ExcelPackage(file.InputStream))
        //                {
        //                    ExcelWorksheet currentSheet = package.Workbook.Worksheets.First();
        //                    var workSheet = currentSheet;
        //                    var noOfCol = workSheet.Dimension.End.Column;
        //                    var noOfRow = workSheet.Dimension.End.Row;
        //                    List<Unit> unitList = new List<Unit>();
        //                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //                    {
        //                        var NameUnit = workSheet.Cells[rowIterator, 1].Value == null ? "" : workSheet.Cells[rowIterator, 1].Value.ToString();
        //                        var date = DateTime.Now;
        //                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(NameUnit);
        //                        string base64String = Convert.ToBase64String(bytes);
        //                        var id = base64String + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
        //                        var ids = db.Units.Where(x => x.Id == id).ToList();
        //                        var checkNameExcel = unitList.FirstOrDefault(x => x.Name.ToLower() == NameUnit.ToLower());
        //                        var checkNameUnit = db.Units.FirstOrDefault(x => x.Name.ToLower() == NameUnit.ToLower());
        //                        if(checkNameUnit != null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Tên Đơn Vị Ở Dòng {rowIterator} Đã Có Trong Hệ Thống" }, JsonRequestBehavior.AllowGet);
        //                        }

        //                        if (string.IsNullOrEmpty(NameUnit))
        //                        {
        //                            return Json(new { status = 500, msg = $"Tên Đơn Vị Ở Dòng {rowIterator} Không Được Bỏ Trống" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkNameExcel != null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Tên Đơn Vị Ở Dòng {rowIterator} Bị Trùng Với Dòng {checkNameExcel.Line} " }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (ids.Count == 0)
        //                        {
        //                            var d = new Unit()
        //                            {
        //                                Id = id.Replace("=",""),
        //                                Name = NameUnit,
        //                            };
        //                            unitList.Add(d);
        //                        }
        //                        else
        //                        {
        //                            return Json(new { status = 500, msg = $"Trùng Mã Đơn Vị" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    foreach (var item in unitList)
        //                    {

        //                        var name = item.Name;
        //                        var reponse = Add(name);
        //                    }
        //                }
        //            }
        //        }
        //        return Json(new { status = 200, logError, msg = "Thành công " }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { status = 500, logError, msg = "Thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}