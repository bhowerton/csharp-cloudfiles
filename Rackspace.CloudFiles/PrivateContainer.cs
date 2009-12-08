using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class PrivateContainer :Container
    {
        public PrivateContainer(string containerName, IAccount request) : base(containerName, request)
        {
        }

        public IAuthenticatedRequestFactory Connection
        {
            get { return _account.Connection; }
            
        }
    }
}