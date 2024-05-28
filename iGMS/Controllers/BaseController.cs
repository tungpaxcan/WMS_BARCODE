using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using System.Threading;
using System.Web.Routing;
using System.Globalization;
using Resources;

namespace WMS.Controllers
{
    public class BaseController : Controller
    {
        private WMSEntities db = new WMSEntities();
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["user"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { controller = "Login", action = "Login" }));
            }
        }
        //Initializing culture on controller Initialization
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session[Common.Currentculture] != null)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session[Common.Currentculture].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session[Common.Currentculture].ToString());

            }
            else
            {
                Session[Common.Currentculture] = "vi";
                Thread.CurrentThread.CurrentCulture = new CultureInfo("vi");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi");
            }
        }
        // change culture
        [HttpPost]
        public ActionResult ChangeCulture(string ddlculture, string returnUrl)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(ddlculture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ddlculture);

            Session[Common.Currentculture] = ddlculture;

            return Json(new { success = true, redirectUrl = returnUrl });
        }

        [HttpGet]
        public JsonResult SignOut()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                Session["user"]=null;
               return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}