using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS
{
    public class Chat
    {
        string id;
        public static bool isSqlDepEventSubscribed = false;
        public bool IsSqlDepEventSubscribed
        {
            get { return isSqlDepEventSubscribed; }
        }
        public void RegisterNotification(string user)
        {
            //id = user;
            //string conStr = ConfigurationManager.ConnectionStrings["WMSConnected"].ConnectionString;
            //string sqlCommand = @"SELECT [Status], [IdUserReceipt],[Text]
                                     
            //                      FROM [dbo].[ChatMes] WHERE [IdUserReceipt]= @IdUserReceipt OR [IdUserSend] = @IdUserSend";  

            ////you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency
            //using (SqlConnection con = new SqlConnection(conStr))
            //{
            //    SqlCommand cmd = new SqlCommand(sqlCommand, con);

            //    cmd.Parameters.AddWithValue("@IdUserReceipt", user);
            //    cmd.Parameters.AddWithValue("@IdUserSend", user);
            //    if (con.State != System.Data.ConnectionState.Open)
            //    {
            //        con.Open();
            //    }
            //    cmd.Notification = null;
            //    SqlDependency sqlDep = new SqlDependency(cmd);
            //    // Đăng ký sự kiện mới

            //    // Đăng ký sự kiện chỉ khi chưa được đăng ký
            //    if (!isSqlDepEventSubscribed)
            //    {
            //        sqlDep.OnChange += sqlDep_OnChange;
            //        isSqlDepEventSubscribed = true;
            //    }
            //    //we must have to execute the command here
            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        // nothing need to add here now
            //    }
            //}
        }
        private void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                //SqlDependency sqlDep = sender as SqlDependency;
                //sqlDep.OnChange -= RequestsqlDep_OnChange;

                //from here we will send notification message to client
                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("ok");
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;
                isSqlDepEventSubscribed = false;
                //re-register notification
                RegisterNotification(id);
            }
        }
    }
}