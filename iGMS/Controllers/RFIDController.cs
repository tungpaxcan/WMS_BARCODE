using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class RFIDController : Controller
    {
        private WMSEntities db = new WMSEntities();
        // GET: RFID
        //------------------RFID---------------
        [HttpGet]
        public JsonResult AllShowEPC()
        {
            try
            {
                var a = (from b in db.DetailEPCs.Where(x => x.Status==true)
                         select new
                         {
                             id = b.IdEPC,
                         }).ToList();
                return Json(new { code = 200, a = a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
      
        [HttpPost]
        public JsonResult falseEPC(string epc)
        {
            try
            {
                if (epc != null)
                {
                    var deepc = db.DetailEPCs.Find(epc);
                    deepc.Status = false;
                    db.SaveChanges();
                }
                return Json(new { code = 200,epc }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CompareReceiptMC33(string epc)
        {
            try
            {
                var a = (from b in db.EPCs.Where(x => x.IdEPC == epc && x.Status == false)
                         select new
                         {
                             idgood = b.IdGoods
                         }).ToList();
                return Json(new { code = 200, a = a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Refresh()
        {
            try
            {
                var a = db.DetailEPCs.Where(x => x.Status == false).ToList();
                for (int i = 0; i < a.Count(); i++)
                {
                    var b = db.DetailEPCs.OrderBy(x => x.Status == false).ToList().LastOrDefault();
                    db.DetailEPCs.Remove(b);
                    db.SaveChanges();
                }
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Refresh1()
        {
            try
            {
                var a = db.DetailEPCs.Where(x => x.Status != null).ToList();
                for (int i = 0; i < a.Count(); i++)
                {
                    var b = db.DetailEPCs.OrderBy(x => x.Status != null).ToList().LastOrDefault();
                    db.DetailEPCs.Remove(b);
                    db.SaveChanges();
                }
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult StatusEPC(string epc)
        {
            try
            {
                var c = db.DetailEPCs.SingleOrDefault(x => x.IdEPC == epc && x.Status == true);
                if (c != null)
                {
                    c.Status = false;
                    db.SaveChanges();
                }
                else
                {

                }                   
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteEPC()
        {
            try
            {
                var b = db.DetailEPCs.ToList();
                    db.DetailEPCs.RemoveRange(b);
                    db.SaveChanges();
                
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteId(string epc)
        {
            try
            {
                var b = db.DetailEPCs.SingleOrDefault(x => x.IdEPC == epc);
                db.DetailEPCs.Remove(b);
                db.SaveChanges();
                return Json(new { code = 200}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public string Post(Root[] root)
        {
            try
            {
                foreach (var tag in root)
                {
                    DetailEPC t = new DetailEPC
                    {
                        IdEPC = tag.data.idHex,
                        //FXConnect = tag.data.userDefined,
                        Status = true,
                    };

                    if (!db.DetailEPCs.Any(x => x.IdEPC.Equals(tag.data.idHex)))
                        db.DetailEPCs.Add(t);
                    db.SaveChanges();
                }

                return "";
            }
            catch (Exception e)
            {

                return e.Message+e.InnerException;
            }
        }
        [HttpPost]
        public JsonResult getBarcodeByEPC(string[] tags)
        
        {
            List<string> lstBar = new List<string>();
            try
            {
                foreach (var tag in tags)
                {
                    var barcode = db.EPCs.FirstOrDefault(b => b.IdEPC == tag);
                    if(barcode != null)
                    {
                        if (!lstBar.Contains(barcode.IdGoods))
                        {
                            lstBar.Add(barcode.IdGoods);
                        }
                       
                    }
                 
                }
                return Json(new { code = 200, barcode = lstBar}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult checkRFID(string[] tags)
        {
            List<EPC> lstEPC = new List<EPC>();
            try
            {
                foreach (var tag in tags)
                {
                    var UnPaidECP = db.EPCs.FirstOrDefault(e => e.IdEPC == tag);
                    if (UnPaidECP != null)
                    {
                        if ((bool)UnPaidECP.Status)
                            lstEPC.Add(UnPaidECP);
                    }

                }
                var unpaids = lstEPC.Select(x => new { x.IdEPC }).ToList();
                return Json(new { unpaids }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }

}