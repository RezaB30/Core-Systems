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
    
    public partial class SubscriptionGPSCoord
    {
        public long SubscriptionID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    
        public virtual Subscription Subscription { get; set; }
    }
}
