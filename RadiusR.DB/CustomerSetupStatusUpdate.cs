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
    
    public partial class CustomerSetupStatusUpdate
    {
        public long ID { get; set; }
        public long CustomerSetupTaskID { get; set; }
        public short FaultCode { get; set; }
        public string Description { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.DateTime> ReservationDate { get; set; }
    
        public virtual CustomerSetupTask CustomerSetupTask { get; set; }
    }
}
