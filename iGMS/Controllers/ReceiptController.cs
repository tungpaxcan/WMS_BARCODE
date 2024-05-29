using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Windows.Forms;
using WMS.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WMS.Controllers
{
    public class ReceiptController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);

        // GET: Receipt
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Add(int statusSave, string id, string purchaseorder, string user1, string user2,string Des, string arrayGoods, string arrayepc)
        {
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(purchaseorder))
                {
                    var session = (ApiAccount)Session["user"];
                    var purchaseOrder = db.PurchaseOrders.Find(purchaseorder);
                    var existingReceipt = db.Receipts.Find(id);
                    var idwarehouse = purchaseOrder.IdWareHouse;
                    var InventoryStatus = db.ModelSettings.Find("inventorystatus").Status;
                    var detailGoodsOrder = JsonConvert.DeserializeObject<DetailGoodOrder[]>(arrayGoods);
                    var epcs = JsonConvert.DeserializeObject<string[]>(arrayepc);
                    // lưu vào receipt
                    if (existingReceipt == null)
                    {
                        var newReceipt = new Receipt
                        {
                            Id = id,
                            IdPurchaseOrder = purchaseorder,
                            IdUser1 = user1 == "-1" ? null : user1,
                            IdUser2 = user2 == "-1" ? null : user2,
                            Description = Des == "" ? null : Des,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreateBy = session.FullName,
                            ModifyBy = session.FullName,
                            Status = statusSave == 1 ? true : false,
                        };
                        db.Receipts.Add(newReceipt);
                        var listReceiptFalse = db.Receipts.Where(x => x.IdPurchaseOrder == purchaseorder).ToList();
                        if(statusSave == 1)
                        {
                            foreach(var item in listReceiptFalse)
                            {
                                item.Status = true;
                            }
                        }
                        // đổi status trong purchaseorder thành true
                        if (purchaseOrder != null)
                        {
                            purchaseOrder.Status = statusSave == 1 ? true : false;
                        }
                        else
                        {
                            return Json(new { status = 500, msg = rm.GetString("Mã Phiếu Nhập Không Tồn Tại !!!").ToString() }, JsonRequestBehavior.AllowGet);
                        }
                        // Cập nhật DetailGoodOrder
                        foreach (var detail in detailGoodsOrder)
                        {

                            var de = db.DetailGoodOrders
                                .SingleOrDefault(d => d.PurchaseOrder.IdWareHouse == idwarehouse && d.IdPurchaseOrder == purchaseorder && d.IdGoods == detail.IdGoods);
                            var qtyScanned = detail.QuantityScan - (de.QuantityScan == null ? 0 : de.QuantityScan);
                            if (de == null)
                            {
                                return Json(new { status = 500, msg = rm.GetString("Mã Hàng Trong Danh Sách Không Có Trong Phiếu Nhập").ToString() }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var dew = db.DetailWareHouses
                                .SingleOrDefault(d => d.IdWareHouse == idwarehouse && d.IdGoods == detail.IdGoods);
                                if (dew != null)
                                {
                                    if (InventoryStatus == true)
                                    {
                                        var goods = db.Goods.Find(detail.IdGoods);
                                        if (goods != null)
                                        {
                                            goods.Inventory += qtyScanned;
                                        }
                                        dew.Inventory += qtyScanned;
                                    }
                                    de.IdReceipt = id;
                                    de.QuantityScan = detail.QuantityScan;
                                    de.Status = statusSave == 1 ? true : false;
                                    de.ModifyDate = DateTime.Now;
                                    de.ModifyBy = session.FullName;
                                }
                                else
                                {
                                    var newDew = new DetailWareHouse
                                    {
                                        IdWareHouse = idwarehouse,
                                        IdGoods = detail.IdGoods,
                                        Inventory = detail.QuantityScan,
                                        Status = true,
                                    };
                                    db.DetailWareHouses.Add(newDew);
                                }
                            }
                        }
                        //cập nhật epc
                        foreach (var epc in epcs)
                        {
                            if (!string.IsNullOrEmpty(epc))
                            {
                                var e = db.EPCs.SingleOrDefault(d => d.IdEPC == epc && d.Status == false);
                                if (e != null)
                                {
                                    e.Status = true;
                                    e.IdReceipt = id;
                                    e.IdWareHouse = idwarehouse;
                                }
                                else
                                {
                                    return Json(new { status = 300, epc = epc, c = e.IdGoods, msg = rm.GetString("Mã Hàng") + e.IdGoods + rm.GetString("Đã Tồn Tại, Có Mã EPC Là") + epc + " !!!, "+ rm.GetString("Hoặc Chưa Có Trong Hệ Thống") }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                       
                        return Json(new { status = 500, msg = rm.GetString("Mã Phiếu Nhập Đã Tồn Tại").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = 200, msg = rm.GetString("CreateSucess") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = 500, msg = rm.GetString("Nhập Đủ Số Phiếu Nhập").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
     
    [HttpGet]
        public JsonResult UserNV()
        {
            try
            {
                var c = (from b in db.Users.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public JsonResult ReceipLast(string id)
        {
            try
            {
                var a = (from b in db.Receipts.Where(x => x.Id == id)
                         select new { id = b.Id, datepn = b.CreateDate.Value.Day+"/"+ b.CreateDate.Value.Month+"/"+ b.CreateDate.Value.Year }).ToList();
                return Json(new { code = 200,a=a}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SendEpc(string epc)
        {
            try
            {
                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("sendepc"+epc);
                return Json(new { code = 200 ,msg=rm.GetString("Đã gửi thành công").ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg ="lỗi" +e.Message }, JsonRequestBehavior.AllowGet);
            }
        }  
        [HttpGet]
        public JsonResult ListPO()
        {
            try
            {
                var data = (from r in db.Receipts where r.Status == true
                            select new
                            {
                                id = r.Id,
                            }).ToList();
                return Json(new { code = 200 ,data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg ="lỗi" +e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Detail(string id)
        {
            try
            {
                var data = (from r in db.Receipts
                            where r.Status == true && r.Id == id
                            join p in db.PurchaseOrders on r.IdPurchaseOrder equals p.Id
                            join d in db.DetailGoodOrders on r.Id equals d.IdReceipt
                            select new
                            {
                                nameWarehouse = p.WareHouse.Name,
                                idWarehouse = p.WareHouse.Id,
                                nameCustomer = p.Customer.Name,
                                addressCustomer = p.Customer.AddRess,
                                idGoods = d.Good.Id,
                                nameGoods = d.Good.Name,
                                quantity = d.Quantity,
                                groupGoods = d.Good.GroupGood.Name,
                                unitGoods = d.Good.Unit.Name,
                            }).ToList();
                if(data.Count() <= 0)
                {
                    return Json(new { code = 500, msg = "Không Có Dữ Liệu Cho PO Này" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 200, data }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "lỗi" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}