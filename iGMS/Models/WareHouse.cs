
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
    
public partial class WareHouse
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public WareHouse()
    {

        this.Deliveries = new HashSet<Delivery>();

        this.DetailStocks = new HashSet<DetailStock>();

        this.DetailWareHouses = new HashSet<DetailWareHouse>();

        this.EPCs = new HashSet<EPC>();

        this.Goods = new HashSet<Good>();

        this.PurchaseOrders = new HashSet<PurchaseOrder>();

        this.SalesOrders = new HashSet<SalesOrder>();

    }


    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public string CreateBy { get; set; }

    public Nullable<System.DateTime> ModifyDate { get; set; }

    public string ModifyBy { get; set; }

    public Nullable<bool> Status { get; set; }

    public Nullable<int> MinInventory { get; set; }

    public Nullable<int> MaxInventory { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Delivery> Deliveries { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DetailStock> DetailStocks { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DetailWareHouse> DetailWareHouses { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EPC> EPCs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Good> Goods { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<SalesOrder> SalesOrders { get; set; }

}

}
