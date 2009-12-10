using System;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class PublicContainer:BaseContainer
    {
        public PublicContainer(string containerName, IAccount request, long objectcount, long bytesused) : base(containerName, request,objectcount ,bytesused)
        {
        }
        public Uri CdnUri
        {
            get; private set;
        }
        public int TTL
        {
            get; private set;
        }
        public bool LogRetention
        {
            get; private set;
        }
        public string UserAgentACL
        {
            get; private set;
        }
        public string ReferrerACL
        {
            get; private set;
        }
 
    }
}