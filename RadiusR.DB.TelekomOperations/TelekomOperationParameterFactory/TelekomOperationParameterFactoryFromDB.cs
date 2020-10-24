using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.TurkTelekom.WebServices.TTApplication;
using RezaB.TurkTelekom.WebServices;
using RadiusR.DB.Enums.TelekomOperations;

namespace RadiusR.DB.TelekomOperations
{
    //public static partial class TelekomOperationParameterFactory
    //{
    //    public static ApplicationTicket GetXDSLApplicationTicketFromDB(TelekomWorkOrder workOrder)
    //    {
    //        var parsedParameters = InitializeParameters(workOrder);
    //        if (parsedParameters == null)
    //            return null;

    //        return new ApplicationTicket()
    //        {
    //            Address = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationAddress)).ParsedValue,
    //            ApplicationType = (ApplicationType)((ParameterParser.ParsedParameter<int>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationType)).ParsedValue,
    //            BBK = ((ParameterParser.ParsedParameter<long>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationBBK)).ParsedValue,
    //            BuildingCode = ((ParameterParser.ParsedParameter<long>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationBuildingCode)).ParsedValue,
    //            BuildingNo = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationBuildingNo)).ParsedValue,
    //            CityCode = ((ParameterParser.ParsedParameter<long>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationCityCode)).ParsedValue,
    //            ClientFirstName = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationClientFirstName)).ParsedValue,
    //            ClientLastName = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationClientLastName)).ParsedValue,
    //            ClientPhoneNo = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationClientPhoneNo)).ParsedValue,
    //            DistrictName = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationDistrictName)).ParsedValue,
    //            Domain = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationDomain)).ParsedValue,
    //            ISPCode = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationISPCode)).ParsedValue,
    //            PacketCode = ((ParameterParser.ParsedParameter<int>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationPacketCode)).ParsedValue,
    //            Password = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationPassword)).ParsedValue,
    //            ProvinceCode = ((ParameterParser.ParsedParameter<int>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationProvinceCode)).ParsedValue,
    //            Speed = ((ParameterParser.ParsedParameter<int>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationSpeed)).ParsedValue,
    //            StreetName = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationStreetName)).ParsedValue,
    //            TariffCode = ((ParameterParser.ParsedParameter<int>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationTariffCode)).ParsedValue,
    //            Username = ((ParameterParser.ParsedParameter<string>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationUsername)).ParsedValue,
    //            XDSLType = (XDSLType)((ParameterParser.ParsedParameter<int>)parsedParameters.FirstOrDefault(p => p.ParameterType == TelekomWorkOrderParameterType.ApplicationXDSLType)).ParsedValue
    //        };
    //    }
    //}
}
