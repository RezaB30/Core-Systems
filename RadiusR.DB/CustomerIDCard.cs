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
    
    public partial class CustomerIDCard
    {
        public long CustomerID { get; set; }
        public short TypeID { get; set; }
        public string TCKNo { get; set; }
        public string PassportNo { get; set; }
        public string VolumeNo { get; set; }
        public string RowNo { get; set; }
        public string PageNo { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Neighbourhood { get; set; }
        public string SerialNo { get; set; }
        public string PlaceOfIssue { get; set; }
        public Nullable<System.DateTime> DateOfIssue { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
