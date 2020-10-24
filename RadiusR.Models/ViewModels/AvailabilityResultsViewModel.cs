using RezaB.TurkTelekom.WebServices;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AvailabilityResultsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ADSL")]
        public AvailabilityResult ADSL { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "VDSL")]
        public AvailabilityResult VDSL { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Fiber")]
        public AvailabilityResult Fiber { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationHistory")]
        public CancellationHistoryResult CancellationHistory { get; set; }

        public class AvailabilityResult
        {

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SVUID")]
            public string SVUID { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "XDSLType")]
            [EnumType(typeof(XDSLType), typeof(RadiusR.Localization.Lists.XDSLType))]
            [UIHint("LocalizedList")]
            public short XDSLType { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Infrastructure")]
            public bool? HasInfrastructure { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PortState")]
            [EnumType(typeof(RezaB.TurkTelekom.WebServices.Availability.AvailabilityServiceClient.PortState), typeof(RadiusR.Localization.Lists.PortState))]
            [UIHint("LocalizedList")]
            public short PortState { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InfrastructureType")]
            [EnumType(typeof(RezaB.TurkTelekom.WebServices.Availability.AvailabilityServiceClient.InfrastructureType), typeof(RadiusR.Localization.Lists.InfrastructureType))]
            [UIHint("LocalizedList")]
            public short InfrastructureType { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Distance")]
            public int? Distance { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AcceptableDistance")]
            public bool DistanceIsValid { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "HasOpenWorkOrder")]
            public bool? HasOpenRequest { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DSLMaxSpeed")]
            [UIHint("TransferRate")]
            public string DSLMaxSpeed { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Result")]
            [EnumType(typeof(RezaB.TurkTelekom.WebServices.Availability.AvailabilityServiceClient.AvailabilityResult), typeof(RadiusR.Localization.Lists.AvailabilityResult))]
            [UIHint("LocalizedList")]
            public short Result { get; set; }
        }

        public class CancellationHistoryResult
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DaysSinceLastCancellation")]
            public string Days { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ErrorMessage")]
            public string ErrorMessage { get; set; }
        }
    }
}