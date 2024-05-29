using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using WMS.Models;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Resources;

namespace WMS.Controllers
{
    public class PurchaseOrderController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: PurchaseOrder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
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
                        return Json(new { code = 500, msg = rm.GetString("Chọn Kho Hàng Để Tiếp Tục").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    if (Request.Form["customer"] == "-1")
                    {
                        return Json(new { code = 500, msg = rm.GetString("Chọn Khách Hàng Để Tiếp Tục").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    var ttSum = int.Parse(Request.Form["tt"]); // số lượng cbi nhập
                    var idWareHouse = Request.Form["warehouse"]; // lấy mã kho hàng
                    var maxInvetory = db.WareHouses.Find(idWareHouse).MaxInventory; // số lượng kho chứa lớn nhất
                    var totalInventory = db.DetailWareHouses.Where(d => d.IdWareHouse == idWareHouse)
                       .GroupBy(d => d.IdWareHouse)
                       .Select(g => new
                       {
                           IdWareHouse = g.Key,
                           TotalInventory = g.Sum(d => d.Inventory)
                       }).FirstOrDefault();
                    var sumInventory = totalInventory == null ? 0 : totalInventory.TotalInventory; // tổng Invetory trong kho đang có
                    //tong sl chuan bi nhap vao kho
                    var totalQuantity = db.DetailGoodOrders
                        .Where(d => d.PurchaseOrder.IdWareHouse == idWareHouse&&d.Status==false&&d.IdReceipt==null)
                        .Sum(d => d.Quantity);
                    totalQuantity = totalQuantity == null ? 0 : totalQuantity;
                    //tong so luong cua kho va cac sl cbi nhap kho
                    var ttqtt = totalQuantity + ttSum + sumInventory;
                    if (ttqtt >= maxInvetory)
                    {
                        return Json(new { code = 500, msg = rm.GetString("Số Lượng Nhập Vượt Quá Số Lượng Cho Phép Của Kho").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    PurchaseOrder purchaseOrder = new PurchaseOrder()
                    {
                        IdWareHouse = Request.Form["warehouse"],
                        IdCustomer = Request.Form["customer"],
                        Name = Request.Form["name"],
                        Deliver = Request.Form["deliver"],
                        Description = Request.Form["des"],
                        CreateBy = session.Name,
                        CreateDate = DateTime.Now,
                        ModifyBy = session.Name,
                        ModifyDate = DateTime.Now,
                        Status = false,
                    };
                    var id = "";
                    var checkId = new PurchaseOrder();
                    do
                    {
                        id = await Encode.GenerateRandomString(7);
                        checkId = db.PurchaseOrders.Find(id);
                    } while (checkId != null);
                    purchaseOrder.Id = id;
                    List<DetailGoodOrder> data = new List<DetailGoodOrder>();
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
                                data.Add(new DetailGoodOrder());
                            }
                            if (propertyName == "IdGoods")
                            {
                                data[index].IdGoods = Request.Form[key];
                            }
                            else if (propertyName == "Quantity")
                            {
                                data[index].Quantity = float.Parse(Request.Form[key]);
                            }
                            data[index].CreateDate = DateTime.Now;
                            data[index].ModifyDate = DateTime.Now;
                            data[index].CreateBy = session.Name;
                            data[index].ModifyBy = session.Name;
                            data[index].IdPurchaseOrder = id;
                            data[index].Status = false;
                        }
                    }

                    db.PurchaseOrders.Add(purchaseOrder);
                    db.DetailGoodOrders.AddRange(data);
                    db.SaveChanges();
                    return Json(new { code = 200, id, msg = rm.GetString("Tạo Phiếu Nhập Hàng Thành Công").ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("Không Nhận Được Tất Cả Dữ Liệu").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Detail(string id)
        {
            try
            {
                var session = (User)Session["user"];
                var nameAdmin = session.User1;
                if (!string.IsNullOrEmpty(id))
                {                   
                    var po = (from p in db.PurchaseOrders
                              where p.Id == id 
                              select new
                              {
                                  id = p.Id,
                                  createDate = p.CreateDate,
                                  nameCustomer = p.Customer.Name,
                                  idCustomer = p.Customer.Id,
                                  nameWarehouse = p.WareHouse.Name,
                                  idWarehouse = p.IdWareHouse,
                                  addressCustomer= p.Customer.AddRess,
                                  phone= p.Customer.Phone,
                                  status =p.Status,
                                  
                              }).ToList().LastOrDefault();   
                    var detailPo = (from p in db.DetailGoodOrders
                                    where p.IdPurchaseOrder == id
                                    select new
                                    {
                                        id = p.Id,
                                        idGoods = p.IdGoods,
                                        nameGoods = p.Good.Name,
                                        quantity = p.Quantity,
                                        quantityscan = p.QuantityScan,
                                        idGroupGoods = p.Good.IdGroupGood,
                                        nameGroupGoods = p.Good.GroupGood.Name==null?"": p.Good.GroupGood.Name,
                                        idUnit = p.Good.IdUnit== null?"":p.Good.IdUnit,
                                        nameUnit = p.Good.Unit.Name == null ? "" : p.Good.Unit.Name,
                                        status = p.Status,
                                    }).ToList();
                    if(po == null)
                    {
                        return Json(new { code = 500, msg = rm.GetString("Mã Phiếu Nhập Không Tồn Tại !!!").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { code = 200, po, detailPo, msg= rm.GetString("Mã Phiếu Nhập Hợp Lệ").ToString() }, JsonRequestBehavior.AllowGet);
                    }  
                }else {
                    return Json(new { code = 500, msg=rm.GetString("Vui Lòng Nhập Mã Phiếu Nhập").ToString() }, JsonRequestBehavior.AllowGet); 
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") }, JsonRequestBehavior.AllowGet);
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
                             name = b.Name,
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") }, JsonRequestBehavior.AllowGet);
            }
        }
       
        [HttpGet]
        public JsonResult EPCDaCo()
        {
            try
            {
                var c = (from b in db.EPCs.Where(x => x.Status == true)
                         select new
                         {
                             epc = b.IdGoods
                         }).ToList();
                var d = (from b in db.DetailGoodOrders.Where(x => x.Status==true)
                         select new
                         {
                             epc = b.IdGoods
                         }).ToList();
                return Json(new { code = 200, c = c,d=d }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") }, JsonRequestBehavior.AllowGet);
            }
        }
     
        [HttpGet]
        public JsonResult ShowListPurchase(DateTime s, DateTime e, string warehouse, string idpurchase, string status)
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
                }else if (status == "false")
                {
                    st = false;
                }
                
                var queryNotScannedYet = from r in db.Receipts
                                         where r.IdPurchaseOrder != null && r.Status == false
                                         join b in db.WareHouses on r.PurchaseOrder.IdWareHouse equals b.Id
                                         where r.PurchaseOrder.IdWareHouse.Contains(warehouse) &&
                                               r.PurchaseOrder.Id.Contains(idpurchase) &&
                                               r.PurchaseOrder.CreateDate >= s &&
                                               r.PurchaseOrder.CreateDate <= e
                                         select new
                                         {
                                             id = r.PurchaseOrder.Id,
                                             idwarehouse = b.Id,
                                             namewarehouse = b.Name,
                                             createdate = r.PurchaseOrder.CreateDate,
                                             namecustomer = r.PurchaseOrder.Customer.Name,
                                             status = notScannedYet
                                         };
                var wNotScannedYet = queryNotScannedYet.OrderByDescending(x => x.createdate).ToList();
                var queryPuchase = from x in db.PurchaseOrders
                                   join b in db.WareHouses on x.IdWareHouse equals b.Id
                                   where x.IdWareHouse.Contains(warehouse) &&
                                         x.Id.Contains(idpurchase) &&
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
                if(status == "false2")
                {
                    return Json(new { code = 200, w =wNotScannedYet }, JsonRequestBehavior.AllowGet);
                }else if(status == "true" || status == "false")
                {
                    return Json(new { code = 200, w=wPuchase }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 200, w = wNotScannedYet.Concat(wPuchase).ToList() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = rm.GetString("false") }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}