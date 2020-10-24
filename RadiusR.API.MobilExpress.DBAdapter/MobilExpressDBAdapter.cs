using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.API.MobilExpress.Request;
using RadiusR.DB;
using RadiusR.API.MobilExpress.DBAdapter.AdapterParameters;
using RadiusR.DB.Utilities.Billing;

namespace RadiusR.API.MobilExpress.DBAdapter
{
    public class MobilExpressDBAdapter : IMobileExpressDBAdapter
    {
        public MobilExpressSaveCustomerRequest CreateCustomer(Customer customer)
        {
            return new MobilExpressSaveCustomerRequest()
            {
                CustomerID = customer.ID.ToString(),
                CustomerName = customer.ValidDisplayName,
                Email = customer.Email,
                IsActive = true,
                PhoneNo = customer.ContactPhoneNo,
                TCK_VKN = customer.CustomerType == (short)RadiusR.DB.Enums.CustomerType.Individual ? customer.CustomerIDCard.TCKNo : customer.CorporateCustomerInfo != null ? customer.CorporateCustomerInfo.TaxNo : null,
                ResponseCulture = customer.Culture
            };
        }

        public MobilExpressSaveCardRequest CreateCard(Customer customer, AdapterCard card)
        {
            return new MobilExpressSaveCardRequest()
            {
                CustomerID = customer.ID.ToString(),
                CustomerName = customer.ValidDisplayName,
                Email = customer.Email,
                Phone = customer.ContactPhoneNo,
                CardHolderName = card.CardHolderName,
                CardNumber = card.CardNumber,
                CardMonth = card.CardMonth,
                CardYear = card.CardYear,
                ResponseCulture = customer.Culture
            };
        }

        public MobilExpressGetCardsRequest CreateCardListRequest(Customer customer)
        {
            return new MobilExpressGetCardsRequest()
            {
                CustomerID = customer.ID.ToString(),
                ResponseCulture = customer.Culture
            };
        }

        public MobilExpressDeleteCardRequest CreateDeleteCardRequest(Customer customer, string cardToken)
        {
            return new MobilExpressDeleteCardRequest()
            {
                CustomerID = customer.ID.ToString(),
                CardToken = cardToken,
                ResponseCulture = customer.Culture
            };
        }

        public MobilExpressPayBillRequest CreatePayBillRequest(Customer customer, Bill bill, string cardToken)
        {
            return CreatePayBillRequest(customer, bill.GetPayableCost(), cardToken);
        }

        public MobilExpressPayBillRequest CreatePayBillRequest(Customer customer, decimal amount, string cardToken)
        {
            return new MobilExpressPayBillRequest()
            {
                CardToken = cardToken,
                CustomerID = customer.ID.ToString(),
                Email = customer.Email,
                POSID = MobilExpressSettings.MobilExpressPOSID,
                TotalAmount = amount,
                ResponseCulture = customer.Culture
            };
        }
    }
}
