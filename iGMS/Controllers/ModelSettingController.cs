using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class ModelSettingController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        // GET: ModelSetting
        public ActionResult Index()
        {
            ViewBag.inventorystatus = db.ModelSettings.Find("inventorystatus").Status;
            return View();
        }
        //----Hà Sửa------
        [HttpPost]
        public ActionResult UpdateInventoryStatus(ModelSetting modelSetting)
        {
            try
            {
                var updateStt = db.ModelSettings.Find(modelSetting.Id);
                // Cập nhật trạng thái
                updateStt.Id= modelSetting.Id;
                updateStt.Status = modelSetting.Status;
                
                // Lưu thay đổi.
                db.SaveChanges();
                return Json(new { status = 200, msg = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                // Xử lý nếu có lỗi.
                return Json(new { status = 500, msg = "Cập nhật thất bại " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}