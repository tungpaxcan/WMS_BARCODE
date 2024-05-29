using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WMS.Controllers
{
    public class ReceiptStockController : BaseController
    {
        // GET: ReceiptStock
        private WMSEntities db = new WMSEntities();
        public ActionResult Index()
        {
            return View();
        }
        /* [HttpGet]
         public JsonResult WareHouse()
         {
             try
             {
                 var c = (from b in db.WareHouses.Where(x => x.Id.Length > 0)
                          select new
                          {
                              id = b.Id,
                              name = b.Name,
                          }).ToList();
                 return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
             }
             catch (Exception e)
             {
                 return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
             }
         }*/
        [HttpGet]
        public JsonResult ShowListWareHouse()
        {
            try
            {
                var slW = (from x in db.WareHouses
                           select new
                           {
                               id = x.Id,
                               name = x.Name,
                           }).ToList();
                return Json(new { code = 200, slW }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Hiển thị dữ liệu thất bại !!!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GoodsChose()
        {
            try
            {
                var c = (from b in db.Goods.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult ShowListGoods()
        {
            try
            {
                var slG = (from x in db.Goods
                           select new
                           {
                               Id = x.Id,
                               name = x.Name,
                               idUnit = x.IdUnit,
                               idGrGoods = x.IdGroupGood,
                               nameGroupGoods = x.GroupGood.Name == null ? "" : x.GroupGood.Name,
                               nameUnit = x.Unit.Name == null ? "" : x.Unit.Name,
                           }).ToList();
                return Json(new { code = 200, slG }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Hiển thị dữ liệu thất bại !!!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Add()
        {
            try
             {
                var session = (User)Session["user"];
                if (Request.Form.Count > 0)
                {
                    if (Request.Form["warehouse"] == "-1")
                    {
                        return Json(new { code = 500, msg = "Chọn Kho Hàng Để Tiếp Tục" }, JsonRequestBehavior.AllowGet);
                    }
                    if (Request.Form["good"] == "-1")
                    {
                        return Json(new { code = 500, msg = "Chọn Hàng Hóa Để Tiếp Tục" }, JsonRequestBehavior.AllowGet);
                    }
                    Stock stock = new Stock()
                    {
                        Description = Request.Form["des"],
                        CreateBy = session.Name,
                        CreateDate = DateTime.Now,
                        ModifyBy = session.Name,
                        ModifyDate = DateTime.Now,
                        Status = false,
                    };
                    var id = "";
                    var checkId = new Stock();
                    do
                    {
                        id = await Encode.GenerateRandomString(7);
                        checkId = db.Stocks.Find(id);
                    } while (checkId != null);

                    stock.Id = id;
                    List<DetailStock> data = new List<DetailStock>();

                    foreach (string key in Request.Form)
                    {
                        if (key.StartsWith("data"))
                        {
                            string[] parts = key.Split('.');
                            string input = parts[0];
                            Match match = Regex.Match(input, @"\d+");
                            int index = 0;
                            if (match.Success)
                            {
                                index = int.Parse(match.Value);
                            }
                            string propertyName = parts[1];
                            if (data.Count <= index)
                            {
                                data.Add(new DetailStock());
                            }
                            if (propertyName == "IdGoods")
                            {
                                var idg = Request.Form[key];
                                data[index].IdGoods = idg;
                                if (stock.IdGoods == null)
                                {
                                    stock.IdGoods += idg + ",";
                                }
                                else
                                {
                                    if (!stock.IdGoods.Contains(idg))
                                    {
                                        stock.IdGoods += idg + ",";
                                    }
                                }
                            }
                            else if (propertyName == "IdWarehouse")
                            {
                                var idw = Request.Form[key];
                                data[index].IdWareHouse = idw;
                                if (stock.IdWareHouse == null)
                                {
                                    stock.IdWareHouse += idw + ",";
                                }
                                else
                                {
                                    if (!stock.IdWareHouse.Contains(idw))
                                    {
                                        stock.IdWareHouse += idw + ",";
                                    }
                                }
                            }
                            if (data[index].IdGoods != null && data[index].IdWareHouse != null)
                            {
                                var idg = data[index].IdGoods;
                                var idw = data[index].IdWareHouse;
                                data[index].QuantityInvetory = db.DetailWareHouses.FirstOrDefault(x => x.IdGoods == idg && x.IdWareHouse == idw).Inventory;
                            }
                           
                            data[index].IdStock = stock.Id;
                            data[index].Status = false;
                            var iddetailStock = "";
                            do
                            {
                                iddetailStock = await Encode.GenerateRandomString(7);
                            } while (db.DetailStocks.Find(iddetailStock) != null || data.SingleOrDefault(x => x.Id == iddetailStock) != null);

                            data[index].Id = iddetailStock;
                        }
                    }

                    db.Stocks.Add(stock);
                    db.DetailStocks.AddRange(data);
                    db.SaveChanges();
                    return Json(new { code = 200, id, msg = "Tạo Phiếu Kiểm Kê Thành Công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = "Không Nhận Được Tất Cả Dữ Liệu" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Hiển thị dữ liệu thất bại " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DetailGoods(string id)
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
                        return Json(new { code = 500, msg = "Mã Hàng Hóa Không Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        return Json(new { code = 200, goods }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { code = 500, msg = "Mã Hàng Hóa Không Tồn Tại !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = 500, msg = "Thất Bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult getdeatil(string IdGoods, string ArrayDetail)
        {
            try
            {
                var warehouse = JArray.Parse(ArrayDetail);
                List<object> detailsList = new List<object>();
                foreach (var tempData in warehouse)
                {
                    var idwarehouse = tempData["IdWareHouse"].ToString();
                    var details = (from b in db.DetailWareHouses
                                   where b.IdGoods == IdGoods && b.IdWareHouse == idwarehouse
                                   select new
                                   {
                                       idgoods = b.IdGoods,
                                       nameGoods = b.Good.Name,
                                       idWarehouse = b.IdWareHouse,
                                       nameWareHouse = b.WareHouse.Name,
                                       qttInventory = b.Inventory,
                                   }).ToList();
                    detailsList.Add(details);
                }
                return Json(new { code = 200, msg = "Thành Công !!!", detailsList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Lỗi !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        //để hiển thị ra phiếu kiểm kê
        public JsonResult Detail(string id)
        {
            try
            {
                var session = (User)Session["user"];
                var nameAdmin = session.User1;
                if (!string.IsNullOrEmpty(id))
                {
                    var stock = (from p in db.Stocks
                                 where p.Id == id
                                 select new
                                 {
                                     id = p.Id,
                                     createDate = p.CreateDate,
                                     receiptstocker = p.CreateBy,
                                     //stocker = p.Stocker,
                                     status = p.Status,
                                 }).ToList();
                    var detailStock = (from s in db.DetailStocks
                                       where s.IdStock == id
                                       select new
                                       {
                                           idGoods = s.IdGoods,
                                           nameGoods = s.Good.Name,
                                           idGroupGoods = s.Good.IdGroupGood,
                                           groupGoods = s.Good.GroupGood.Name == null ? "" : s.Good.GroupGood.Name,
                                           idUnit = s.Good.IdUnit == null ? "" : s.Good.IdUnit,
                                           unitGoods = s.Good.Unit.Name == null ? "" : s.Good.Unit.Name,
                                           idWarehouse = s.IdWareHouse,
                                           nameWarehouse = s.WareHouse.Name,
                                           QuantityInventory = db.DetailWareHouses
                                               .FirstOrDefault(dw => dw.IdWareHouse == s.IdWareHouse && dw.IdGoods == s.IdGoods).Inventory,
                                       }).ToList();
                    var warehouse = (from w in db.DetailStocks
                                     where w.IdStock == id
                                     group w by w.IdWareHouse into grouped
                                     select new
                                     {
                                         idWareHouse = grouped.Key,
                                         nameWarehouse = grouped.FirstOrDefault().WareHouse.Name,
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
                        return Json(new { code = 200, stock, detailStock, warehouse }, JsonRequestBehavior.AllowGet);
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
    }
}