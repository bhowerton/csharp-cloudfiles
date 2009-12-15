using System.Net;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class Authenticate
    {
        private readonly IAuthorizationRequest _request;

        public Authenticate(IAuthorizationRequest request)
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
            
            var accountrequest = authenicatedrequest.CreateRequest();
            accountrequest.Method = HttpVerb.HEAD;
            var accountresponse = accountrequest.SubmitStorageRequest("");
            var containercount = long.Parse(accountresponse.Headers[Constants.X_ACCOUNT_CONTAINER_COUNT]);
            var bytecount = long.Parse(accountresponse.Headers[Constants.X_ACCOUNT_BYTES_USED]);

           
            return new Account(authenicatedrequest, containercount, bytecount);
        }
        public Account ConnectToAccount(string username, string apikey, IWebProxy webproxy)
        {
            _request.WebProxy = webproxy;
            return ConnectToAccount(username, apikey);
            
        }
        public static Account Connection(string username, string apikey, IWebProxy webproxy)
        {
            return new Authenticate().ConnectToAccount(username, apikey, webproxy);
             
        }
        public static Account Connection(string username, string apikey)
        {
            return new Authenticate().ConnectToAccount(username, apikey);
        }
       
    }
}