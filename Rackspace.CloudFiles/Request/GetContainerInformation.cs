///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.domain.request;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Request
{
    /// <summary>
    /// GetContainerInformation
    /// </summary>
    public class GetContainerInformation : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;

        /// <summary>
        /// GetContainerInformation constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name is invalid</exception>
        public GetContainerInformation(string storageUrl,  string containerName)
        {

            _storageUrl = storageUrl;
            _containerName = containerName;
        
            
        }

        public Uri CreateUri()
        {
            return new Uri(_storageUrl + "/" + _containerName.Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "HEAD";
        }
    }

    public class GetContainerInformationSerialized : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;
        private readonly Format _format;

        /// <summary>
        /// GetContainerInformationSerialized constructor
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null</exception>
        public GetContainerInformationSerialized(string storageUrl, string containerName, Format format)
        {  
            _storageUrl = storageUrl;
            _containerName = containerName;
            _format = format;
        }

        public Uri CreateUri()
        {
            return new Uri(_storageUrl + "/" + _containerName.Encode() + "?format=" + EnumHelper.GetDescription(_format));
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
        }
    }
}