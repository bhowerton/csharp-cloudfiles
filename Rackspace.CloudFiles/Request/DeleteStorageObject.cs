///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Request.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Request
{
    /// <summary>
    /// DeleteStorageObject
    /// </summary>
    public class DeleteStorageObject : IAddToWebRequest
    {
        private readonly string _storageUrl;
        
        private readonly string _containerName;
        private readonly string _storageItemName;

        /// <summary>
        /// DeleteStorageObject constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="storageItemName">the name of the storage item to add meta information too</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="InvalidStorageObjectNameException">Thrown when the object name is invalid</exception>
        public DeleteStorageObject(string storageUrl,  string containerName, string storageItemName)
        {
            _storageUrl = storageUrl;
         
            _containerName = containerName;
            _storageItemName = storageItemName;
        }

        public Uri CreateUri()
        {
            return new Uri(_storageUrl + "/" + _containerName.Encode() + "/" + _storageItemName.StripSlashPrefix().Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "DELETE";

        }
    }
}