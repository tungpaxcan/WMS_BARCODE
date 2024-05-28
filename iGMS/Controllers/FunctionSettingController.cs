using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Windows.Documents;
using System.Xml;
using WMS.Models;
using static Microsoft.SqlServer.Management.Dmf.ExpressionNodeFunction;

namespace WMS.Controllers
{
    public class FunctionSettingController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        ResourceManager rm = new ResourceManager("WMS.App_GlobalResources.Resource", typeof(Resources.Resource).Assembly);
        // GET: FunctionSetting
        public ActionResult FunctionSetting()
        {
            return View();
        }
        public ActionResult AddFunction(string menuName, string functionName, string functionId)
        {
            var function = db.Functions.FirstOrDefault(f => f.Name == functionName && f.Menu.Name == menuName && f.Id == functionId);
            if (function != null)
            {
                // Gửi thông tin của chức năng đến view
                ViewBag.MenuName = function.Menu.Name;
                ViewBag.FunctionName = function.Name;
                ViewBag.FunctionId = function.Id;
                ViewBag.Method = function.Method;
            }
            return View();
        }
        public ActionResult EditFunction()
        {
            return View();
        }
        // hiển thị dữ liệu lên thanh thư mục
        [HttpPost]
        public JsonResult GetMenu(string id)
        {
            try
            {
                var menu = (from m in db.Menus
                            select new
                            {
                                id = m.Id,
                                name = m.Name,
                                functions = (from f in db.Functions
                                             where f.IdMenu == m.Id
                                             select new
                                             {
                                                 id = f.Id,
                                                 name = f.Name,
                                                 status = f.Status,
                                                 namefunctionapi = f.NameFunctionAPI,                                                
                                                 param =(from p in f.ParamFunctions
                                                          select new
                                                          {
                                                              nameparam = p.Name,
                                                              method = p.Method,
                                                              datatype = p.DataTypes
                                                          }).ToList()
                                             }).ToList()
                            }).ToList();

                return Json(new { code = 200, menu }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Data()
        {
            try
            {
                Data data = new Data();
                data.InitializeData();
                return Json(new
                {
                    code = 200,
                    paramfunction = data.paramfunction,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // cập nhật trạng thái function khi bật tắt thư mục trong trang functionsetting
        [HttpPost]
        public JsonResult UpdateFunctionStatus(string idfunction, bool status)
        {
            try
            {
                var functionToUpdate = db.Functions.FirstOrDefault(f => f.Id == idfunction);

                if (functionToUpdate != null)
                {
                    functionToUpdate.Status = status;
                    db.SaveChanges();

                    return Json(new { code = 200, message = "Cập nhật trạng thái thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, message = "Lỗi" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, message = "Lỗi " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // lưu các cài đặt trong trang add function
        [HttpPost]
        public JsonResult AddFunctionAPI(string functionId, string namefunction,string returnjson, string statusReturnjson, string nameReturnjson, string methodfunction, List<string> nameparam, List<string> datatype)
        {
            try
            {
                // Kiểm tra xem functionId có tồn tại không
                var function = db.Functions.FirstOrDefault(f => f.Id == functionId);

                if (function != null)
                {
                    // Xóa các paramfunction hiện có cho functionId
                    var existingParams = db.ParamFunctions.Where(p => p.IdFunction == functionId);
                    db.ParamFunctions.RemoveRange(existingParams);

                    // Cập nhật tên chức năng
                    function.NameFunctionAPI = namefunction;
                    function.ReturnJson = returnjson;
                    function.StatusReturnJson = statusReturnjson;
                    function.NameReturnJson = nameReturnjson;
                    function.Method = methodfunction;

                    // Lặp qua danh sách đối số và thêm mới
                    if (nameparam != null)
                    {
                        for (int i = 0; i < nameparam.Count; i++)
                        {
                            var paramName = nameparam[i];
                            var paramType = datatype[i];

                            // Tạo mới đối số
                            var newParam = new ParamFunction
                            {
                                Name = paramName,
                                Method = methodfunction,
                                DataTypes = paramType,
                                IdFunction = function.Id,
                            };
                            db.ParamFunctions.Add(newParam);
                        }
                    }
                    

                    db.SaveChanges();

                    return Json(new { code = 200, message = "Cập nhật chức năng thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, message = "Không tồn tại IdFunction" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, message = "Lỗi" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddReturnValue()
        {
            try
            {
                var text = Request.Form["values"];
                var functionId = Request.Form["functionId"];
                var f = (from i in db.Functions
                         where i.Id == functionId
                         select i).FirstOrDefault();
                if(f != null)
                {
                    f.ReturnValue = text;
                    db.SaveChanges();
                    return Json(new { code = 200, text = f.ReturnValue, msg = "Đã lưu thành công" });
                }
                else
                {
                    return Json(new { code = 500, msg = "Lỗi hệ thống, chưa lưu được" });
                }
            }
            catch (Exception e)
            {
                return Json(new {code = 500, msg = e.Message });
            }
        }

        [HttpPost]
        public JsonResult GetReturnValue()
        {
            try
            {
                var functionId = Request.Form["functionId"];
                var getFunction = (from i in db.Functions
                                   where i.Id == functionId
                                   select i).FirstOrDefault();
                if(getFunction != null)
                {
                    return Json(new { code = 200, values = getFunction.ReturnValue });
                }
                else
                {
                    return Json(new { code = 500, msg = "Không tìm thấy function thích hợp!" });
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = e.Message });
            }
        }

        // lưu hostname của api
        [HttpPost]
        public ActionResult SaveApiHostname(string hostname)
        {
            try
            {
                // Lưu hostname vào tệp cấu hình web.config
                string configFilePath = HttpContext.Server.MapPath("~/Web.config");
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilePath);

                var appSettingsNode = xmlDoc.SelectSingleNode("//appSettings");
                var hostnameNode = appSettingsNode.SelectSingleNode("//add[@key='ApiHostname']");
                if (hostnameNode == null)
                {
                    hostnameNode = xmlDoc.CreateElement("add");
                    var attributeKey = xmlDoc.CreateAttribute("key");
                    attributeKey.Value = "ApiHostname";
                    hostnameNode.Attributes.Append(attributeKey);
                    appSettingsNode.AppendChild(hostnameNode);
                }

                var attributeValue = hostnameNode.Attributes["value"];
                if (attributeValue == null)
                {
                    attributeValue = xmlDoc.CreateAttribute("value");
                    hostnameNode.Attributes.Append(attributeValue);
                }
                attributeValue.Value = hostname;

                xmlDoc.Save(configFilePath);

                var users = Session["user"];


                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // hàm lấy thông tin paramfunction để ajax gửi đến url api
        [HttpGet]
        public JsonResult FunctionAPI(string functionId)
            {
                try
                {
                    string apiHostname = ConfigurationManager.AppSettings["ApiHostname"];

                    if (string.IsNullOrEmpty(apiHostname))
                    {
                        return Json(new { code = 500, message = "Lỗi" }, JsonRequestBehavior.AllowGet);
                    }

                    var function = db.ParamFunctions
                        .Where(p => p.IdFunction == functionId)
                        .GroupBy(p => new { p.IdFunction, p.Function.NameFunctionAPI, p.Method, p.Function.ReturnJson, p.Function.StatusReturnJson, p.Function.NameReturnJson })
                        .Select(g => new
                        {
                            functionId = g.Key.IdFunction,
                            functionNameAPI = apiHostname + g.Key.NameFunctionAPI,
                            returnjson=g.Key.ReturnJson,
                            statusreturnjson = g.Key.StatusReturnJson,
                            namereturnjson = g.Key.NameReturnJson,
                            method = g.Key.Method,
                            paramData = g.Select(p => new { nameParam = p.Name, dataType = p.DataTypes }).ToList()
                        })
                        .ToList();

                    return Json(new { code = 200, function }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { code = 500, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

        // hàm lấy status để setting cho trang có sử dụng api
        public ActionResult GetFunctionSettingEnabled(string functionId)
        {
            bool functionSettingEnabled = db.Functions.Any(x => x.Status==true && x.Id == functionId);


            return Content(JsonConvert.SerializeObject(functionSettingEnabled), "application/json");
        }

        [HttpGet]
        public ActionResult GetApiHostname()
        {
            // Đọc giá trị hostname từ Web.config
            string apiHostname = ConfigurationManager.AppSettings["ApiHostname"];

            // Trả về giá trị hostname dưới dạng JSON
            return Json(new { hostname = apiHostname }, JsonRequestBehavior.AllowGet);
        }
        // lấy dữ liệu bảng paramfunction
        [HttpGet]
        public JsonResult GetParamFunctions(string functionId)
        {
            try
            {
                var paramFunctions = (from p in db.ParamFunctions
                                      where p.IdFunction == functionId
                                      select new
                                      {

                                          idfunction = p.IdFunction,
                                          nameparam = p.Name,
                                          returnjson = p.Function.ReturnJson,
                                          statusreturnjson = p.Function.StatusReturnJson,
                                          namereturnjson = p.Function.NameReturnJson,
                                          method = p.Method,
                                          datatype = p.DataTypes,
                                      }).ToList();
                return Json(new { success = true, paramFunctions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddFolderObjectAPI(string functionId, string idfolder, string namefolder, string idchildrent, string namechildrent)
        {
            try
            {
                var function = db.Functions.FirstOrDefault(f => f.Id == functionId);

                if (function != null)
                {
                    var folder = new ParentFolder
                    {
                        Id = idfolder,
                        Name = namefolder,
                        IdFunction = functionId
                    };
                    db.ParentFolders.Add(folder);

                    var childrenFolder = new ChildrenFolder
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdParent = idchildrent,
                        Name = namechildrent, 
                    };
                    db.ChildrenFolders.Add(childrenFolder);
                    db.SaveChanges();
                }

                    return Json(new {code =200, msg="tạo cây thư mục đối tượng thành công"}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { code =500, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}