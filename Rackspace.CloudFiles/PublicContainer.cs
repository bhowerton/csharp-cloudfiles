using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class PublicContainer:Container
    {
        public PublicContainer(string containerName, IAccount request) : base(containerName, request)
        {
        }

        public IAuthenticatedRequestFactory Connection
        {
            get { return _account.Connection; }
            
        }
    }
}