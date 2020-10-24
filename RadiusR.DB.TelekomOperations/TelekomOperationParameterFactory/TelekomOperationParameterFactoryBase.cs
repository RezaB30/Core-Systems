using RadiusR.DB.Enums.TelekomOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations
{
    public static partial class TelekomOperationParameterFactory
    {
        private static IEnumerable<ParameterParser.ParsedParameter> InitializeParameters(TelekomWorkOrder workOrder)
        {
            if (workOrder.TelekomWorkOrderParameters == null)
            {
                return null;
            }
            if (!CheckParameters(workOrder))
                return null;

            var requiredParameters = GetParametersForOperation((TelekomOperationType)workOrder.OperationTypeID, (TelekomOperationSubType)workOrder.OperationSubType);

            var parsedParameters = ParseParameters(workOrder, requiredParameters);
            if (parsedParameters == null)
                return null;

            return parsedParameters;
        }

        private static IEnumerable<ParameterParser.ParsedParameter> ParseParameters(TelekomWorkOrder workOrder, IEnumerable<OperationParameter> requiredParameters)
        {
            var parsedParameters = new List<ParameterParser.ParsedParameter>();
            foreach (var parameter in requiredParameters)
            {
                var currentDBParameter = workOrder.TelekomWorkOrderParameters.FirstOrDefault(p => p.ParameterCode == parameter.ParameterCode);
                var currentParsedParameter = (ParameterParser.ParsedParameter)typeof(ParameterParser).GetMethod("ParseParameter").MakeGenericMethod(new[] { parameter.ParameterType }).Invoke(null, new[] { currentDBParameter.Value });
                if (!currentParsedParameter.IsParsed)
                    return null;
                parsedParameters.Add(currentParsedParameter);
            }

            return parsedParameters;
        }

        private static bool CheckParameters(TelekomWorkOrder workOrder)
        {
            if (!Enum.IsDefined(typeof(TelekomOperationType), workOrder.OperationTypeID) || !Enum.IsDefined(typeof(TelekomOperationSubType), workOrder.OperationSubType))
                return false;
            var availableParameters = workOrder.TelekomWorkOrderParameters.Select(p => p.ParameterCode);
            var requiredParameters = GetParametersForOperation((TelekomOperationType)workOrder.OperationTypeID, (TelekomOperationSubType)workOrder.OperationSubType);
            if (requiredParameters.Select(rp => rp.ParameterCode).Except(availableParameters).Any())
                return false;
            return true;
        }

        private static IEnumerable<OperationParameter> GetParametersForOperation(TelekomOperationType operationType, TelekomOperationSubType operationSubType)
        {
            switch (operationType)
            {
                case TelekomOperationType.Registration:
                    switch (operationSubType)
                    {
                        case TelekomOperationSubType.XDSL:
                            return new OperationParameter[]
                            {
                                new OperationParameter() { ParameterCode = (int)TelekomWorkOrderParameterType.ApplicationType, ParameterType = typeof(int) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationAddress, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationBBK, ParameterType = typeof(long) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationBuildingCode, ParameterType = typeof(long) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationBuildingNo, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationCityCode, ParameterType = typeof(long) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationClientFirstName, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationClientLastName, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationClientPhoneNo, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationDistrictName, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationISPCode, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationPacketCode, ParameterType = typeof(int) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationPassword, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationProvinceCode, ParameterType = typeof(int) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationSpeed, ParameterType = typeof(int) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationStreetName, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationTariffCode, ParameterType = typeof(int) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationUsername, ParameterType = typeof(string) },
                                new OperationParameter() { ParameterCode =  (int)TelekomWorkOrderParameterType.ApplicationXDSLType, ParameterType = typeof(int) }
                            };
                        case TelekomOperationSubType.FTTX:
                            break;
                        case TelekomOperationSubType.XDSL_to_FTTX:
                            break;
                        case TelekomOperationSubType.FTTX_to_XDSL:
                            break;
                        case TelekomOperationSubType.Normal_to_Handicap:
                            break;
                        default:
                            break;
                    }
                    break;
                case TelekomOperationType.Transport:
                    break;
                case TelekomOperationType.TariffChange:
                    break;
                case TelekomOperationType.Cancellation:
                    break;
                default:
                    break;
            }
            return Enumerable.Empty<OperationParameter>();
        }
    }
}
