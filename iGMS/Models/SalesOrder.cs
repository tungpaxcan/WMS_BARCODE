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
    
    public partial class SalesOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesOrder()
        {
            this.Deliveries = new HashSet<Delivery>();
            this.DetailSaleOrders = new HashSet<DetailSaleOrder>();
        }
    
        public string Id { get; set; }
        public Nullable<int> IdTypeStatus { get; set; }
        public string IdWareHouse { get; set; }
        public string IdCustomer { get; set; }
        public string IdUser { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string PO { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Delivery> Deliveries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetailSaleOrder> DetailSaleOrders { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
