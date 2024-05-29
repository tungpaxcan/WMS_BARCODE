using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Windows;
using WMS.Models;
using OfficeOpenXml;
using System.Text;
using WebGrease.Activities;
using Microsoft.SqlServer.Management.XEvent;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using System.Runtime.InteropServices.ComTypes;
using System.Resources;

namespace WMS.Controllers
{
    public class GoodsController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Goods
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Adds()
        {
            return View();
        }
        public ActionResult DetailList()
        {
            return View();
        }
        public ActionResult SearchEPC()
        {
            return View();

        }
        public ActionResult Edits(string id)
        {
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUnit = good.IdUnit == null ? "-1" : good.IdUnit;
            ViewBag.nameUnit = good.IdUnit == null ? rm.GetString("Thuộc Đơn Vị").ToString() : good.Unit.Name;
            ViewBag.idGroupGoods = good.IdGroupGood == null ? "-1" : good.IdGroupGood;
            ViewBag.nameGroupGoods = good.IdGroupGood == null ? rm.GetString("Thuộc Nhóm Hàng").ToString() : good.GroupGood.Name;
            return View(good);
        }
        public ActionResult Details(string id)
        {
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }
            return View(good);
        }
        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.Goods.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             gd = b.GroupGood.Name == null ? "" : b.GroupGood.Name,
                             date = b.CreateDate.Value.Day + "/" + b.CreateDate.Value.Month + "/" + b.CreateDate.Value.Year,
                             inventory = b.Inventory,
                             minimum = b.MinimumInventory,
                             maximum = b.MaximumInventory,
                             name = b.Name,
                             unit = b.Unit.Name == null ? "" : b.Unit.Name,

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
        [HttpPost]
        public JsonResult Add(Good good, string idDetailWarehouse)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var session = (User)Session["user"];
                var nameAdmin = session.Name;

                if (string.IsNullOrEmpty(good.Name))
                {
                    return Json(new { status = 500, msg = rm.GetString("nhập tên hàng".ToString()) }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(good.Id))
                {
                    return Json(new { status = 500, msg = rm.GetString("nhập mã hàng").ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var checkGoods = db.Goods.Find(good.Id);
                    if (checkGoods == null)
                    {
                        if (good.IdGroupGood == "-1")
                        {
                            good.IdGroupGood = null;
                        }
                        if (good.IdUnit == "-1")
                        {
                            good.IdUnit = null;
                        }
                        good.CreateDate = DateTime.Now;
                        good.ModifyDate = DateTime.Now;
                        good.CreateBy = session.Name;
                        good.ModifyBy = session.Name;

                        db.Goods.Add(good);
                        // lưu thông tin vào detail warehouse
                    }
                    else
                    {
                        checkGoods.Inventory += good.Inventory;
                    }
                    var warehouse = db.WareHouses.Find(idDetailWarehouse);
                    if (warehouse != null)
                    {
                        if(good.Inventory > warehouse.MaxInventory)
                        {
                            return Json(new { status = 500, msg = $"{rm.GetString("NhapKhoQuaSL")} {warehouse.Name}" }, JsonRequestBehavior.AllowGet);
                        }
                        var detailWarehouse = db.DetailWareHouses.SingleOrDefault(x => x.IdWareHouse == idDetailWarehouse && x.IdGoods == good.Id);
                        if (detailWarehouse != null)
                        {
                            detailWarehouse.Inventory += good.Inventory;
                        }
                        else
                        {
                            DetailWareHouse newDetailWareHouse = new DetailWareHouse()
                            {
                                IdWareHouse = idDetailWarehouse,
                                IdGoods = good.Id,
                                Inventory = good.Inventory,
                                Status = true,
                            };
                            db.DetailWareHouses.Add(newDetailWareHouse);
                        }
                    }
                    else
                    {
                        return Json(new { status = 500, msg = $"Kho Không Tồn Tại Trong Hệ Thống" }, JsonRequestBehavior.AllowGet);
                    }
                    db.SaveChangesAsync();
                    return Json(new { status = 200, msg = rm.GetString("CreateSucess") }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = 500, msg = rm.GetString("FailCreate") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult showtbd(string id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var session = (User)Session["user"];
                var editG = db.Goods.Find(id);
                var detailTbd = db.DetailWareHouses.FirstOrDefault(x => x.IdGoods == id);
                var e = (from b in db.Goods
                         join d in db.DetailWareHouses on b.Id equals d.IdGoods
                         where b.Id == id
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                             wareHouse = d.WareHouse.Name,
                             d.IdWareHouse,
                             inventory = d.Inventory,
                         }).ToList();

                return Json(new { code = 200, e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult Edit(Good good,List<EPC> filteredData)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var session = (User)Session["user"];
                var editG = db.Goods.Find(good.Id);
                if (editG.IdGroupGood == "-1")
                {
                    editG.IdGroupGood = null;
                }
                else
                {
                    editG.IdGroupGood = good.IdGroupGood;
                }
                if (editG.IdUnit == "-1")
                {
                    editG.IdUnit = null;
                }
                else
                {
                    editG.IdUnit = good.IdUnit;
                }
                editG.Id = good.Id;
                editG.Name = good.Name;
                editG.Description = good.Description;
                editG.Inventory = good.Inventory;
                editG.ModifyDate = DateTime.Now;
                editG.ModifyBy = session.Name;
                if(filteredData?.Count > 0 || filteredData!=null)
                {
                    var listWarehouse = filteredData.GroupBy(x => x.IdWareHouse)?.Select(x => x.Key).ToList();
                    foreach (var item in listWarehouse)
                    {
                        var maxInventory = db.WareHouses.Find(item).MaxInventory;
                        var nameWareHouse = db.WareHouses.Find(item).Name;
                        var detailWH = db.DetailWareHouses?.FirstOrDefault(x => x.IdWareHouse == item && x.IdGoods == good.Id);
                        var inventoryCurrent = detailWH?.Inventory;
                        var inventoryForWarehouse = filteredData.Count(x => x.IdWareHouse == item) + inventoryCurrent;

                        if (inventoryForWarehouse == null || inventoryCurrent == null)
                        {
                            inventoryForWarehouse = 0;
                        }
                        if (inventoryForWarehouse > maxInventory)
                        {
                            return Json(new { status = 500, msg = $"{rm.GetString("NhapKhoQuaSL")} {nameWareHouse}" }, JsonRequestBehavior.AllowGet);
                        }
                        detailWH.Inventory = inventoryForWarehouse;
                    }
                    foreach (var item in filteredData)
                    {
                        item.IdGoods = good.Id;
                        item.Status = true;
                    }
                    db.EPCs.AddRange(filteredData);
                }
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
            db.Configuration.ProxyCreationEnabled = false;
            var session = (User)Session["user"];
            try
            {
                var detailGoodOrder = db.DetailGoodOrders.Where(x => x.IdGoods == id).ToList();
                db.DetailGoodOrders.RemoveRange(detailGoodOrder);
                var detailSalesOrder = db.DetailSaleOrders.Where(x => x.IdGoods == id).ToList();
                db.DetailSaleOrders.RemoveRange(detailSalesOrder);
                var detailStock = db.DetailStocks.Where(x => x.IdGoods == id).ToList();
                db.DetailStocks.RemoveRange(detailStock);
                db.SaveChanges();
                var epcs = db.EPCs.Where(x => x.IdGoods == id);
                db.EPCs.RemoveRange(epcs);
                var detailWarehouses = db.DetailWareHouses.Where(x => x.IdGoods == id);
                db.DetailWareHouses.RemoveRange(detailWarehouses);
                var d = db.Goods.Find(id);
                db.Goods.Remove(d);
                db.SaveChanges();

                return Json(new { code = 200, msg = rm.GetString("SucessDelete") }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { code = 500, msg = rm.GetString("FailDelete") }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detail(string id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (!string.IsNullOrEmpty(id))
                {
                    var goods = (from g in db.Goods
                                 where g.Id == id
                                 select new
                                 {
                                     g.Id,
                                     g.Name,
                                     g.Description,
                                     g.IdUnit,
                                     g.IdGroupGood,
                                     nameGroupGoods = g.GroupGood.Name == null ? "" : g.GroupGood.Name,
                                     nameUnit = g.Unit.Name == null ? "" : g.Unit.Name,
                                 }).ToList().LastOrDefault();

                    if (goods == null)
                    {
                        return Json(new { code = 500, msg = rm.GetString("mã hàng không tồn tại").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        return Json(new { code = 200, goods }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("mã hàng không tồn tại").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = 500, msg = rm.GetString("error") }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GroupGoods()
        {
            try
            {
                var c = (from b in db.GroupGoods.Where(x => x.Id.Length > 0)
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

        [HttpGet]
        public JsonResult WareHouse()
        {
            try
            {
                var c = (from b in db.WareHouses.Where(x => x.Id.Length > 0)
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
        //[HttpPost]
        //public JsonResult Upload()
        //{
        //    try
        //    {
        //        var session = (User)Session["user"];
        //        var nameAdmin = session.UserName;
        //        if (Request != null)
        //        {
        //            HttpPostedFileBase file = Request.Files["UploadedFile"];
        //            if (file.ContentLength == 0)
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
        //                    List<Good> goodList = new List<Good>();
        //                    List<DetailWareHouse> detailWareHouseList = new List<DetailWareHouse>();
        //                    List<EPC> epcList = new List<EPC>();
        //                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //                    {
        //                        var idEPCs = workSheet.Cells[rowIterator, 6]?.Value?.ToString();
        //                        var idGoods = workSheet.Cells[rowIterator, 1]?.Value?.ToString();
        //                        var Name = workSheet.Cells[rowIterator, 2].Value?.ToString();
        //                        var Unit = workSheet.Cells[rowIterator, 3]?.Value == null ? "-1" : workSheet.Cells[rowIterator, 3].Value.ToString();
        //                        var grGoods = workSheet.Cells[rowIterator, 4]?.Value == null ? "-1" : workSheet.Cells[rowIterator, 4].Value.ToString();
        //                        var wareHouse = workSheet.Cells[rowIterator, 5].Value.ToString();
        //                        var checkId = goodList.SingleOrDefault(x => x.Id == idGoods);
        //                        var checkE = db.EPCs.FirstOrDefault(x => x.IdEPC == idEPCs);
        //                        var checkUnit = db.Units.FirstOrDefault(x => x.Id.ToLower() == Unit.ToLower());
        //                        var checkGrGoods = db.GroupGoods.FirstOrDefault(x => x.Id.ToLower() == grGoods.ToLower());
        //                        var checkWareHouse = db.WareHouses.FirstOrDefault(x => x.Id.ToLower() == wareHouse.ToLower());
        //                        if (wareHouse == null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã Kho Tại Dòng {rowIterator} Không Có Giá Trị" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkUnit == null && Unit != "-1")
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã Đơn Vị Tại Dòng {rowIterator} Không Có Trong Hệ Thống" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkGrGoods == null && grGoods != "-1")
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã Nhóm Hàng Hóa Tại Dòng {rowIterator} Không Có Trong Hệ Thống" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkWareHouse == null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã Kho Tại Dòng {rowIterator} Không Có Trong Hệ Thống" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkE != null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã EPC Ở Dòng {rowIterator} Đã Có Trong Hệ Thống Thuộc Kho {checkE.WareHouse.Name} (Mã Hàng {checkE.Good.Name})" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkId == null)
        //                        {
        //                            Good good = new Good()
        //                            {
        //                                Id = idGoods,
        //                                Name = Name,
        //                                IdUnit = Unit,
        //                                IdGroupGood = grGoods,
        //                                CreateDate = DateTime.Now,
        //                                ModifyDate = DateTime.Now,
        //                                CreateBy = session.FullName,
        //                                ModifyBy = session.FullName,
        //                                Line = rowIterator,
        //                                Inventory = 1,
        //                            };
        //                            goodList.Add(good);
        //                        }
        //                        else
        //                        {
        //                            checkId.Inventory++;
        //                            if (checkId.IdUnit != Unit)
        //                            {
        //                                return Json(new { status = 500, msg = $"Tên Đơn Vị Tại Dòng {rowIterator} Khác Với Dòng {checkId.Line}" }, JsonRequestBehavior.AllowGet);
        //                            }
        //                            if (checkId.IdGroupGood != grGoods)
        //                            {
        //                                return Json(new { status = 500, msg = $"Nhóm Hàng Hóa Tại Dòng {rowIterator} Khác Với Dòng {checkId.Line}" }, JsonRequestBehavior.AllowGet);
        //                            }
        //                            if (checkId.Name != Name)
        //                            {
        //                                return Json(new { status = 500, msg = $"Tên Hàng Hóa Tại Dòng {rowIterator} Khác Với Dòng {checkId.Line}" }, JsonRequestBehavior.AllowGet);
        //                            }
        //                        }
        //                        var checkDtWareHouse = detailWareHouseList.FirstOrDefault(x => x.IdWareHouse == checkWareHouse.Id && x.IdGoods == idGoods);
        //                        if (checkDtWareHouse != null)
        //                        {
        //                            checkDtWareHouse.Inventory++;
        //                        }
        //                        else
        //                        {
        //                            DetailWareHouse detailWareHouse = new DetailWareHouse()
        //                            {
        //                                IdWareHouse = checkWareHouse.Id,
        //                                IdGoods = idGoods,
        //                                Inventory = 1,
        //                                Status = true
        //                            };
        //                            detailWareHouseList.Add(detailWareHouse);
        //                        }
        //                        var checkEpc = epcList.FirstOrDefault(x => x.IdEPC == idEPCs);
        //                        if (checkEpc == null)
        //                        {
        //                            EPC epc = new EPC()
        //                            {
        //                                IdEPC = idEPCs,
        //                                IdGoods = idGoods,
        //                                IdWareHouse = checkWareHouse.Id,
        //                                Line = rowIterator,
        //                                Status = true
        //                            };
        //                            epcList.Add(epc);
        //                        }
        //                        else
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã EPC {idEPCs} Tại Dòng {rowIterator} Trùng Với Dòng {checkEpc.Line} " }, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    foreach (var item in goodList)
        //                    {                               
        //                        var arrayDetailWarehouse = detailWareHouseList.Where(x => x.IdGoods == item.Id).ToList();
        //                        if(arrayDetailWarehouse.Count < 1)
        //                        {
        //                            return Json(new { status = 500, msg = $"Mã Hàng {item.Name} Tại Dòng {item.Line} Không Có Mã Kho Khớp " }, JsonRequestBehavior.AllowGet);
        //                        }else if(arrayDetailWarehouse.Count == 1)
        //                        {
        //                            var idDetailWH = arrayDetailWarehouse.FirstOrDefault().IdWareHouse;
        //                            var arrayepc = JsonConvert.SerializeObject(epcList.Where(x => x.IdGoods == item.Id && x.IdWareHouse == idDetailWH).Select(x => x.IdEPC).ToArray());
        //                            var reponse = Add(item, idDetailWH, arrayepc);
        //                            var stringReponse = JsonConvert.SerializeObject(reponse.Data);
        //                            var jsonReponse = JsonConvert.DeserializeObject<reponses>(stringReponse);
        //                            if (jsonReponse.status == 500)
        //                            {
        //                                return Json(new { status = 500, msg = "Lỗi: " + jsonReponse.msg }, JsonRequestBehavior.AllowGet);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var detailWH = arrayDetailWarehouse.FirstOrDefault();
        //                            var idDetailWH = detailWH.IdWareHouse;
        //                            var arrayepc = JsonConvert.SerializeObject(epcList.Where(x => x.IdGoods == item.Id && x.IdWareHouse == idDetailWH).Select(x => x.IdEPC).ToArray());
        //                            var reponse = Add(item, idDetailWH, arrayepc);
        //                            var stringReponse = JsonConvert.SerializeObject(reponse.Data);
        //                            var jsonReponse = JsonConvert.DeserializeObject<reponses>(stringReponse);
        //                            if (jsonReponse.status == 500)
        //                            {
        //                                return Json(new { status = 500, msg = "Lỗi: " + jsonReponse.msg }, JsonRequestBehavior.AllowGet);
        //                            }
        //                            arrayDetailWarehouse.Remove(detailWH);//xóa phần tử đầu đã add của chi tiết kho
        //                            foreach (var w in arrayDetailWarehouse)
        //                            {
        //                                var filteredData = epcList.Where(x => x.IdGoods == item.Id && x.IdWareHouse == w.IdWareHouse).ToList();
        //                                var reponseE = Edit(item, filteredData);
        //                                var stringReponseE = JsonConvert.SerializeObject(reponseE.Data);
        //                                var jsonReponseE = JsonConvert.DeserializeObject<reponses>(stringReponseE);
        //                                if (jsonReponseE.status == 500)
        //                                {
        //                                    return Json(new { status = 500, msg = "Lỗi: " + jsonReponseE.msg }, JsonRequestBehavior.AllowGet);
        //                                }
        //                            }
        //                        }
                                
                               
        //                        //if (isTrue)
        //                        //{
        //                        //    var reponse = Add(item, arrayDetailWarehouse, arrayepc);
        //                        //    var stringReponse = JsonConvert.SerializeObject(reponse.Data);
        //                        //    var jsonReponse = JsonConvert.DeserializeObject<reponses>(stringReponse);
        //                        //    if (jsonReponse.status == 200)
        //                        //    {
        //                        //        Idold.Add(item.Id);
        //                        //        isTrue = true;
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        foreach (var i in Idold)
        //                        //        {
        //                        //            Delete(i);
        //                        //        }

        //                        //        return Json(new { status = 500, msg = "Lỗi: " + jsonReponse.msg }, JsonRequestBehavior.AllowGet);
        //                        //    }
        //                        //}
        //                    }
        //                }
        //            }
        //        }


        //        return Json(new { status = 200, msg = "Thành công " }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { status = 500, msg = "Thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public class reponses
        {
            public string msg { get; set; }
            public int status { get; set; }
        }
        [HttpGet]
        public JsonResult DetailListGoods(int pagenum, int page, string idgoods)
        {
            try
            {
                var pageSize = pagenum;
                var epc = (from p in db.EPCs
                           where p.IdGoods == idgoods && p.Status == true
                           select new
                           {
                               idGoods = p.IdGoods,
                               idEPC = p.IdEPC,
                           }).ToList();
                var pages = epc.Count() % pageSize == 0 ? epc.Count() / pageSize : epc.Count() / pageSize + 1;
                var c = epc.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = epc.Count();
                return Json(new { code = 200, epc, pages = pages, count = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}