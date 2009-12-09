using System;
using System.Collections.Generic;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles
{
    public class PrivateContainer :Container
    {
        public PrivateContainer(string containerName, IAccount request) : base(containerName, request)
        {
        }

      

       
    }
}