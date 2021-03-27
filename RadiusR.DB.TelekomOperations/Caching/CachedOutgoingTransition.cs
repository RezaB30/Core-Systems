using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations.Caching
{
    public class CachedOutgoingTransition
    {
        internal CachedOutgoingTransition(int domainID, DateTime? creationDate, long transactionID, string xdslNo, string counterpartOperator, IndividualCustomerInfo individualInfo, CorporateCustomerInfo corporateInfo)
        {
            DomainID = domainID;
            CreationDate = creationDate;
            TransactionID = transactionID;
            XDSLNo = xdslNo;
            CounterpartOperator = counterpartOperator;
            IndividualInfo = individualInfo;
            CorporateInfo = corporateInfo;
        }

        public int DomainID { get; set; }

        public DateTime? CreationDate { get; private set; }

        public long TransactionID { get; private set; }

        public string XDSLNo { get; private set; }

        public string CounterpartOperator { get; private set; }

        public IndividualCustomerInfo IndividualInfo { get; private set; }

        public CorporateCustomerInfo CorporateInfo { get; private set; }

        public class IndividualCustomerInfo
        {
            internal IndividualCustomerInfo(string firstName, string lastName, string tckNo)
            {
                FirstName = firstName;
                LastName = lastName;
                TCKNo = tckNo;
            }

            public string FirstName { get; private set; }

            public string LastName { get; private set; }

            public string TCKNo { get; private set; }
        }

        public class CorporateCustomerInfo
        {
            internal CorporateCustomerInfo(string companyTitle, string tckNo, string taxNo)
            {
                CompanyTitle = companyTitle;
                TCKNo = tckNo;
                TaxNo = taxNo;
            }

            public string CompanyTitle { get; private set; }

            public string TCKNo { get; private set; }

            public string TaxNo { get; private set; }
        }
    }
}
