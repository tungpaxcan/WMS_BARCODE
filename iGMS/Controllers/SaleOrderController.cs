
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using WMS.Models;

namespace WMS.Controllers
{
    public class SaleOrderController : BaseController
    {
        private WMSEntities db = new WMSEntities();

        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: SaleOrder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Customer()
        {
            try
            {
                var c = (from b in db.Customers.Where(x => x.Id.Length > 0)
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
                var h = (from b in db.WareHouses.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();

                return Json(new { code=200, h }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Add()
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                if (Request.Form.Count > 0)
                {
                    if (Request.Form["warehouse"] == "-1")
                    {
                        return Json(new { code = 500, msg = rm.GetString("Chọn Kho Hàng Để Tiếp Tục").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    if (Request.Form["customer"] == "-1") 
                    {
                        return Json(new { code = 500, msg = rm.GetString("Chọn Khách Hàng Để Tiếp Tục").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    var ttSum = int.Parse(Request.Form["tt"]);
                    var idWareHouse = Request.Form["warehouse"];
                    var minInvetory = db.WareHouses.Find(idWareHouse).MinInventory;
                    var maxInvetory = db.WareHouses.Find(idWareHouse).MaxInventory;
                    SalesOrder salesOrder = new SalesOrder()
                    {
                        IdWareHouse = Request.Form["warehouse"],
                        IdCustomer = Request.Form["customer"],
                        Name = Request.Form["name"],
                        Description = Request.Form["des"],
                        CreateBy = session.FullName,
                        CreateDate = DateTime.Now,
                        ModifyBy = session.FullName,
                        ModifyDate = DateTime.Now,
                        Status = false,
                    };
                    var id = "";
                    var checkId = new SalesOrder();
                    do
                    {
                        id = await Encode.GenerateRandomString(7);
                        checkId = db.SalesOrders.Find(id);
                    } while (checkId != null);
                    salesOrder.Id = id;
                    List<DetailSaleOrder> data = new List<DetailSaleOrder>();
                    var idGoods = "";
                    int ttGoodsWare = 0;
                    int ttReadySale = 0;
                    int ttCurrent = 0;
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
                                index = int.Parse(match.Value); // Extract the index from the key
                            }

                            string propertyName = parts[1]; // Extract the property name from the key

                            if (data.Count <= index)
                            {
                                data.Add(new DetailSaleOrder());
                            }
                            var nameWareHouse = db.WareHouses.Find(idWareHouse).Name;

                            if (propertyName == "IdGoods")
                            {
                                idGoods = Request.Form[key];
                                var listReadySale = db.DetailSaleOrders
                                   .Where(d => d.Status == false && d.SalesOrder.IdWareHouse == idWareHouse && d.IdGoods == idGoods).ToList();
                                var GoodsWare = db.DetailWareHouses
                                    .Where(d => d.IdWareHouse == idWareHouse && d.IdGoods == idGoods).ToList();
                                if (GoodsWare != null)
                                {
                                    ttGoodsWare = (int)GoodsWare.FirstOrDefault().Inventory;
                                }
                                else
                                {
                                    return Json(new { code = 500, msg = $"{rm.GetString("Mã Hàng")} {idGoods} {rm.GetString("Không Tồn Tại Trong Kho")}" }, JsonRequestBehavior.AllowGet);
                                }
                                if (listReadySale != null)
                                {
                                    ttReadySale = (int)listReadySale.Sum(c => c.Quantity);
                                }
                                data[index].IdGoods = idGoods;
                            }
                            else if (propertyName == "Quantity")
                            {
                                ttCurrent = int.Parse(Request.Form[key]);
                                data[index].Quantity = float.Parse(Request.Form[key]);
                            }
                            if (ttGoodsWare - ttReadySale < ttCurrent)
                            {
                                return Json(new { code = 500, msg =$"{rm.GetString("Mã Hàng")} {idGoods} {rm.GetString("Trong")} {nameWareHouse} {rm.GetString("Không Đủ Để Xuất")}"}, JsonRequestBehavior.AllowGet);
                            }
                            data[index].CreateDate = DateTime.Now;
                            data[index].ModifyDate = DateTime.Now;
                            data[index].CreateBy = session.FullName;
                            data[index].ModifyBy = session.FullName;
                            data[index].IdSaleOrder = id;
                            data[index].Status = false;
                        }
                    }
                    db.SalesOrders.Add(salesOrder);
                    db.DetailSaleOrders.AddRange(data);
                    db.SaveChanges();
                    return Json(new { code = 200, id, msg = rm.GetString("Tạo Phiếu Xuất Hàng Thành Công").ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("Không Nhận Được Tất Cả Dữ Liệu").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg =rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult Bill(string id)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                if (!string.IsNullOrEmpty(id))
                {
                    var c = (from b in db.SalesOrders.Where(x => x.Id == id)
                             select new
                             {
                                 id = b.Id,
                                 createdate = b.CreateDate.Value.Day + "/" + b.CreateDate.Value.Month + "/" + b.CreateDate.Value.Year,
                                 customer = b.Customer.Name,
                                 address = b.Customer.AddRess,
                                 ware = b.WareHouse.Name,
                                 status = b.Status,
                             }).ToList();

                    if (c.Count == 0) 
                    {
                        return Json(new { code = 500, msg = rm.GetString("Mã Phiếu Xuất Không Tồn Tại").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    
                    var e = (from b in db.DetailSaleOrders.Where(x => x.IdSaleOrder == id)
                             select new
                             {
                                 id = b.Id,
                                 idgood = b.IdGoods,
                                 name = b.Good.Name,
                                 unit = b.Good.Unit.Name == null ?"": b.Good.Unit.Name,
                                 gr = b.Good.GroupGood.Name == null ? "": b.Good.GroupGood.Name,
                                 qtt = b.Quantity,
                                 qttscan = b.QuantityScan,
                             }).ToList();

                    return Json(new { code = 200, c, e, msg = rm.GetString("Mã Phiếu Xuất Hợp Lệ").ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("Vui Lòng Nhập Mã Phiếu Xuất").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ShowListSaleOrder(DateTime s, DateTime e, string id, string idwarehouse, string status)
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
                var queryNotScannedYet = from r in db.Deliveries
                                         where r.IdSalesOrder != null && r.Status == false
                                         join b in db.WareHouses on r.SalesOrder.IdWareHouse equals b.Id
                                         where r.SalesOrder.IdWareHouse.Contains(idwarehouse) &&
                                               r.SalesOrder.Id.Contains(id) &&
                                               r.SalesOrder.CreateDate >= s &&
                                               r.SalesOrder.CreateDate <= e
                                         select new
                                         {
                                             id = r.SalesOrder.Id,
                                             idwarehouse = b.Id,
                                             namewarehouse = b.Name,
                                             createdate = r.SalesOrder.CreateDate,
                                             namecustomer = r.SalesOrder.Customer.Name,
                                             status = notScannedYet
                                         };
                var wNotScannedYet = queryNotScannedYet.OrderByDescending(x => x.createdate).ToList();
                var queryPuchase = from x in db.SalesOrders
                                   join b in db.WareHouses on x.IdWareHouse equals b.Id
                                   where x.IdWareHouse.Contains(idwarehouse) &&
                                         x.Id.Contains(id) &&
                                         x.CreateDate >= s &&
                                         x.CreateDate <= e &&
                                         (!st.HasValue || x.Status == st.Value)
                                   select new
                                   {
                                       id = x.Id,
                                       idwarehouse = b.Id,
                                       namewarehouse = b.Name,
                                       createdate = x.CreateDate,
                                       namecustomer = x.Customer.Name,
                                       status = x.Status == true ? scanned : notScanned
                                   };

                var wPuchase = queryPuchase.OrderByDescending(x => x.createdate).ToList();

                if (status == "false2")
                {
                    return Json(new { code = 200, w = wNotScannedYet }, JsonRequestBehavior.AllowGet);
                }
                else if (status == "true" || status == "false")
                {
                    return Json(new { code = 200, w = wPuchase }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 200, w = wNotScannedYet.Concat(wPuchase).ToList() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }

}