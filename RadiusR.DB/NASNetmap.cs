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
    
    public partial class NASNetmap
    {
        public long ID { get; set; }
        public int NASID { get; set; }
        public string LocalIPSubnet { get; set; }
        public string RealIPSubnet { get; set; }
        public int PortCount { get; set; }
        public bool PreserveLastByte { get; set; }
    
        public virtual NAS NAS { get; set; }
    }
}
