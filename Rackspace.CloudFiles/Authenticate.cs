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
        public Account ConnectToAccount(string username, string apikey)
        {
            _request.Url = Constants.AUTH_URL;
            _request.Method = "GET";
            _request.Headers.Add(Constants.X_AUTH_USER, username.Encode());
            _request.Headers.Add(Constants.X_AUTH_KEY, apikey.Encode());
            var authenicatedrequest = _request.Submit();
            return new Account(authenicatedrequest);
        }
        public Account ConnectToAccount(string username, string apikey, HttpProxy webproxy)
        {
            _request.WebProxy = webproxy;
            return ConnectToAccount(username, apikey);
            
        }
        public static Account Connection(string username, string apikey, HttpProxy webproxy)
        {
            return new Authenticate().ConnectToAccount(username, apikey, webproxy);
             
        }
        public static Account Connection(string username, string apikey)
        {
            return new Authenticate().ConnectToAccount(username, apikey);
        }
       
    }
}