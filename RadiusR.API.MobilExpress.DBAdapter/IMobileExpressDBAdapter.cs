using RadiusR.API.MobilExpress.DBAdapter.AdapterParameters;
using RezaB.API.MobilExpress.Request;
using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.API.MobilExpress.DBAdapter
{
    public interface IMobileExpressDBAdapter
    {
        MobilExpressSaveCustomerRequest CreateCustomer(Customer dbCustomer);

        MobilExpressSaveCardRequest CreateCard(Customer customer, AdapterCard card);

        MobilExpressGetCardsRequest CreateCardListRequest(Customer customer);

        MobilExpressDeleteCardRequest CreateDeleteCardRequest(Customer customer, string cardToken);

        MobilExpressPayBillRequest CreatePayBillRequest(Customer customer, Bill bill, string cardToken);
    }
}
