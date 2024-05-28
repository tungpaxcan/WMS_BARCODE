using Google.Apis.Http;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net;
using System.Resources;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Windows.Documents;
using WMS.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WMS.Controllers
{
    public class WareHouseController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        //private IHubContext<RealHub> hub;
        // GET: WareHouse
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult ShowList()
        {
            return View();
        }
        public ActionResult AssetShowList() 
        { 
            return View();
        }
        public ActionResult Adds()
        {
            var lastId = db.WareHouses.OrderBy(x => x.Id).ToList().LastOrDefault();
            var idNext = "";
            if (lastId != null)
            {
                idNext = IdNext(lastId.Id);
            }
            ViewBag.idNext = idNext;
            return View();
        }
        public ActionResult Edits(string id)
        {
            if (id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHouse wareHouse = db.WareHouses.Find(id);
            if (wareHouse == null)
            {
                return HttpNotFound();
            }
            return View(wareHouse);
        }
        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.WareHouses.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                             min = b.MinInventory,
                             max = b.MaxInventory,

                         }).ToList().Where(x => x.name.ToLower().Contains(seach));
                var pages = a.Count() % pageSize == 0 ? a.Count() / pageSize : a.Count() / pageSize + 1;
                var c = a.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = a.Count();
                return Json(new { code = 200, c = c, pages = pages, count = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Add(string name, int min, int max)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                var date = DateTime.Now;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(name);
                string base64String = Convert.ToBase64String(bytes);
                var id = base64String + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
                var ids = db.WareHouses.Where(x => x.Id == id).ToList();
                var checkName = db.WareHouses.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

                if (checkName != null)
                {
                    return Json(new { code = 500, msg = rm.GetString("DuplicateName")}, JsonRequestBehavior.AllowGet);
                }
                if (ids.Count == 0)
                {
                    var d = new WareHouse();
                    d.Id = id.Replace("=", "");
                    d.Name = name;
                    d.MinInventory = min;
                    d.MaxInventory = max;
                    db.WareHouses.Add(d);
                    db.SaveChanges();
                    return Json(new { code = 200, msg = rm.GetString("CreateSucess") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 300, msg = rm.GetString("Duplicate") }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //public JsonResult Upload(FormCollection formCollection)
        //{
        //    List<string> logError = new List<string>();
        //    try
        //    {
        //        var session = (ApiAccount)Session["user"];
        //        var nameAdmin = session.UserName;

        //        if (Request != null)
        //        {
        //            HttpPostedFileBase file = Request.Files["UploadedFile"];
        //            if (file.ContentLength == 0)
        //            {
        //                return Json(new { status = 500, msg = "Vui Lòng Chọn File" }, JsonRequestBehavior.AllowGet);
        //            }
        //            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //            {
        //                string fileName = file.FileName;
        //                string fileContentType = file.ContentType;
        //                byte[] fileBytes = new byte[file.ContentLength];
        //                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
        //                using (var package = new ExcelPackage(file.InputStream))
        //                {
        //                    ExcelWorksheet currentSheet = package.Workbook.Worksheets.First();
        //                    var workSheet = currentSheet;
        //                    var noOfCol = workSheet.Dimension.End.Column;
        //                    var noOfRow = workSheet.Dimension.End.Row;
        //                    List<WareHouse> warehouseList = new List<WareHouse>();
        //                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //                    {
        //                        var NameWareHouse = workSheet.Cells[rowIterator, 1].Value == null ? "" : workSheet.Cells[rowIterator, 1].Value.ToString();
        //                        var Min = workSheet.Cells[rowIterator, 2].Value == null ? "" : workSheet.Cells[rowIterator, 2].Value.ToString();
        //                        var Max = workSheet.Cells[rowIterator, 3].Value == null ? "" : workSheet.Cells[rowIterator, 3].Value.ToString();

        //                        var date = DateTime.Now;
        //                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(NameWareHouse);
        //                        string base64String = Convert.ToBase64String(bytes);
        //                        var id = base64String + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
        //                        var ids = db.WareHouses.Where(x => x.Id == id).ToList();
        //                        var checkNameExcel = warehouseList.FirstOrDefault(x => x.Name.ToLower() == NameWareHouse.ToLower());
        //                        var checkNameWareHouse = db.WareHouses.FirstOrDefault(x => x.Name.ToLower() == NameWareHouse.ToLower());
        //                        if(checkNameWareHouse != null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Tên Kho Hàng Ở Dòng {rowIterator} Đã Có Trong Hệ Thống" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (string.IsNullOrEmpty(NameWareHouse))
        //                        {
        //                            return Json(new { status = 500, msg = $"Tên Kho Hàng Ở Dòng {rowIterator} Không Được Bỏ Trống" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (checkNameExcel != null)
        //                        {
        //                            return Json(new { status = 500, msg = $"Tên Kho Hàng Ở Dòng {rowIterator} Bị Trùng Với Dòng {checkNameExcel.Line} " }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if(int.Parse(Min) >= int.Parse(Max))
        //                        {
        //                            return Json(new { status = 500, msg = $"Số Lượng Tồn Thấp Nhất Ở Dòng {rowIterator} Không Được Lớn Hơn Số Lượng Tồn Cao Nhất" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                        if (ids.Count == 0)
        //                        {
        //                            var d = new WareHouse()
        //                            {
        //                                Id = id.Replace("=", ""),
        //                                Name = NameWareHouse,
        //                                MinInventory = int.Parse(Min),
        //                                MaxInventory = int.Parse(Max),
        //                                Line = rowIterator,
        //                            };
        //                            warehouseList.Add(d);
        //                        }
        //                        else
        //                        {
        //                            return Json(new { status = 500, msg = $"Trùng Mã Nhóm Hàng" }, JsonRequestBehavior.AllowGet);
        //                        }
        //                    }
        //                    db.WareHouses.AddRange(warehouseList);
        //                    db.SaveChanges();
        //                }
        //            }
        //        }
        //        return Json(new { status = 200, logError, msg = "Thành công " }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { status = 500, logError, msg = "Thất bại" + e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [HttpPost]
        public JsonResult Edit(string id, string name, int min, int max)
        {
            try
            {
                var session = (ApiAccount)Session["user"];
                var nameAdmin = session.UserName;
                var d = db.WareHouses.Find(id);
                d.Id = id;
                d.Name = name;
                d.MinInventory = min;
                d.MaxInventory = max;
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("SucessEdit").ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult WareHouseSearch(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var c = (from b in db.WareHouses
                                  where b.Name.Contains(name)
                                  select new
                                  {
                                      idwarehouse = b.Id,
                                      nameWarehouse = b.Name,
                                  }).ToList();

                    return Json(new { code = 200, c  }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("error") }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DetailWareHouse(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var c = (from b in db.WareHouses
                             where b.Id == id
                             select new
                             {
                                 idwarehouse = b.Id,
                                 nameWarehouse = b.Name,
                             }).ToList();
                    return Json(new { code = 200, c }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = rm.GetString("error") }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            var session = (ApiAccount)Session["user"];
            try
            {
                var d = db.WareHouses.Find(id);
                db.WareHouses.Remove(d);
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("SucessDelete") }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception)
            {
                return Json(new { code = 500, msg = rm.GetString("FailDelete") }, JsonRequestBehavior.AllowGet);
            }
        }
        public string IdNext(string id)
        {

            char lastChar = id[id.Length - 1];

            var position = GetAlphabetPosition(lastChar);
            string newId = "";
            if (position == -1)
            {
                var numNext = int.Parse(lastChar.ToString()) + 1;
                if (numNext - 1 == 9)
                {
                    newId = id.Substring(0, id.Length) + "0";
                }
                else
                {
                    newId = id.Substring(0, id.Length - 1) + numNext.ToString();
                }
                return newId;
            }
            else
            {
                char character = (char)('A' + position);

                if (id.Length > 0)
                {
                    if (character.ToString() == "[")
                    {
                        newId = id.Substring(0, id.Length) + "A";
                    }
                    else
                    {
                        newId = id.Substring(0, id.Length - 1) + character.ToString();
                    }
                }
                return newId;
            }

        }
        static int GetAlphabetPosition(char character)
        {
            // Chuyển đổi ký tự thành chữ cái in hoa để đảm bảo tính nhất quán
            char upperCaseChar = char.ToUpper(character);

            // Kiểm tra xem ký tự có thuộc bảng chữ cái tiếng Anh không
            if (upperCaseChar < 'A' || upperCaseChar > 'Z')
            {
                return -1; // Ký tự không thuộc bảng chữ cái tiếng Anh
            }

            // Tính vị trí của ký tự trong bảng chữ cái
            int position = upperCaseChar - 'A' + 1;

            return position;
        }

        [HttpGet]
        public JsonResult ShowDetailWareHouse(string idwarehouse, string idgoods)
        {
            try
            {
                var list = db.DetailWareHouses
                    .Where(b => b.IdWareHouse.Contains(idwarehouse))
                    .GroupBy(b => new { b.IdWareHouse, b.WareHouse.Name })
                    .Select(g => new
                    {
                        idwarehouse = g.Key.IdWareHouse,
                        namewarehouse = g.Key.Name,
                        count = g.Sum(item => item.Inventory),
                        idgoods = g.Select(item => item.IdGoods).ToList()
                    }).ToList();

                var goods = (from p in db.DetailWareHouses
                             where p.IdWareHouse == idwarehouse
                             select new
                             {

                                 idgoods = p.IdGoods,
                                 name = p.Good.Name,
                                 qtt = p.Inventory,
                             }).ToList();
                var epcInGoods = (from e in db.DetailWareHouses
                                  join d in db.EPCs on new { e.IdGoods, e.IdWareHouse } equals new { d.IdGoods, d.IdWareHouse }
                                  where e.IdWareHouse == idwarehouse && e.IdGoods==idgoods
                                  select new
                                   {
                                       idgoods = e.IdGoods,
                                       namewarehouse = e.WareHouse.Name,
                                       idEPC = d.IdEPC,
                                   }).ToList();

                return Json(new { code = 200, list, goods, epcInGoods }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = rm.GetString("error") + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult InventoryCheck()
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                var username = user.UserName;
                var warehouses = db.WareHouses.ToList();
                var notifications = db.InventoryNotifies.ToList();
                const int CONDITIONNOTIFY = 2;
                foreach (var warehouse in warehouses)
                {
                    var warehouseId = warehouse.Id;
                    var warehouseName = warehouse.Name;
                    var minInventory = warehouse.MinInventory;

                    //th1: tổng tồn kho trong kho hàng
                    var totalInventory = db.DetailWareHouses.Where(x => x.IdWareHouse == warehouseId).Sum(x => x.Inventory);

                    // Kiểm tra xem thông báo đã tồn tại trong StockNotify chưa
                    var existNotify = notifications.FirstOrDefault(n => n.Warehouse == warehouseName && n.Goods == null && n.UserName == username);

                    if (totalInventory - minInventory <= CONDITIONNOTIFY)
                    {
                        if (existNotify == null)
                        {
                            // Tạo thông báo mới nếu chưa có trong StockNotify và tổng tồn kho không đủ
                            var newNotify = new InventoryNotify
                            {
                                IdWareHouse = warehouseId,
                                Warehouse = warehouseName,
                                Message = $"{rm.GetString("Tổng Lượng Tồn Hàng Hóa")} {rm.GetString("Trong")} {warehouseName} {rm.GetString("Còn")} {totalInventory}. ",
                                Status = true,
                                CreateDate = DateTime.Now,
                                NewInventory = totalInventory,
                                UserName = username
                            };

                            notifications.Add(newNotify);
                            db.InventoryNotifies.Add(newNotify);
                        }
                        else
                        {
                            existNotify.NewInventory = totalInventory;
                            existNotify.Message = $"{rm.GetString("Tổng Lượng Tồn Hàng Hóa")} {rm.GetString("Trong")} {warehouseName} {rm.GetString("Còn")} {totalInventory}. ";

                        }
                    }
                    else
                    {
                        if (existNotify != null)
                        {
                            // Xóa thông báo trong StockNotify nếu tổng tồn kho đã đủ và thông báo đã tồn tại
                            db.InventoryNotifies.Remove(existNotify);
                            notifications.Remove(existNotify);
                        }
                    }

                    //TH2: Kiểm tra và cập nhật thông báo cho từng sản phẩm trong kho

                    var goodsInWarehouse = db.DetailWareHouses.Where(x => x.IdWareHouse == warehouseId).ToList();
                    foreach (var goods in goodsInWarehouse)
                    {
                        var namegoods = goods.Good.Name;
                        var idgoods = goods.IdGoods;
                        var goodsInventory = goods.Inventory;

                        var existGoodsNotify = notifications.FirstOrDefault(n => n.Warehouse == warehouseName && n.Goods == namegoods && n.UserName== username);
                        if(goodsInventory <= 2)
                        {
                            if (existGoodsNotify == null)
                            {
                                // Tạo thông báo mới nếu chưa có trong StockNotify
                                var newGoodsNotify = new InventoryNotify
                                {
                                    IdWareHouse = warehouseId,
                                    Warehouse = warehouseName,
                                    IdGoods = idgoods,
                                    Goods = namegoods,
                                    NewInventory = goodsInventory,
                                    Message = $"{rm.GetString("Lượng Tồn Của Hàng Hóa")} {namegoods} {rm.GetString("Trong")} {warehouseName} {rm.GetString("Còn")} {goodsInventory}. ",
                                    Status = true,
                                    CreateDate = DateTime.Now,
                                    UserName = username,
                                };

                                notifications.Add(newGoodsNotify);
                                db.InventoryNotifies.Add(newGoodsNotify);
                            }
                            else
                            {

                                // Cập nhật thông báo cho sản phẩm nếu đã có trong StockNotify
                                existGoodsNotify.NewInventory = goodsInventory;
                                existGoodsNotify.Message = $"{rm.GetString("Lượng Tồn Của Hàng Hóa")} {namegoods} {rm.GetString("Trong")} {warehouseName} {rm.GetString("Còn")} {goodsInventory}.";

                            }
                        }
                        else
                        {
                            if(existGoodsNotify != null)
                            {
                                // Xóa thông báo trong StockNotify
                                db.InventoryNotifies.Remove(existGoodsNotify);
                                notifications.Remove(existGoodsNotify);
                            }
                        }  
                    }
                }

                db.SaveChanges();
                // hiển thị số lượng thông tin có trạng thái là true
                var numNotify = db.InventoryNotifies.Where(x => x.Status == true && x.UserName == username).Count();

                // sắp xếp thông tin mới nhất lên đầu danh sách thông báo
                var updatedNotify = db.InventoryNotifies.Where(n =>n.UserName==username).ToList();

                return Json(new {code= 200, updatedNotify, numNotify }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("error") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult InventoryNotifyClicked(string message)
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                var username = user.UserName;

                foreach (var stockNotify in db.InventoryNotifies.Where(x => x.Status == true && x.UserName==username))
                {
                    if (stockNotify.Message.Trim() == message.Trim())
                    {
                        stockNotify.Status = false;
                    }
                }

                db.SaveChanges();

                var numNotify = db.InventoryNotifies.Where(x => x.Status == true && x.UserName == username).Count();
                return Json(new { code = 200, numNotify }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = rm.GetString("error") + ex.Message });
            }
        }

    }
}