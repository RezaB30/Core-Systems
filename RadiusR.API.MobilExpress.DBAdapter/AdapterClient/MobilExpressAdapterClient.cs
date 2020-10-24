using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB;
using RezaB.API.MobilExpress;
using RadiusR.API.MobilExpress.DBAdapter.AdapterParameters;
using RezaB.API.MobilExpress.Request;
using RadiusR.DB.Utilities.Billing;

namespace RadiusR.API.MobilExpress.DBAdapter.AdapterClient
{
    public class MobilExpressAdapterClient
    {
        private MobilExpressClient InternalClient { get; set; }
        private MobilExpressDBAdapter InternalAdapter { get; set; }

        private ClientConnectionDetails ConnectionDetails { get; set; }

        public MobilExpressAdapterClient(string merchantKey, string apiPassword, ClientConnectionDetails connectionDetails)
        {
            InternalClient = new MobilExpressClient(merchantKey, apiPassword);
            InternalAdapter = new MobilExpressDBAdapter();
            ConnectionDetails = connectionDetails;
        }

        public MobilExpressAdapterClientResponseBase SaveCustomer(Customer customer)
        {
            var request = InternalAdapter.CreateCustomer(customer);
            PrepareRequest(request);
            try
            {
                var response = InternalClient.SaveCustomer(request);
                return new MobilExpressAdapterClientResponseBase()
                {
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new MobilExpressAdapterClientResponseBase()
                {
                    InternalException = ex
                };
            }
        }

        public MobilExpressAdapterClientSaveCardResponse SaveCard(Customer customer, AdapterCard card)
        {
            var request = InternalAdapter.CreateCard(customer, card);
            PrepareRequest(request);
            try
            {
                var response = InternalClient.SaveCard(request);
                return new MobilExpressAdapterClientSaveCardResponse()
                {
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new MobilExpressAdapterClientSaveCardResponse()
                {
                    InternalException = ex
                };
            }
        }

        public MobilExpressAdapterClientGetCardsResponse GetCards(Customer customer)
        {
            var request = InternalAdapter.CreateCardListRequest(customer);
            PrepareRequest(request);
            try
            {
                var response = InternalClient.GetCards(request);
                return new MobilExpressAdapterClientGetCardsResponse()
                {
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new MobilExpressAdapterClientGetCardsResponse()
                {
                    InternalException = ex
                };
            }
        }

        public MobilExpressAdapterClientResponseBase DeleteCard(Customer customer, string cardToken)
        {
            var request = InternalAdapter.CreateDeleteCardRequest(customer, cardToken);
            PrepareRequest(request);
            try
            {
                var response = InternalClient.DeleteCard(request);
                return new MobilExpressAdapterClientResponseBase()
                {
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new MobilExpressAdapterClientResponseBase()
                {
                    InternalException = ex
                };
            }
        }

        public MobilExpressAdapterClientPayBillResponse PayBill(Customer customer, Bill bill, string cardToken)
        {
            return PayBill(customer, bill.GetPayableCost(), cardToken);
        }

        public MobilExpressAdapterClientPayBillResponse PayBill(Customer customer, decimal amount, string cardToken)
        {
            var request = InternalAdapter.CreatePayBillRequest(customer, amount, cardToken);
            PrepareRequest(request);
            try
            {
                var response = InternalClient.PayBill(request);
                return new MobilExpressAdapterClientPayBillResponse()
                {
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new MobilExpressAdapterClientPayBillResponse()
                {
                    InternalException = ex
                };
            }
        }

        private void PrepareRequest(MobilExpressRequestBase request)
        {
            request.ClientIP = ConnectionDetails.IP;
            request.ClientUserAgent = ConnectionDetails.UserAgent;
        }
    }
}
