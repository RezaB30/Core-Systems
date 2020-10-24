//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RadiusR.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class BillFee
    {
        public long ID { get; set; }
        public long BillID { get; set; }
        public Nullable<long> FeeID { get; set; }
        public Nullable<short> FeeTypeID { get; set; }
        public decimal CurrentCost { get; set; }
        public int InstallmentCount { get; set; }
        public Nullable<long> DiscountID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Description { get; set; }
    
        public virtual Discount Discount { get; set; }
        public virtual FeeTypeCost FeeTypeCost { get; set; }
        public virtual Bill Bill { get; set; }
        public virtual Fee Fee { get; set; }
    }
}
