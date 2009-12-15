using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.Cloudfiles.Response;
using Rackspace.Cloudfiles.Response.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class AuthenticatedRequest : IAuthenticatedRequest
    {
        private readonly string _storageurl;
        private readonly string _cdnmanagmenturl;
        private readonly string _authtoken;
      
        public AuthenticatedRequest(string storageurl, string cdnmanagmenturl, string authtoken)
        {
            Headers = new WebHeaderCollection();
            _storageurl = storageurl;
            _cdnmanagmenturl = cdnmanagmenturl;
            _authtoken = authtoken;


        }
        private readonly Action<HttpWebRequest>  NoActionOnWebRequest = x=> { };
        private readonly Action<HttpWebResponse> NoActionOnWebResponse = x => { };

        public ICloudFilesResponse SubmitCdnRequest(string appendtocdrnurl)
        {
            return ResponseBuild(_cdnmanagmenturl, appendtocdrnurl, NoActionOnWebRequest, NoActionOnWebResponse);
        }
        public ICloudFilesResponse SubmitCdnRequest(string appendtocdnurl, Action<HttpWebRequest> attachtorequest, Action<HttpWebResponse> getresponsestream)
        {
            return ResponseBuild(_cdnmanagmenturl, appendtocdnurl, attachtorequest, getresponsestream);
        }
       
        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl)
        {
            return ResponseBuild(_storageurl, appendtostorageurl, NoActionOnWebRequest, NoActionOnWebResponse);
        }
        public ICloudFilesResponse SubmitStorageRequest(string appendtostorageurl, Action<HttpWebRequest> attachtorequest, Action<HttpWebResponse> getresponsestream)
        {
            return ResponseBuild(_storageurl, appendtostorageurl, attachtorequest, getresponsestream);

        }

        
        
        private ICloudFilesResponse ResponseBuild(string baseurl, string appendurl, Action<HttpWebRequest> attachToRequest, Action<HttpWebResponse> getresponsestream)
        {
            var url = baseurl + appendurl;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = Method.GetDescription();
            request.Headers = Headers;
            request.Headers.Add(Constants.X_AUTH_TOKEN, _authtoken);
            if (ContentType != null) request.ContentType = ContentType;
            request.AllowWriteStreamBuffering = AllowWriteStreamBuffering;
            request.SendChunked = Chunked;
            
            attachToRequest.Invoke(request);
           
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException webex)
            {
                response = ((HttpWebResponse)webex.Response);

            }
            getresponsestream.Invoke(response);


            return new CloudFilesResponse(response);
        }
        public HttpVerb Method
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public WebHeaderCollection Headers
        {
            get;
            set;
        }

        public bool AllowWriteStreamBuffering
        {
            set;
            get;
        }

        public bool Chunked
        {
            set; get;
        }
     
       
    }
}