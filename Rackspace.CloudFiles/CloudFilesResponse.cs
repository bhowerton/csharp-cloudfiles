///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Rackspace.Cloudfiles.Response.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.Cloudfiles.Response
{
    /// <summary>
    /// Represents the response information from a CloudFiles request
    /// </summary>
    public class CloudFilesResponse :  ICloudFilesResponse
    {
        

        public CloudFilesResponse(HttpWebResponse webResponse) 
        {
            Status = webResponse.StatusCode;
            Headers = webResponse.Headers;
            Method = webResponse.Method;
            StatusDescription = webResponse.StatusDescription;
            ContentType = webResponse.ContentType;
            ContentLength = webResponse.ContentLength;
            LastModified = webResponse.LastModified;
            webResponse.Close();
        }

       /// <summary>
        /// A property representing the HTTP Status code returned from cloudfiles
        /// </summary>
        public HttpStatusCode Status { get; private set; }

        /// <summary>
        /// A collection of key-value pairs representing the headers returned from the create container request
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }

        
        /// <summary>
        /// dictionary of meta tags assigned to this storage item
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get
            {
                var tags = new Dictionary<string, string>();
                foreach (string s in Headers.Keys)
                {
                    if (s.IndexOf(Constants.META_DATA_HEADER) == -1) continue;
                    var metaKeyStart = s.LastIndexOf("-");
                    tags.Add(s.Substring(metaKeyStart + 1), Headers[s]);
                }
                return tags;
            }
        }

        public string Method
        {
            get; private set;
        }


        public string StatusDescription
        {
            get; private set;
        }



        public string ContentType
        {
            get; private set;
        }

        public string ETag
        {
            get { return Headers[Constants.ETAG]; }
        }

        public long ContentLength
        {
            get; private set;
        }

        public DateTime LastModified
        {
            get; private set;
        }

        
    }
}