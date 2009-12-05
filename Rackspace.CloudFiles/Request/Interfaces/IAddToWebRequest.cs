using System;
using System.Net;
using Rackspace.CloudFiles.Request.Interfaces;

namespace Rackspace.CloudFiles.domain.request.Interfaces
{
    public interface IAddToWebRequest
    {
         Uri CreateUri();
         void Apply(ICloudFilesRequest request);
    }
}