using System;
using System.Collections.Generic;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class ProdWebRequest:IWebRequest
    {
        public string Url
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Method
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IDictionary<string, string> Headers
        {
            get { throw new NotImplementedException(); }
        }

        public HttpProxy WebProxy
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IAuthenticatedRequestFactory Submit()
        {
            throw new NotImplementedException();
        }
    }
}