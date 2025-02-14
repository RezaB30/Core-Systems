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
    
    public partial class FeeTypeCost
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FeeTypeCost()
        {
            this.FeeTypeVariants = new HashSet<FeeTypeVariant>();
            this.TaxRates = new HashSet<TaxRate>();
            this.BillFees = new HashSet<BillFee>();
            this.Fees = new HashSet<Fee>();
            this.RecurringDiscounts = new HashSet<RecurringDiscount>();
            this.SpecialOffers = new HashSet<SpecialOffer>();
        }
    
        public short FeeTypeID { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public bool IsAllTime { get; set; }
        public bool CanBeInitial { get; set; }
        public bool CanBeAdditional { get; set; }
        public bool HasVariants { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FeeTypeVariant> FeeTypeVariants { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaxRate> TaxRates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillFee> BillFees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fee> Fees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecurringDiscount> RecurringDiscounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SpecialOffer> SpecialOffers { get; set; }
    }
}
