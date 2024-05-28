using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class RoleAdminController : Controller
    {
        private WMSEntities db = new WMSEntities();
        // GET: RoleAdmin
        [HttpPost]
        public JsonResult RoleAdmin(bool manageMaincategories, bool purchasemanager, bool salesmanager,
                                    bool warehousemanagement, bool managepayments, bool accountingtransfer)
        {
            try
            {
                var d = new RoleAdmin();
                d.ManageMainCategories = manageMaincategories;
                d.PurchaseManager = purchasemanager;
                d.SalesManager = salesmanager;
                d.WarehouseManagement = warehousemanagement;
                db.RoleAdmins.Add(d);
                db.SaveChanges();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Hiểm thị dữ liệu thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult EditRoleAdmin(int idroleadmin, bool manageMaincategories, bool purchasemanager, bool salesmanager,
                                 bool warehousemanagement, bool managepayments, bool accountingtransfer)
        {
            try
            {
                var d = db.RoleAdmins.Find(idroleadmin);
                d.ManageMainCategories = manageMaincategories;
                d.PurchaseManager = purchasemanager;
                d.SalesManager = salesmanager;
                d.WarehouseManagement = warehousemanagement;
                db.SaveChanges();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Hiểm thị dữ liệu thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //    [HttpPost]
        //    public JsonResult Role(bool admin,bool editdiscountgoods,bool editdiscountbill,bool editpricegoods,
        //                           bool editamountgoods,bool deletegoods,bool printagainbill,bool hangbill, bool changecategoods,
        //                           bool changeunit,bool editdate,bool identifyconsultants,bool confirmcusinfor,bool returngoods)
        //    {
        //        try
        //        {
        //            var d = new Role();
        //            d.Admin = admin;
        //            d.EditDiscountGoods = editdiscountgoods;
        //            d.EditDiscountBill = editdiscountbill;
        //            d.EditPriceGoods = editpricegoods;
        //            d.EditAmountGoods = editamountgoods;
        //            d.DeleteGoods = deletegoods;
        //            d.PrintAgainBill = printagainbill;
        //            d.HangBill = hangbill;
        //            d.ChangeCateGoods = changecategoods;
        //            d.ChangeUnit = changeunit;
        //            d.EditDate = editdate;
        //            d.IdentifyConsultants = identifyconsultants;
        //            d.ConfirmCusInfor = confirmcusinfor;
        //            d.ReturnGoods = returngoods;
        //            db.Roles.Add(d);
        //            db.SaveChanges();
        //            return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception e)
        //        {
        //            return Json(new { code = 500, msg = "Hiểm thị dữ liệu thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    [HttpPost]
        //    public JsonResult EditRole(int idrole,bool admin, bool editdiscountgoods, bool editdiscountbill, bool editpricegoods,
        //                     bool editamountgoods, bool deletegoods, bool printagainbill, bool hangbill, bool changecategoods,
        //                     bool changeunit, bool editdate, bool identifyconsultants, bool confirmcusinfor, bool returngoods)
        //    {
        //        try
        //        {
        //            var d = db.Roles.Find(idrole);
        //            d.Admin = admin;
        //            d.EditDiscountGoods = editdiscountgoods;
        //            d.EditDiscountBill = editdiscountbill;
        //            d.EditPriceGoods = editpricegoods;
        //            d.EditAmountGoods = editamountgoods;
        //            d.DeleteGoods = deletegoods;
        //            d.PrintAgainBill = printagainbill;
        //            d.HangBill = hangbill;
        //            d.ChangeCateGoods = changecategoods;
        //            d.ChangeUnit = changeunit;
        //            d.EditDate = editdate;
        //            d.IdentifyConsultants = identifyconsultants;
        //            d.ConfirmCusInfor = confirmcusinfor;
        //            d.ReturnGoods = returngoods;
        //            db.SaveChanges();
        //            return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception e)
        //        {
        //            return Json(new { code = 500, msg = "Hiểm thị dữ liệu thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}
    }
}