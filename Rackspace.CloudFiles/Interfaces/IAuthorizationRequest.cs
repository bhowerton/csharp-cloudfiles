using System.Collections.Generic;
using System.Net;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IAuthorizationRequest
    {
        string Url { get; set; }
        string Method { get; set; }
        IDictionary<string,string> Headers { get; }
        IWebProxy WebProxy { get; set; }
        IAuthenticatedRequestFactory Submit();
    }
}