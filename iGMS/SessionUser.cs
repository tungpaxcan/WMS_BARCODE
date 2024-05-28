using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS
{
    public class SesionUser
    {
        public static bool isSqlDepEventSubscribed = false;
        public bool IsSqlDepEventSubscribed
        {
            get { return isSqlDepEventSubscribed; }
        }
        public void RegisterNotification()
        {
            string conStr = ConfigurationManager.ConnectionStrings["WMSConnected"].ConnectionString;
            string sqlCommand = @"SELECT [RoleAdmin], [Role]
                                     
                                  FROM [dbo].[Users] ";

            //you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                // Đăng ký sự kiện mới

                // Đăng ký sự kiện chỉ khi chưa được đăng ký
                if (!isSqlDepEventSubscribed)
                {
                    sqlDep.OnChange += sqlDep_OnChange;
                    isSqlDepEventSubscribed = true;
                }
                //we must have to execute the command here
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }
        private void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                //from here we will send notification message to client
                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("change");
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;
                isSqlDepEventSubscribed = false;
                //re-register notification
                RegisterNotification();
            }
        }

        public void InventoryNotification()
        {
            string conStr = ConfigurationManager.ConnectionStrings["WMSConnected"].ConnectionString;

            string sqlCommand = @"SELECT d.[Inventory], w.[Inventory]
                                FROM [dbo].[DetailWarehouse] d
            INNER JOIN [dbo].[Goods] w ON d.[IdGoods] = w.[Id]";

            //you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChangeInventory;
                isSqlDepEventSubscribed = true;
                //we must have to execute the command here
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }
        private void sqlDep_OnChangeInventory(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                //from here we will send notification message to client
                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("Inventory");
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChangeInventory;

                InventoryNotification();
            }
        }

        public void PurchaseNotification()
        {
            string conStr = ConfigurationManager.ConnectionStrings["WMSConnected"].ConnectionString;

            string sqlCommand = @"SELECT p.[Status], d.[Status]
                                FROM [dbo].[PurchaseOrder] p
            INNER JOIN [dbo].[DetailGoodOrder] d ON p.[Id] = d.[IdPurchaseOrder]";

            //you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);

                sqlDep.OnChange += sqlDep_OnChangePurchase;
                isSqlDepEventSubscribed = true;
   
                //we must have to execute the command here
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }
        private void sqlDep_OnChangePurchase(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                //from here we will send notification message to client
                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("purchase");
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChangePurchase;

                PurchaseNotification();
            }
        }

        public void SaleOrderNotification()
        {
            string conStr = ConfigurationManager.ConnectionStrings["WMSConnected"].ConnectionString;

            string sqlCommand = @"SELECT p.[Status], d.[Status]
                                FROM [dbo].[SalesOrder] p
            INNER JOIN [dbo].[DetailSaleOrder] d ON p.[Id] = d.[IdSaleOrder]";

            //you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);

                sqlDep.OnChange += sqlDep_OnChangeSaleOrder;
                isSqlDepEventSubscribed = true;

                //we must have to execute the command here
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }
        private void sqlDep_OnChangeSaleOrder(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {

                //from here we will send notification message to client
                var realHub = GlobalHost.ConnectionManager.GetHubContext<RealHub>();
                realHub.Clients.All.notify("saleorder");
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChangeSaleOrder;
                SaleOrderNotification();
            }
        }
    }
}