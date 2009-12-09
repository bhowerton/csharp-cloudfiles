using System.Collections.Generic;
using System.IO;
using System.Net;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.Interfaces;

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

        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl)
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

        public void SetContent(Stream stream)
        {
            StreamSet = stream;
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
        public ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl)
        {
            _cdnurlspassed.Add(appendtocdnurl);
            return _response;
        }
    }
}