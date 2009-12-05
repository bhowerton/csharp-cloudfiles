///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.Text;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Request.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Request
{
    /// <summary>
    /// GetContainerStorageObjectList
    /// </summary>
    public class GetContainerItemList : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;
        private readonly StringBuilder _stringBuilder;

        /// <summary>
        /// GetContainerStorageObjectList constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="requestParameters">dictionary of parameter filters to place on the request url</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name is invalid</exception>
        public GetContainerItemList(string storageUrl,  string containerName, 
                                    Dictionary<GetItemListParameters, string> requestParameters)
        {
            _storageUrl = storageUrl;
            _containerName = containerName;
            _stringBuilder = new StringBuilder();


           
        }

        /// <summary>
        /// GetContainerStorageObjectList constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        public GetContainerItemList(string storageUrl, string containerName)
            : this(storageUrl, containerName, null)
        {
        }

        public Uri CreateUri()
        {
            return  new Uri(_storageUrl + "/" + _containerName.Encode() + _stringBuilder);
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
        }
   }
}