
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
    
public partial class TypeCustomer
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public TypeCustomer()
    {

        this.Customers = new HashSet<Customer>();

    }


    public string Id { get; set; }

    public string Name { get; set; }

    public string Des { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public string CreateBy { get; set; }

    public Nullable<System.DateTime> ModifyDate { get; set; }

    public string ModifyBy { get; set; }

    public Nullable<bool> Status { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Customer> Customers { get; set; }

}

}
