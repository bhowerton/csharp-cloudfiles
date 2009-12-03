using System.IO;
using System.Net;

namespace Rackspace.CloudFiles.unit.tests
{
    internal class OurWebRequest : Request
    {
        private readonly WebRequest request;

        public OurWebRequest(WebRequest request)
        {
            this.request = request;
        }

        public Stream GetRequestStream()
        {
            return request.GetRequestStream();
        }

        public Response GetResponse()
        {
            return new OurWebResponse(request.GetResponse());
        }
    }
}