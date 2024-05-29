
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
    
public partial class PurchaseOrder
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public PurchaseOrder()
    {

        this.DetailGoodOrders = new HashSet<DetailGoodOrder>();

        this.Receipts = new HashSet<Receipt>();

    }


    public string Id { get; set; }

    public Nullable<int> IdTypeStatus { get; set; }

    public string IdWareHouse { get; set; }

    public string Name { get; set; }

    public Nullable<System.DateTime> DeliveryDate { get; set; }

    public string Description { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public string CreateBy { get; set; }

    public Nullable<System.DateTime> ModifyDate { get; set; }

    public string ModifyBy { get; set; }

    public Nullable<bool> Status { get; set; }

    public string Deliver { get; set; }

    public string IdCustomer { get; set; }



    public virtual Customer Customer { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DetailGoodOrder> DetailGoodOrders { get; set; }

    public virtual TypeStatu TypeStatu { get; set; }

    public virtual WareHouse WareHouse { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Receipt> Receipts { get; set; }

}

}
