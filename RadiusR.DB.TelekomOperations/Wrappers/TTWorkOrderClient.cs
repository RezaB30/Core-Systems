using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations.Wrappers
{
    public class TTWorkOrderClient
    {
        public Caching.CachedTelekomWorkOrder GetWorkOrderState(QueryReadyWorkOrder qrWorkOrder)
        {
            if (qrWorkOrder == null)
            {
                return null;
            }

            var currentDomain = DomainsCache.DomainsCache.GetDomainByID(qrWorkOrder.DomainID);
            if (currentDomain == null || currentDomain.TelekomCredential == null)
                return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown);
            // transition status
            if (qrWorkOrder.OperationType == Enums.TelekomOperations.TelekomOperationType.Transition)
            {
                if (!qrWorkOrder.TransactionID.HasValue || string.IsNullOrWhiteSpace(qrWorkOrder.XDSLNo))
                {
                    return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown);
                }
                var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, qrWorkOrder.TelekomCustomerCode ?? currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                var response = serviceClient.GetTransitionStatus(new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionStatusRequest()
                {
                    TransactionID = qrWorkOrder.TransactionID.Value,
                    XDSLNo = qrWorkOrder.XDSLNo
                });
                if (response.InternalException != null)
                {
                    return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown);
                }
                else
                {
                    RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState status = RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown;
                    string cancellationReason = null;
                    switch (response.Data.Stage)
                    {
                        case RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionStages.WaitingForApproval:
                            status = RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.InProgress;
                            break;
                        case RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionStages.SenderApproved:
                            status = RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Done;
                            break;
                        case RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionStages.CancelledBySender:
                        case RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionStages.CancelledByReceiver:
                            status = RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Cancelled;
                            cancellationReason = $"{response.Data.CancellationCauseName}: {response.Data.CancellationDescription}";
                            break;
                    }
                    return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)status, cancellationReason);
                }
            }
            // registration trace
            else
            {
                if (!qrWorkOrder.ManagementCode.HasValue || !qrWorkOrder.QueueNo.HasValue || !qrWorkOrder.ProvinceCode.HasValue)
                {
                    return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown);
                }
                var serviceClient = new RezaB.TurkTelekom.WebServices.TTApplication.TTApplicationServiceClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, qrWorkOrder.TelekomCustomerCode ?? currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                var response = serviceClient.TraceRegistration(qrWorkOrder.ProvinceCode.Value, qrWorkOrder.ManagementCode.Value, qrWorkOrder.QueueNo.Value);
                if (response.InternalException != null)
                {
                    return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown);
                }
                else
                {
                    return new Caching.CachedTelekomWorkOrder(qrWorkOrder.ID, (short)response.Data.State);
                }
            }
        }
    }
}
