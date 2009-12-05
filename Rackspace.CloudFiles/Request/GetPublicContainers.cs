///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.Request.Interfaces;

namespace Rackspace.CloudFiles.domain.request
{
    public class GetPublicContainers : IAddToWebRequest
    {
        private readonly string _cdnManagementUrl;

        public GetPublicContainers(string cdnManagementUrl)
        {
            _cdnManagementUrl = cdnManagementUrl;
            if (string.IsNullOrEmpty(cdnManagementUrl))
                throw new ArgumentNullException();
        }

        public Uri CreateUri()
        {
            return  new Uri(_cdnManagementUrl + "?enabled_only=true");
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
            
        }
    }
}