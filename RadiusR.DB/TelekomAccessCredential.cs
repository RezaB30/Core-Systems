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
    
    public partial class TelekomAccessCredential
    {
        public int DomainID { get; set; }
        public string XDSLWebServiceUsername { get; set; }
        public string XDSLWebServicePassword { get; set; }
        public string XDSLWebServiceCustomerCode { get; set; }
        public string OLOPortalUsername { get; set; }
        public string OLOPortalPassword { get; set; }
        public string OLOPortalCustomerCode { get; set; }
        public string TransitionFTPUsername { get; set; }
        public string TransitionFTPPassword { get; set; }
        public int TransitionOperatorID { get; set; }
    
        public virtual Domain Domain { get; set; }
    }
}
