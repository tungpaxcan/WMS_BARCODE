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
    
    public partial class Stock
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stock()
        {
            this.DetailStocks = new HashSet<DetailStock>();
        }
    
        public string Id { get; set; }
        public string IdGoods { get; set; }
        public string IdWareHouse { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetailStock> DetailStocks { get; set; }
    }
}
