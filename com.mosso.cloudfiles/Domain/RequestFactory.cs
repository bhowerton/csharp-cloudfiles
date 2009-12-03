using System;
using System.Net;
using Rackspace.CloudFiles.domain.request.Interfaces;

namespace Rackspace.CloudFiles.domain.request
{
    public interface IRequestFactory
    {
        ICloudFilesRequest Create(Uri uri);
        ICloudFilesRequest Create(Uri uri, ProxyCredentials creds);
    }

    public class RequestFactory : IRequestFactory
    {
        public ICloudFilesRequest Create(Uri uri, ProxyCredentials creds)
        {
            return new CloudFilesRequest((HttpWebRequest) WebRequest.Create(uri), creds);
        }
        public  ICloudFilesRequest Create(Uri uri)
        {
            return new CloudFilesRequest((HttpWebRequest) WebRequest.Create(uri));
        }
    }
}