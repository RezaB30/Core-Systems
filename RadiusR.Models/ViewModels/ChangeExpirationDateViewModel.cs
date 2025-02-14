﻿using RezaB.Web.Helpers.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ChangeExpirationDateViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ExpirationDate")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [UIHint("ExactTime")]
        public DateWithTime NewDate { get; set; }
    }
}