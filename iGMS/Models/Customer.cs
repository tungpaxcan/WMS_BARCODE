//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
            this.SalesOrders = new HashSet<SalesOrder>();
        }
    
        public string Id { get; set; }
        public string IdUser { get; set; }
        public string IdGroupGoods { get; set; }
        public string IdGeneral { get; set; }
        public string IdInternal { get; set; }
        public string IdTypeCustomer { get; set; }
        public string Name { get; set; }
        public string NameTransaction { get; set; }
        public Nullable<double> Money { get; set; }
        public Nullable<double> Point { get; set; }
        public string AddRess { get; set; }
        public string TaxCode { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Represent { get; set; }
        public string Position { get; set; }
        public string Website { get; set; }
        public string STK1 { get; set; }
        public string Bank1 { get; set; }
        public string STK2 { get; set; }
        public string Bank2 { get; set; }
        public Nullable<double> DebtLimit { get; set; }
        public Nullable<double> NumberDaysOwed { get; set; }
        public Nullable<double> Deposit { get; set; }
        public Nullable<System.DateTime> DatePay { get; set; }
        public Nullable<double> Discount { get; set; }
        public string Description { get; set; }
        public string MembershipCard { get; set; }
        public string CustomerResources { get; set; }
        public string Career { get; set; }
        public string PaymentMethods { get; set; }
        public string AccountingAccountCode { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Type { get; set; }
    
        public virtual GroupGood GroupGood { get; set; }
        public virtual TypeCustomer TypeCustomer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }
    }
}