using RadiusR.DB.Enums.TelekomOperations;
using RezaB.TurkTelekom.WebServices.TTApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations
{
    //public static partial class TelekomOperationParameterFactory
    //{
    //    public static IEnumerable<TelekomWorkOrderParameter> GetXDSLApplicationTicketForDB(ApplicationTicket ticket)
    //    {
    //        return new List<TelekomWorkOrderParameter>()
    //        {
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationAddress,
    //                Value = ticket.Address
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationBBK,
    //                Value = Convert.ToString(ticket.BBK)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationBuildingCode,
    //                Value = Convert.ToString(ticket.BuildingCode)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationBuildingNo,
    //                Value = ticket.BuildingNo
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationCityCode,
    //                Value = Convert.ToString(ticket.CityCode)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationClientFirstName,
    //                Value = ticket.ClientFirstName
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationClientLastName,
    //                Value = ticket.ClientLastName
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationClientPhoneNo,
    //                Value = ticket.ClientPhoneNo
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationDistrictName,
    //                Value = ticket.DistrictName
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationDomain,
    //                Value = ticket.Domain
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationISPCode,
    //                Value = ticket.ISPCode
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationPacketCode,
    //                Value = Convert.ToString(ticket.PacketCode)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationPassword,
    //                Value = ticket.Password
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationProvinceCode,
    //                Value = Convert.ToString(ticket.ProvinceCode)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationSpeed,
    //                Value = Convert.ToString(ticket.Speed)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationStreetName,
    //                Value = ticket.StreetName
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationTariffCode,
    //                Value = Convert.ToString(ticket.TariffCode)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationType,
    //                Value = Convert.ToString((int)ticket.ApplicationType)
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationUsername,
    //                Value = ticket.Username
    //            },
    //            new TelekomWorkOrderParameter()
    //            {
    //                ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationXDSLType,
    //                Value = Convert.ToString((int)ticket.XDSLType)
    //            },
    //        };
    //    }
    //}
}
