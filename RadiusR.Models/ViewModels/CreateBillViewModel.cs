using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CreateBillViewModel
    {
        public long ClientID { get; set; }

        public string ClientName { get; set; }

        public IEnumerable<AddedFeeViewModel> Fees { get; set; }

        public class AddedFeeViewModel
        {
            public bool IsSelected { get; set; }

            public long ID { get; set; }

            [EnumType(typeof(FeeType), typeof(RadiusR.Localization.Lists.FeeType))]
            [UIHint("LocalizedList")]
            public short FeeTypeID { get; set; }

            public string Description { get; set; }
        }
    }
}