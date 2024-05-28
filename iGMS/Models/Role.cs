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
    
    public partial class Role
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Role()
        {
            this.Users = new HashSet<User>();
        }
    
        public int Id { get; set; }
        public Nullable<bool> AddWareHouse { get; set; }
        public Nullable<bool> EditWareHouse { get; set; }
        public Nullable<bool> DeleteWareHouse { get; set; }
        public Nullable<bool> AddGoods { get; set; }
        public Nullable<bool> EditGoods { get; set; }
        public Nullable<bool> DeleteGoods { get; set; }
        public Nullable<bool> AddCustomer { get; set; }
        public Nullable<bool> EditCustomer { get; set; }
        public Nullable<bool> DeleteCustomer { get; set; }
        public Nullable<bool> AddTypeCustomer { get; set; }
        public Nullable<bool> EditTypeCustomer { get; set; }
        public Nullable<bool> DeleteTypeCustomer { get; set; }
        public Nullable<bool> AddUser { get; set; }
        public Nullable<bool> EditUser { get; set; }
        public Nullable<bool> EditPassWord { get; set; }
        public Nullable<bool> DeleteUser { get; set; }
        public Nullable<bool> BackUpSQL { get; set; }
        public Nullable<bool> AddGroupGoods { get; set; }
        public Nullable<bool> EditGroupGoods { get; set; }
        public Nullable<bool> DeleteGroupGoods { get; set; }
        public string Username { get; set; }
        public Nullable<bool> EditAuth { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}