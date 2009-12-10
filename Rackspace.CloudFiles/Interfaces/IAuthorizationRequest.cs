using System.Collections.Generic;
using System.Net;
using Rackspace.CloudFiles.domain;

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