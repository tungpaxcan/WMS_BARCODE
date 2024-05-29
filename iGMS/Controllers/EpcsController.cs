using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class EpcsController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Epcs
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Data()
        {
            return View();
        }
        [HttpGet]
        public JsonResult CheckIdGoodsInSystem(string upc,string warehouse)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var session = (User)Session["user"];
                var checkEpc = (from e in db.DetailWareHouses where e.IdGoods == upc && e.IdWareHouse == warehouse
                                join w in db.WareHouses on e.IdWareHouse equals w.Id
                                select new
                                {
                                    e.IdGoods,
                                    Name = (w == null ? null : w.Name),
                                    e.Status
                                }).FirstOrDefault();
                if (checkEpc != null)
                {
                    return Json(new { code = 200,checkEpc,status=false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 200, status = true }, JsonRequestBehavior.AllowGet);
                }
               
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult Epc(string epc)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var epcs = JsonConvert.DeserializeObject<List<EPC>>(epc);
                foreach(var item in epcs)
                {
                    item.Status = false;
                }
                db.EPCs.AddRange(epcs);
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("Đã nhận được EPC Thành Công").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("getEPCerror").ToString() + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult LocationEpc(string epc)
        {
            try
            {
                string filePath = Server.MapPath("~/localtion.txt"); // Đường dẫn tới tệp tin
                System.IO.File.WriteAllText(filePath, epc);

                // Đọc nội dung của tệp tin
                string fileContent = System.IO.File.ReadAllText(filePath);



                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("locationEpc");
                return Json(new { code = 200, msg = rm.GetString("Đã nhận được EPC Thành Công").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("getEPCerror").ToString() + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetLocationEpc()
        {
            try
            {
                string filePath = Server.MapPath("~/localtion.txt"); // Đường dẫn tới tệp tin
                string epc = System.IO.File.ReadAllText(filePath);

                return Json(new { code = 200, msg = rm.GetString("Đã nhận được EPC Thành Công").ToString() + " !!!" , epc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("getEPCerror").ToString() + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string currentDirectory = HostingEnvironment.MapPath("~");
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet currentSheet = package.Workbook.Worksheets.First();
                        var workSheet = currentSheet;
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        ImportError[] importError = new ImportError[noOfRow+2];
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            try
                            {
                                var id = workSheet.Cells[rowIterator, 1].Value == null ? null : workSheet.Cells[rowIterator, 1].Value.ToString();
                                var epc = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                                if (id == null)
                                {
                                    importError[rowIterator] = new ImportError(rm.GetString("Chưa Có Mã Hàng").ToString(), rowIterator);
                                    continue;
                                }
                                else
                                {
                                    var checkId = db.Goods.FirstOrDefault(x => x.Id == id);
                                    if(checkId == null)
                                    {
                                        importError[rowIterator] = new ImportError(rm.GetString("Chưa Có Mã Hàng").ToString(), rowIterator);
                                        continue;
                                    }
                                }
                                if (epc == null)
                                {
                                    importError[rowIterator] = new ImportError(rm.GetString("Chưa Có EPC").ToString(), rowIterator);
                                    continue;
                                }
                                var checkEpc = db.EPCs.SingleOrDefault(x => x.IdEPC == epc);
                                if (checkEpc == null)
                                {
                                    var user = (ApiAccount)Session["user"];
                                    var ePC = new EPC();
                                    ePC.IdGoods = id;
                                    ePC.IdEPC = epc;
                                    ePC.Status = false;
                                    db.EPCs.Add(ePC);
                                }
                                else
                                {
                                    importError[rowIterator] = new ImportError(rm.GetString("Đã Tồn Tại, Có Mã EPC Là").ToString()+" "+ epc, rowIterator);
                                    continue;
                                }

                            }
                            catch (DbEntityValidationException ex)
                            {
                                foreach (var error in ex.EntityValidationErrors)
                                {
                                    foreach (var validationError in error.ValidationErrors)
                                    {
                                        Console.WriteLine("Lỗi xác thực: {0}", validationError.ErrorMessage);
                                    }
                                }
                            }
                        }
                      
                        importError = importError.Where(enem => enem != null).ToArray();
                        var listError = importError.Select(i => i.ToString()).ToList();
                        if(listError.Count > 0)
                        {
                            return Json(new { code = 500, msg = listError }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            db.SaveChanges();
                            return Json(new { code = 200, msg = rm.GetString("Thành Công").ToString() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new { code = 500, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}