using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class DeliveryController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Delivery
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Add(int statusSave,string id, string idSaleOrder, string Des, string ArraySales, string ArrayEPC)
        {
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(idSaleOrder))
                {
                    var session = (ApiAccount)Session["user"];
                    var salesOrder = db.SalesOrders.Find(idSaleOrder);
                    var existingDelivery = db.Deliveries.Find(id);
                    var idwarehouse = salesOrder.IdWareHouse;
                    var InventoryStatus = db.ModelSettings.Find("inventorystatus").Status;
                    var detailSaleOrder = JsonConvert.DeserializeObject<DetailSaleOrder[]>(ArraySales);
                    var epcs = JsonConvert.DeserializeObject<string[]>(ArrayEPC);
                    // lưu vào delivery
                    if (existingDelivery == null)
                    {
                        var newDelivery = new Delivery
                        {
                            Id = id,
                            IdSalesOrder = idSaleOrder,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Description = Des == "" ? null : Des,
                            CreateBy = session.FullName,
                            ModifyBy = session.FullName,
                            Status = statusSave == 1 ? true : false,
                        };
                        db.Deliveries.Add(newDelivery);
                        var listDeliveryFalse = db.Deliveries.Where(x => x.IdSalesOrder == idSaleOrder).ToList();
                        if (statusSave == 1)
                        {
                            foreach (var item in listDeliveryFalse)
                            {
                                item.Status = true;
                            }
                        }
                        // đổi status trong delivery thành true
                        if (salesOrder != null)
                        {
                            salesOrder.Status = statusSave == 1 ? true : false;
                        }
                        else
                        {
                            return Json(new { status = 500, msg = rm.GetString("Mã Phiếu Xuất Không Tồn Tại").ToString() }, JsonRequestBehavior.AllowGet);
                        }
                        // Cập nhật DetailSaleOrder
                        var idgoods = "";
                        foreach (var detail in detailSaleOrder)
                        {
                            
                            var de = db.DetailSaleOrders.SingleOrDefault(d =>d.SalesOrder.IdWareHouse == idwarehouse && d.IdSaleOrder == idSaleOrder && d.IdGoods == detail.IdGoods);
                            if (de == null)
                            {
                                return Json(new { status = 500, msg = rm.GetString("Mã Hàng Trong Danh Sách Chưa Có Trong Phiếu Xuất").ToString() }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                idgoods = detail.IdGoods;
                                var qtyScanned = detail.QuantityScan - (de.QuantityScan == null ? 0 : de.QuantityScan);
                                var dew = db.DetailWareHouses
                               .SingleOrDefault(d => d.IdWareHouse == idwarehouse && d.IdGoods == detail.IdGoods);
                                if (dew != null)
                                {
                                    if (InventoryStatus == true)
                                    {
                                        var goods = db.Goods.Find(detail.IdGoods);
                                        if (goods != null)
                                        {
                                            goods.Inventory -= qtyScanned;
                                        }
                                        dew.Inventory -= qtyScanned;
                                    }
                                    de.IdDelivery = id;
                                    de.QuantityScan = detail.QuantityScan;
                                    de.Status = statusSave == 1 ? true : false;
                                    de.ModifyDate = DateTime.Now;
                                    de.ModifyBy = session.FullName;
                                }
                            }
                        }
                        foreach (var epc in epcs)
                        {
                            var e = db.EPCs.FirstOrDefault(d => d.IdEPC == epc && d.IdWareHouse == idwarehouse && d.Status == true);
                            if (e == null)
                            {
                                return Json(new { status = 500,c = idgoods, msg = rm.GetString("Mã Hàng") + idgoods + rm.GetString("Có Mã EPC Là") + epc + rm.GetString("Không Tồn Tại") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                db.EPCs.Remove(e);
                            }
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { status = 500, msg = rm.GetString("Mã Phiếu Xuất Đã Tồn Tại").ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = 200, msg =rm.GetString("Lưu Thành Công").ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = 500, msg = rm.GetString("Nhập Đủ Số Phiếu Xuất").ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var d = db.Deliveries.Where(x => x.IdSalesOrder == id).ToList();
                db.Deliveries.RemoveRange(d);
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("SucessDelete") }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { code = 500, msg = rm.GetString("SucessEdit") }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Last(string id)
        {
            try
            {
                var a = (from b in db.Deliveries.Where(x => x.Id == id)
                         select new { id = b.Id, datepx = b.CreateDate.Value.Day + "/" + b.CreateDate.Value.Month + "/" + b.CreateDate.Value.Year }).ToList();
                return Json(new { code = 200, a = a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg =rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }

}
