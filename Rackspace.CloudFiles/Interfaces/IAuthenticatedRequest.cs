using System.IO;
using System.Net;
using Rackspace.CloudFiles.domain.response.Interfaces;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IAuthenticatedRequest
    {
        ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl);
        HttpVerb Method { get; set; }
        string ContentType { get; set; }
        WebHeaderCollection Headers { get; set; }
        bool AllowWriteStreamBuffering { get; set; }
        void SetContent(Stream stream);
        ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl);
    }
}