using System;
using System.IO;
using System.Net;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.Cloudfiles.Response;
using Rackspace.Cloudfiles.Response.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class AuthenticatedRequest :IAuthenticatedRequest
    {
        private readonly string _storageurl;
        private readonly string _cdnmanagmenturl;
        private readonly string _authtoken;
        private Stream _stream;

        public AuthenticatedRequest(string storageurl, string cdnmanagmenturl, string authtoken)
        {
            Headers = new WebHeaderCollection();
            _storageurl = storageurl;
            _cdnmanagmenturl = cdnmanagmenturl;
            _authtoken = authtoken;
        }

        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl)
        {
            return ResponseBuild(_storageurl, appendtostorageurl);

        }
        private ICloudFilesResponse ResponseBuild(string baseurl, string appendurl)
        {
            var request = (HttpWebRequest)WebRequest.Create(baseurl + appendurl);

            request.Headers = Headers;
            request.Method = Method.GetDescription();
            request.Headers.Add(Constants.X_AUTH_TOKEN, _authtoken);
            request.ContentType = ContentType;
            request.AllowWriteStreamBuffering = AllowWriteStreamBuffering;
            if (_stream == null)
            {
            }
            else
            {
                using (var webstream = request.GetRequestStream())
                {
                    var buffer = new byte[Constants.CHUNK_SIZE];

                    var amt = 0;
                    while ((amt = _stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        webstream.Write(buffer, 0, amt);
                    }

                    _stream.Close();
                    webstream.Flush();
                }
            }
            var response = (HttpWebResponse)request.GetResponse();
            return new CloudFilesResponse(response);
        }
        public HttpVerb Method
        {
            get; set;
        }

        public string ContentType
        {
            get; set;
        }

        public WebHeaderCollection Headers
        {
            get; set;
        }

        public bool AllowWriteStreamBuffering
        {
            set; get;
        }

        public void SetContent(Stream stream)
        {
            _stream = stream;
        }

        public ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl)
        {
            return ResponseBuild(_cdnmanagmenturl, appendtocdnurl);
        }
    }
}