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
    
    public partial class WorkArea
    {
        public long ID { get; set; }
        public long ProvinceID { get; set; }
        public Nullable<long> DistrictID { get; set; }
        public Nullable<long> RuralCode { get; set; }
        public Nullable<long> NeighbourhoodID { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string NeighbourhoodName { get; set; }
    
        public virtual Partner Partner { get; set; }
    }
}
