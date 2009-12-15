using System;
using System.IO;
using System.Net;
using Rackspace.Cloudfiles.Response.Interfaces;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IAuthenticatedRequest
    {
        ICloudFilesResponse SubmitCdnRequest(string appendtocdrnurl);
        ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl, Action<HttpWebRequest> attachtorequest, Action<HttpWebResponse> getresponsestream);
        ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl);
        ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl, Action<HttpWebRequest> attachtorequest, Action<HttpWebResponse> getresponsestream);
        HttpVerb Method { get; set; }
        string ContentType { get; set; }
        WebHeaderCollection Headers { get; set; }
        bool AllowWriteStreamBuffering { set; get; }
        bool Chunked { set; get; }
    }
}