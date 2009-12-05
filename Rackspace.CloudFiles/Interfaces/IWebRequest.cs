using System.Collections.Generic;
using Rackspace.CloudFiles.domain;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IWebRequest
    {
        string Url { get; set; }
        string Method { get; set; }
        IDictionary<string,string> Headers { get; }
        HttpProxy WebProxy { get; set; }
        IAuthenticatedRequestFactory Submit();
    }
}