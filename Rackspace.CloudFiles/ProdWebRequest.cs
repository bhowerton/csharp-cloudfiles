using System;
using System.Collections.Generic;
using System.Net;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class ProdWebRequest:IAuthorizationRequest
    {
        public ProdWebRequest()
        {
            Headers = new Dictionary<string, string>();
        }
        public string Url
        {
            get; set;
        }

        public string Method
        {
            get; set;
        }

        public IDictionary<string, string> Headers
        {
            get; private set;
        }

        public IWebProxy WebProxy
        {
            get; set;
        }

        public IAuthenticatedRequestFactory Submit()
        {
            var request =(HttpWebRequest)WebRequest.Create(Url);
            if(this.WebProxy!=null)
            {
                request.Proxy = this.WebProxy;

            }
            foreach (var header in Headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            request.Method = Method;

            var response = (HttpWebResponse) request.GetResponse();
            var storageurl = response.Headers[Constants.X_STORAGE_URL];
            var cdnmanagmenturl = response.Headers[Constants.X_CDN_MANAGEMENT_URL];
            var authtoken = response.Headers[Constants.X_AUTH_TOKEN];
            var factory = new AuthenticatedRequestFactory(storageurl, cdnmanagmenturl, authtoken);
            return factory;
           
        }
    }
}