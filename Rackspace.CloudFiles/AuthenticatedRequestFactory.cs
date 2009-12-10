using System;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class AuthenticatedRequestFactory:IAuthenticatedRequestFactory
    {
        private readonly string _storageurl;
        private readonly string _cdnmanagmenturl;
        private readonly string _authtoken;

        public AuthenticatedRequestFactory(string storageurl, string cdnmanagmenturl, string authtoken)
        {
            _storageurl = storageurl;
            _cdnmanagmenturl = cdnmanagmenturl;
            _authtoken = authtoken;
        }

        public IAuthenticatedRequest CreateRequest()
        {
            return new AuthenticatedRequest(_storageurl, _cdnmanagmenturl, _authtoken);
        }
    }
}