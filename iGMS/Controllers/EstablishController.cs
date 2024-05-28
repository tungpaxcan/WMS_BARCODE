using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class EstablishController : BaseController
    {
        private WMSEntities db = new WMSEntities();
        // GET: Establish
        public ActionResult DataManagement()
        {
            try
            {
                // Lấy chuỗi kết nối từ cấu hình web
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["WMSEntities"].ConnectionString;

                if (connectionString.ToLower().StartsWith("metadata="))
                {
                    System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(connectionString);
                    connectionString = efBuilder.ProviderConnectionString;
                }

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                string DatabaseServer = builder.DataSource;
                string DatabaseName = builder.InitialCatalog;
                ViewBag.DatabaseName = DatabaseName;
                ViewBag.DatabaseServer = DatabaseServer;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return null;
            }
           
        }
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Backup";
        [HttpPost]
        public JsonResult BackupDatabase(string serverName, string databaseName, string backupPath,string userName, string password)
        {
            try
            {
                var user = (ApiAccount)Session["user"];
                backupPath = @"D:\Backup\"+ backupPath + "";
                string connectionString = "Data Source="+ serverName + ";Initial Catalog="+ databaseName + ";User ID="+userName+";Password="+password+";";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string backupQuery = $"BACKUP DATABASE [{databaseName}] TO DISK = '{backupPath}'";
                    using (SqlCommand command = new SqlCommand(backupQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        Models.Backup backup = new Models.Backup()
                        {
                            Server = serverName,
                            Database = databaseName,
                            Path = backupPath,
                            CreateDate = DateTime.Now,
                            CreateBy = user.FullName
                        };
                        db.Backups.Add(backup);
                        db.SaveChanges();
                        return Json(new { code = 200, msg = "Đã sao lưu dữ liệu "+databaseName+" tại "+ backupPath + " - Máy chủ: "+serverName+" hoàn thành" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            
            catch (Exception ex)
            {
                return Json(new { code = 500 ,msg = "Lỗi: " +ex.Message}, JsonRequestBehavior.AllowGet);
            }
        } 
        [HttpPost]
        public JsonResult RetoreDatabase(int id, string userName, string password)
        {
            try
            {
                var backup = db.Backups.Find(id);
                string serverName = backup.Server;
                string databaseName = backup.Database;
                string backupPath = backup.Path;
                string connectionString = "Data Source="+serverName+";Initial Catalog="+databaseName+";User ID="+userName+";Password="+password+";";
                backupPath = @"" + backupPath + "";

                string sqlRestore = $"USE master; RESTORE DATABASE [{ databaseName}] FROM DISK = '{backupPath}'WITH REPLACE;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string killConnectionsQuery = $@"
                    USE master;
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";

                    using (SqlCommand useMasterCommand = new SqlCommand(killConnectionsQuery, connection))
                    {

                        useMasterCommand.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(sqlRestore, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                    return Json(new { code = 200, msg = "Đã phục hồi dữ liệu " + databaseName + " tại " + backupPath + " - Máy chủ: " + serverName + " hoàn thành" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500 ,msg = "Lỗi: " +ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult List()
        {
            try
            {
                var backup = (from a in db.Backups.Where(x => x.Id > 0).OrderByDescending(x=>x.CreateDate)
                              select new
                              {
                                  id=a.Id,
                                  date = a.CreateDate,
                                  sever = a.Server,
                                  user = a.CreateBy,
                                  name = a.Database,
                                  path = a.Path
                              }).ToList();
                return Json(new { code = 200, msg ="Thành Công",backup = backup  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var backup = db.Backups.Find(id);
                db.Backups.Remove(backup);
                bool saveFailed;
                do
                {
                    saveFailed = false;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;

                        // Update the values of the entity that failed to save from the store
                        ex.Entries.Single().Reload();
                    }

                } while (saveFailed);
                return Json(new { code = 200, msg = "Thành Công", }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}