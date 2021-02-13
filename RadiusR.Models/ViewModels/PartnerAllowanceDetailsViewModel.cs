using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class PartnerAllowanceDetailsViewModel
    {
        [EnumType(typeof(RadiusR.DB.Enums.PartnerAllowanceState), typeof(RadiusR.Localization.Lists.PartnerAllowanceState))]
        [UIHint("LocalizedList")]
        public short AllowanceState { get; private set; }

        [UIHint("Currency")]
        public string AllowanceAmount
        {
            get
            {
                return _allowanceAmount.ToString("###,##0.00");
            }
        }

        private decimal _allowanceAmount { get; set; }

        public PartnerAllowanceDetailsViewModel(RadiusR.DB.Enums.PartnerAllowanceState state, decimal amount)
        {
            AllowanceState = (short)state;
            _allowanceAmount = amount;
        }
    }
}
