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
    
    public partial class RadiusAuthorization
    {
        public long SubscriptionID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string StaticIP { get; set; }
        public bool IsEnabled { get; set; }
        public string RateLimit { get; set; }
        public string CLID { get; set; }
        public Nullable<System.DateTime> LastLogout { get; set; }
        public Nullable<System.DateTime> LastInterimUpdate { get; set; }
        public Nullable<bool> IsHardQuotaExpired { get; set; }
    
        public virtual Subscription Subscription { get; set; }
    }
}
