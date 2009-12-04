using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.domain.response.Interfaces;

namespace Rackspace.CloudFiles.domain.request
{
    public interface IWebRequestEngine
    {
        ICloudFilesResponse Submit(ICloudFilesRequest request);
       
    }
}