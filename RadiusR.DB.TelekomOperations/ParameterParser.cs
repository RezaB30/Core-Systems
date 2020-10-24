using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations
{
    static class ParameterParser
    {
        
        public static ParsedParameter<T> ParseParameter<T>(TelekomWorkOrderParameter parameter)
        {
            var results = new ParsedParameter<T>
            {
                IsParsed = false,
                RawValue = parameter.Value,
                ParameterTypeID = parameter.ParameterCode
            };
            // find parameter type
            if (Enum.IsDefined(typeof(TelekomWorkOrderParameterType), parameter.ParameterCode))
            {
                results.ParameterType = (TelekomWorkOrderParameterType)parameter.ParameterCode;
            }
            else
            {
                results.ParameterType = null;
                return results;
            }
            // parse value
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                if(converter.IsValid(parameter.Value))
                    try
                    {
                        var parsedValue = (T)converter.ConvertFromInvariantString(parameter.Value);
                        results.ParsedValue = parsedValue;
                    }
                    catch (NotSupportedException ex)
                    {
                        return results;
                    }
                results.IsParsed = true;
                return results;
            }

            return results;
        }

        public class ParsedParameter<T> : ParsedParameter
        {
            public new T ParsedValue { get; set; }
        }

        public class ParsedParameter
        {
            public int ParameterTypeID { get; set; }

            public TelekomWorkOrderParameterType? ParameterType { get; set; }

            public string RawValue { get; set; }

            public object ParsedValue { get; set; }

            public bool IsParsed { get; set; }
        }
    }
}
