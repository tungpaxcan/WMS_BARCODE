using System;
using System.Web;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class tpsController : Controller
    {
        // GET: tps
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult presign(string action,string port_code,DateTime date)
        {
            if(!string.IsNullOrEmpty(port_code)) {
                string filePath = "~/Upload/a.csv";
                var currentUrl = ControllerContext.HttpContext.Request.Url;

                // Kết hợp hostname với đường dẫn file
                string fileUrl = $"{currentUrl.Scheme}://{currentUrl.Host}:{currentUrl.Port}{VirtualPathUtility.ToAbsolute(filePath)}";
                return Json(new { statusCode = 400, body = fileUrl},JsonRequestBehavior.AllowGet);
            }
            return Json(new { statusCode = 200,  }, JsonRequestBehavior.AllowGet);
        }
    }
}