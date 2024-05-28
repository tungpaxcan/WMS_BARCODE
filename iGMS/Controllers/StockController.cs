using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WMS.Controllers
{
    public class StockController : BaseController
    {
        // GET: Stock
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public JsonResult WareHouse()
        {
            try
            {
                var w = (from b in db.WareHouses.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();

                return Json(new { code = 200, w }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult WarehousesByStockId(string idStock)
        {
            try
            {
                var check = db.DetailStocks.FirstOrDefault(x => x.IdStock == idStock);
                if (check == null)
                {
                    return Json(new { code = 500, msg = "Mã Phiếu Không Tồn Tại" }, JsonRequestBehavior.AllowGet);
                }
                var warehouses = db.DetailStocks
                    .Where(x => x.IdStock == idStock)
                    .Select(s => new
                    {
                        idWarehouse = s.IdWareHouse,
                        nameWarehouse = s.WareHouse.Name,
                    })
                    .Distinct()// lấy giá trị duy nhất từ 1 tập hợp.
                    .ToList();
                var detailStockEpc = (from d in db.DetailStockEpcs
                                      where d.DetailStock.IdStock == idStock
                                      select new
                                      {
                                          epc = d.IdEpc,
                                          id = d.IdDetailStock
                                      }).ToList();
                return Json(new { code = 200, msg = "Vui Lòng Chọn Kho Kiểm Kê", warehouses, detailStockEpc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DetailList(string id)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                var c = db.DetailStocks
                       .Where(x => x.IdStock == id)
                       .Select(s => new
                       {
                           idgoods = s.IdGoods,
                           namegoods = s.Good.Name,
                           namewarehouse = s.WareHouse.Name,
                           QuantityInventory = s.QuantityInvetory,
                           qttStock = s.QuantityStock == null ? 0 : s.QuantityStock,
                       }).Distinct().ToList();
                return Json(new { code = 200, c }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi !!!" + e.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult Detail(string idStock, string Warehouse)
        {
            try
            {
                var check = db.DetailStocks.FirstOrDefault(x => x.IdStock == idStock);
                if (check == null)
                {
                    return Json(new { code = 500, msg = "Mã Phiếu Không Tồn Tại" }, JsonRequestBehavior.AllowGet);
                }
                var checkSTT = db.DetailStocks.FirstOrDefault(x => x.IdStock == idStock && x.Status == true);
                if (checkSTT != null)
                {
                    return Json(new { code = 500, msg = "Mã Phiếu Đã Được Kiểm Kê" }, JsonRequestBehavior.AllowGet);
                }
                var checkidStock = db.DetailStocks.FirstOrDefault(x => x.IdStock == idStock && x.IdWareHouse == Warehouse && x.Status == false);
                if (checkidStock != null)
                {
                    var stock = db.DetailStocks
                        .Where(p => p.IdStock == idStock && p.IdWareHouse == Warehouse && p.Status == false)
                        .GroupBy(p => p.IdStock)
                        .Select(g => new
                        {
                            IdStock = g.Key,
                            GoodsList = g.Select(item => new
                            {
                                id = item.Id,
                                qttStock = item.QuantityStock == null ? 0 : item.QuantityStock,
                                qttSimilar = item.QuantityInvetory == null ? 0 : item.QuantityInvetory,
                                idWarehouse = item.IdWareHouse,
                                nameWarehouse = item.WareHouse.Name,
                                idg = item.IdGoods,
                                nameGoods = item.Good.Name,
                                QuantityInventory = db.DetailWareHouses
                                  .FirstOrDefault(dw => dw.IdWareHouse == item.IdWareHouse && dw.IdGoods == item.IdGoods).Inventory,
                            }).ToList()
                        })
                        .ToList();

                    return Json(new { code = 200, stock }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = "Mã Phiếu Đã Kiểm Kê !!!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi !!!" + e.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        //để hiển thị ra phiếu kiểm kê
        public JsonResult DetailBill(string idStock)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                if (!string.IsNullOrEmpty(idStock))
                {
                    var stock = (from p in db.Stocks
                                 where p.Id == idStock
                                 select new
                                 {
                                     id = p.Id,
                                     createDate = p.CreateDate,
                                     stocker = p.DetailStocks,
                                     status = p.Status,
                                 }).ToList();
                    var detailStock = (from s in db.DetailStocks
                                       where s.IdStock == idStock
                                       select new
                                       {
                                           idGoods = s.IdGoods,
                                           nameGoods = s.Good.Name,
                                           idWarehouse = s.IdWareHouse,
                                           nameWarehouse = s.WareHouse.Name,
                                           qttInvetory = s.QuantityInvetory,
                                           qttStock = s.QuantityStock,
                                           res = s.Residual,
                                           incom = s.Incomplete,
                                           suffice = s.Suffice,
                                           qttSimilar = s.QuantityInvetory,
                                       }).ToList();

                    if (stock == null)
                    {
                        return Json(new { code = 500, msg = "Mã Phiếu Không Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                    }
                    if (detailStock == null)
                    {
                        return Json(new { code = 500, msg = "Mã Phiếu Không Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { code = 200, stock, detailStock }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { code = 500, msg = "Vui Lòng Nhập Mã Phiếu !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Hiển thị dữ liệu thất bại !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Epcshowlist(string idWareHouse)
        {
            try
            {
                /* var session = (ApiAccount)Session["user"];
                 var nameAdmin = session.UserName;*/
                if (!string.IsNullOrEmpty(idWareHouse))
                {
                    var e = db.EPCs
                        .Where(x => x.IdWareHouse == idWareHouse)
                        .Select(x => new
                        {
                            x.IdGoods,
                            x.IdEPC
                        })
                        .ToList();
                    return Json(new { code = 200, e }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = "lỗi" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {

                return Json(new { code = 500, msg = "Đã xảy ra lỗi: " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Add(int statusSave, string idStock, string detailEpcScan)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                if (!string.IsNullOrEmpty(idStock))
                {
                    var detailEpcScans = JsonConvert.DeserializeObject<DetailStockEpc[]>(detailEpcScan);
                    var existingStock = db.Stocks.Find(idStock);
                    if (existingStock == null)
                    {
                        return Json(new { code = 500, msg = rm.GetString("Mã Phiếu Kiểm Không Tồn Tại").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        existingStock.ModifyDate = DateTime.Now;
                        existingStock.ModifyBy = session.UserName;
                        existingStock.Status = statusSave == 1 ? true : false;
                    }
                    var arrayDetailStock =  detailEpcScans.GroupBy(x => x.IdDetailStock).ToList();
                    foreach(var item in arrayDetailStock)
                    {
                        var detailStock = db.DetailStocks.Find(item.Key.ToString());
                        if (detailStock != null)
                        {
                            var countScan = detailEpcScans.Count(x=>x.IdDetailStock==item.Key.ToString());
                            detailStock.Status = statusSave == 1 ? true : false;
                            detailStock.QuantityStock = detailStock.QuantityStock == null? countScan: detailStock.QuantityStock+= countScan;
                        }
                        else
                        {
                            return Json(new { code = 500, msg = rm.GetString("Lỗi Chi Tiết Phiếu Kiểm").ToString() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    db.DetailStockEpcs.AddRange(detailEpcScans);
                    db.SaveChanges();
                }
                return Json(new { code = 200, msg = "Kiểm Kê Thành Công!!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi !!!" + e.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult ShowListStock(string idStock, string idwarehouse, DateTime l, DateTime f, string status)
        {
            try
            {
                string scanned = rm.GetString("Đã Quét").ToString();
                string notScanned = rm.GetString("Chưa Quét").ToString();
                string notScannedYet = rm.GetString("Chưa Quét Xong").ToString();
                bool? st = null;
                if (status == "true")
                {
                    st = true;
                }
                else if (status == "false")
                {
                    st = false;
                }

                var queryNotScannedYet = from r in db.Stocks
                                         where r.IdWareHouse.Contains(idwarehouse) &&
                                               r.Id.Contains(idStock) &&
                                               r.CreateDate >= l &&
                                               r.CreateDate <= f &&
                                               r.CreateDate != r.ModifyDate&&
                                               r.Status == false
                                         select new
                                         {
                                             idstock = r.Id,
                                             createDate = r.CreateDate,
                                             nameWarehouse = r.IdWareHouse,
                                             status = notScannedYet
                                         };
                var wNotScannedYet = queryNotScannedYet.OrderByDescending(x => x.createDate).ToList();
                var show = (from s in db.Stocks
                            where s.IdWareHouse.Contains(idwarehouse) &&
                            s.Id.Contains(idStock) &&
                            s.CreateDate >= l &&
                            s.CreateDate <= f &&
                             (!st.HasValue || s.Status == st.Value)
                            select new
                            {
                                idstock = s.Id,
                                createDate = s.CreateDate,
                                nameWarehouse = s.IdWareHouse,
                                status = s.Status == true ? scanned : notScanned
                            });
                var wPuchase = show.OrderByDescending(x => x.createDate).ToList();
                if (status == "false2")
                {
                    return Json(new { code = 200, show = wNotScannedYet }, JsonRequestBehavior.AllowGet);
                }
                else if (status == "true" || status == "false")
                {
                    return Json(new { code = 200, show = wPuchase }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 200, show = wNotScannedYet.Concat(wPuchase).ToList() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}