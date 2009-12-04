using System;
using System.Net;

namespace Rackspace.CloudFiles.domain.request.Interfaces
{
    public interface IAddToWebRequest
    {
         Uri CreateUri();
         void Apply(ICloudFilesRequest request);
    }
}