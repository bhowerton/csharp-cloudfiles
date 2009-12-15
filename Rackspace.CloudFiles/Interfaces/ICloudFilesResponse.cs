using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Rackspace.Cloudfiles.Response.Interfaces
{
    public interface ICloudFilesResponse
    { 

        /// <summary>
        /// A property representing the status of the request from cloudfiles
        /// </summary>
        HttpStatusCode Status { get; }

        /// <summary>
        /// A collection of key-value pairs representing the headers returned from each request
        /// </summary>
        WebHeaderCollection Headers { get; }
        /// <summary>
        /// dictionary of meta tags assigned to this storage item
        /// </summary>
        Dictionary<string, string> Metadata { get; }

        string Method { get;  }
       
        string StatusDescription { get;  }
   
        string ContentType{ get; }
        string ETag { get;  }
        long ContentLength { get; }
        DateTime LastModified { get; }
        


        
    }
}