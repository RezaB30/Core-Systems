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
    
    public partial class PartnerCredit
    {
        public long ID { get; set; }
        public int PartnerID { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<long> BillID { get; set; }
        public string Details { get; set; }
    
        public virtual Bill Bill { get; set; }
        public virtual Partner Partner { get; set; }
    }
}
