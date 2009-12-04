///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Request
{
    /// <summary>
    /// DeleteContainer
    /// </summary>
    public class DeleteContainer : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;

        /// <summary>
        /// DeleteContainer constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidStorageObjectNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the object name is invalid</exception>
        public DeleteContainer(string storageUrl,  string containerName)
        {
            _storageUrl = storageUrl;
            _containerName = containerName;

        }

        public Uri CreateUri()
        {
            return   new Uri(_storageUrl + "/" + _containerName.Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "DELETE";
        }
    }
}