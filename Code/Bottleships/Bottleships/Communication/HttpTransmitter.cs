using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bottleships.Communication
{
    public class HttpTransmitter
    {
        public string SendMessage(string address, string message, object data)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}", address, message));

            var postData = JsonConvert.SerializeObject(data);
            var byteData = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.Timeout = 30000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteData.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
    }
}
