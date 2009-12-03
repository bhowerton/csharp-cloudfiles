using System;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.domain.request
{
    public class GetPublicContainersInformationSerialized: IAddToWebRequest
    {
        private readonly string _cdnManagementurl;
        private readonly Format _format;

        public GetPublicContainersInformationSerialized(string cdnManagementurl, Format format)
        {
            _cdnManagementurl = cdnManagementurl;
            _format = format;
        }

        public Uri CreateUri()
        {
            var endurl = ("?format=" + EnumHelper.GetDescription(_format) + "&enabled_only=true");
            return new Uri(_cdnManagementurl +endurl );
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";

        }
    }
}