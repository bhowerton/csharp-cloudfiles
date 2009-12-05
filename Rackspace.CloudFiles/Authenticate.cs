using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class Authenticate
    {
        private readonly IWebRequest _request;

        public Authenticate(IWebRequest request)
        {
            _request = request;
        }

        public Authenticate()
            : this(new ProdWebRequest())
        {

        }
        public Connection Connect(string username, string apikey, HttpProxy webProxy)
        {
            _request.WebProxy = webProxy;
            _request.Url = Constants.AUTH_URL;
            _request.Method = "GET";
            _request.Headers.Add(Constants.X_AUTH_USER,username.Encode());
            _request.Headers.Add(Constants.X_AUTH_KEY, apikey.Encode());
            var authenicatedrequest = _request.Submit();
            return new Connection(authenicatedrequest);
            
        }
       
    }
}