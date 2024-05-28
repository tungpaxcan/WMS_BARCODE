using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class FunctionOrderController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        // GET: FunctionOrder
        public ActionResult CardRecognition()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetId(string id)
        {
            try
            {
                var getId = db.Goods.Where(x => x.Id.Replace(".","") == id).Count();
                var detailId = db.Goods.OrderBy(x => x.Id.Replace(".", "") == id).ToList().LastOrDefault();
                var barcode = detailId.Id;
                var name = detailId.Name;
                return Json(new { code = 200, getId = getId, barcode= barcode,name=name }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}