using System;
using System.Net;

namespace Rackspace.CloudFiles.exceptions
{
    public class InvalidResponseCodeException:Exception
    {
        public InvalidResponseCodeException(HttpStatusCode code ):base("The response code was "+ code)
        {
        }
    }
}