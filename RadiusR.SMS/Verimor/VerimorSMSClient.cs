using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RadiusR.SMS.Verimor
{
    public class VerimorSMSClient
    {
        public VerimorSMSResponse SendSMS(string username, string password, string title, string message, string[] phoneNos)
        {
            var results = new VerimorSMSResponse();
            // add 90 to phone nos
            for (int i = 0; i < phoneNos.Count(); i++)
            {
                phoneNos[i] = "90" + phoneNos[i];
            }
            // create JSON string
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var JSONString = serializer.Serialize(new
            {
                username = username,
                password = password,
                source_addr = title,
                datacoding = "1",
                valid_for = "00:03",
                messages = new[]
                {
                        new
                        {
                            msg = message,
                            dest = string.Join(",", phoneNos)
                        }
                    }
            });

            // send the request
            var request = (HttpWebRequest)WebRequest.Create("http://sms.verimor.com.tr/v2/send.json");
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Accept = "*/*";

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(JSONString);
            }
            var response = (HttpWebResponse)request.GetResponse();
            results.ResponseCode = (int)response.StatusCode;
            results.WasSuccessful = results.ResponseCode == (int)HttpStatusCode.OK;
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                var errorMessage = stream.ReadToEnd();
                results.ResponseMessage = errorMessage;
            }

            return results;
        }
    }
}
