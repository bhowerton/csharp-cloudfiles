using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Moq;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.Cloudfiles.Response.Interfaces;

namespace Rackspace.CloudFiles.Specs.Utils
{
    public class MockAuthenticatedRequest:IAuthenticatedRequest
    {
        private readonly ICloudFilesResponse _response;
        private WebHeaderCollection _headers = new WebHeaderCollection() ;
        private IList<string> _cdnurlspassed = new List<string>( );
        private IList<string> _storageurlsPassed = new List<string>();

        public MockAuthenticatedRequest(ICloudFilesResponse response)
        {
            _response = response;
        }

        public ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl, Action<HttpWebRequest> attachtorequest, Action<HttpWebResponse> getresponsestream)
        {
            return _response;
        }

        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl)
        {
            _storageurlsPassed.Add(appendtostorageurl);
            return _response;
        }

        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl, 
            Action<HttpWebRequest> attachtorequest, 
            Action<HttpWebResponse> getresponsestream)
        {
            _storageurlsPassed.Add(appendtostorageurl);
            attachtorequest.Invoke(It.IsAny<HttpWebRequest>());
            getresponsestream.Invoke(It.IsAny<HttpWebResponse>());
            return _response;
        }

        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl, Stream attachStream)
        {
            return _response;
        }

        public HttpVerb Method
        {
            set; get;
        }

        public string ContentType
        {
            set; get;
        }

        public WebHeaderCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        public bool AllowWriteStreamBuffering
        {
            set; get;
        }

        public bool Chunked
        {
            set; get;
        }

    
        public IList<string> Cdnurlspassed
        {
            get { return _cdnurlspassed; }
        }
        public IList<string> StorageUrlsPassed
        {
            get { return _storageurlsPassed; }
        }
        public Stream StreamSet
        {
            get; private set;
        }

        public string Etag
        {
            get; set;
        }

        public ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl)
        {
            _cdnurlspassed.Add(appendtocdnurl);
            return _response;
        }

        public ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl, Stream attachstream)
        {
            _cdnurlspassed.Add(appendtocdnurl);
            return _response;
        }
    }
}