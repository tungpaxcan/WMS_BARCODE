﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WMSEntities : DbContext
    {
        public WMSEntities()
            : base("name=WMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Backup> Backups { get; set; }
        public virtual DbSet<ChildrenFolder> ChildrenFolders { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<DetailEPC> DetailEPCs { get; set; }
        public virtual DbSet<DetailGoodOrder> DetailGoodOrders { get; set; }
        public virtual DbSet<DetailSaleOrder> DetailSaleOrders { get; set; }
        public virtual DbSet<DetailStockEpc> DetailStockEpcs { get; set; }
        public virtual DbSet<DetailWareHouse> DetailWareHouses { get; set; }
        public virtual DbSet<EPC> EPCs { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Good> Goods { get; set; }
        public virtual DbSet<GroupGood> GroupGoods { get; set; }
        public virtual DbSet<HeadFunction> HeadFunctions { get; set; }
        public virtual DbSet<InventoryNotify> InventoryNotifies { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Method> Methods { get; set; }
        public virtual DbSet<ModelSetting> ModelSettings { get; set; }
        public virtual DbSet<ParamFunction> ParamFunctions { get; set; }
        public virtual DbSet<ParentFolder> ParentFolders { get; set; }
        public virtual DbSet<PurchaseNotify> PurchaseNotifies { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<RecentPage> RecentPages { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleAdmin> RoleAdmins { get; set; }
        public virtual DbSet<SaleOrderNotify> SaleOrderNotifies { get; set; }
        public virtual DbSet<SalesOrder> SalesOrders { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TypeCustomer> TypeCustomers { get; set; }
        public virtual DbSet<TypeStatu> TypeStatus { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WareHouse> WareHouses { get; set; }
        public virtual DbSet<DetailStock> DetailStocks { get; set; }
    }
}
