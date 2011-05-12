//----------------------------------------------
// See COPYING file for licensing information
//----------------------------------------------

namespace com.mosso.cloudfiles.domain.request
{
    #region Using
    using System;
    using com.mosso.cloudfiles.domain.request.Interfaces;
    using com.mosso.cloudfiles.exceptions;
    using com.mosso.cloudfiles.utils;
    #endregion

    /// <summary>
    /// A class to represent deleting a storage item in a web request
    /// </summary>
    public class CopyStorageItem : IAddToWebRequest
    {
        private readonly string _url;
        private readonly string _containerName;
        private readonly string _sourceItemName;
        private readonly string _destItemName;

        public CopyStorageItem(string url, string containerName, string sourceItemName, string destItemName)
            : this(url, containerName, sourceItemName, destItemName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyStorageItem"/> class.
        /// </summary>
        /// <param name="url">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="storageItemName">the name of the storage item to add meta information too</param>
        /// <param name="emailAddresses">the email addresses to notify who</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="ContainerNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="StorageItemNameException">Thrown when the object name is invalid</exception>
        public CopyStorageItem(string url, string containerName, string sourceItemName, string destItemName, string[] emailAddresses)
        {
            if (string.IsNullOrEmpty(url)
            || string.IsNullOrEmpty(containerName)
            || string.IsNullOrEmpty(sourceItemName)
            || string.IsNullOrEmpty(destItemName))
            {
                throw new ArgumentNullException();
            }

            if (!ContainerNameValidator.Validate(containerName))
            {
                throw new ContainerNameException();
            }

            if (!ObjectNameValidator.Validate(sourceItemName))
            {
                throw new StorageItemNameException();
            }

            if (!ObjectNameValidator.Validate(destItemName))
            {
                throw new StorageItemNameException();
            }

            _url = url;
            _containerName = containerName;
            _sourceItemName = sourceItemName;
            _destItemName = destItemName;
        }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        /// <returns>A new URI for this container</returns>
        public Uri CreateUri()
        {
            return new Uri(_url + "/" + _containerName.Encode() + "/" + _sourceItemName.StripSlashPrefix().Encode());
        }

        /// <summary>
        /// Applies the apropiate method to the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "COPY";
            request.Headers.Add("Destination", "/" + _containerName.Encode() + "/" + _destItemName.StripSlashPrefix().Encode());
        }
    }
}