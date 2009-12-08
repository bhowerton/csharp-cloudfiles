using System;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class AuthenticatedRequestFactory:IAuthenticatedRequestFactory
    {
        public IAuthenticatedRequest CreateRequest()
        {
            throw new NotImplementedException();
        }
    }
}