using System;
using System.IO;
using System.Net;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles.utils
{
    public class HttpReaderWriter : IHttpReaderWriter 
    {
         

        public string GetStringFromStream(HttpWebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            {
                using(var streamreader = new StreamReader(responseStream))
                {
                    return streamreader.ReadToEnd();
                }
            }
        }

        public void WriteResponse(HttpWebResponse response, Stream getFromWeb, Action<long,long> progressevent)
        {
            using (var responseStream = response.GetResponseStream())
            {
                byte[] buffer = new byte[4096];

                int amt = 0;
                long totalread = 0;
                while ((amt = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    totalread += amt;
                    getFromWeb.Write(buffer, 0, amt);
                    progressevent.Invoke(totalread, responseStream.Length);
                }

            }

        }

        public void WriteRequest(HttpWebRequest request, Stream sendToWeb, Action<long,long> progressevent)
        {

            using (var requestStream = request.GetRequestStream())
            {
                byte[] buffer = new byte[4096];

                int amt = 0;
                long totalread = 0;
                while ((amt = sendToWeb.Read(buffer, 0, buffer.Length)) != 0)
                {
                    totalread += amt;
                    requestStream.Write(buffer, 0, amt);
                    progressevent.Invoke(totalread, requestStream.Length);
                }

            }

        }
    }
}