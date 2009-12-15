using System;
using System.IO;
using System.Net;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IHttpReaderWriter
    {
        string GetStringFromStream(HttpWebResponse response);
        void WriteResponse(HttpWebResponse response, Stream getFromWeb, Action<long,long> progressevent);
        void WriteRequest(HttpWebRequest request, Stream sendToWeb, Action<long, long> progressevent);
    }
}