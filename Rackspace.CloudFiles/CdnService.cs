using System;
using System.Collections.Generic;
using System.Threading;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class CdnService
    {
        public CdnService(IAccount account)
        {
            
        }

        public IList<PublicContainer> GetContainers()
        {
            throw new NotImplementedException();
        }
        public PrivateContainer MakeContainerPrivate(PublicContainer publicContainer)
        {
            throw new NotImplementedException();
        }

        public PublicContainer MakeContainerPublic(PrivateContainer privateContainer, int ttl, bool isLoggingEnabled, string userAcl, string referrerAcl)
        {
            throw new NotImplementedException();
        }

        public PublicContainer MakeContainerPublic(PrivateContainer privatecontainer)
        {
            throw new NotImplementedException();
        }
    }
}