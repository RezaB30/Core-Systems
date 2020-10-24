using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Verimor
{
    public class VerimorClient
    {
        public VerimorResponse GetWebPhoneToken(string key, string internalID)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.bulutsantralim.com/webphone_tokens");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"key\":\"" + key + "\"," +
                                  "\"extension\":\"" + internalID + "\"}";

                    streamWriter.Write(json);
                }
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return new VerimorResponse()
                    {
                        IsSuccess = httpResponse.StatusCode == HttpStatusCode.OK,
                        Data = streamReader.ReadToEnd()
                    };
                }
            }
            catch
            {
                return new VerimorResponse()
                {
                    IsSuccess = false
                };
            }
        }

        public VerimorResponse CallNumber(string key, string internalID, string number)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.bulutsantralim.com/originate?key=" + key + "&extension=" + internalID + "&destination=" + number + "");
                httpWebRequest.Method = "POST";

                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return new VerimorResponse()
                    {
                        IsSuccess = httpResponse.StatusCode == HttpStatusCode.OK,
                        Data = streamReader.ReadToEnd()
                    };
                }
            }
            catch
            {
                return new VerimorResponse()
                {
                    IsSuccess = false
                };
            }
        }
    }
}
