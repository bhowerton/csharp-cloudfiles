///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Request.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Request
{
    /// <summary>
    /// PutStorageObject
    /// </summary>
    public class PutStorageObject : IAddToWebRequest
    {
        //  public Stream Stream { get; set; }
        private readonly string _storageUrl;
        private readonly string _containerName;
        private readonly string _remoteStorageItemName;
        private Stream filetosend;
        private readonly Dictionary<string, string> _metadata;
        private string _fileUrl;
        private Dictionary<string, string> _mimetypes;


        /// <summary>
        /// PutStorageObject constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="remoteStorageItemName">the name of the storage item to add meta information too</param>
        /// <param name="localFilePath">the path of the file to put into cloudfiles</param>
        public PutStorageObject(string storageUrl, string containerName, string remoteStorageItemName, string localFilePath)
            : this(storageUrl, containerName, remoteStorageItemName, localFilePath, null)
        {
        }

        /// <summary>
        /// PutStorageObject constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="remoteStorageItemName">the name of the storage item to add meta information too</param>
        /// <param name="filestream">the fiel stream of the file to put into cloudfiles</param>
        public PutStorageObject(string storageUrl, string containerName, string remoteStorageItemName, Stream filestream)
            : this(storageUrl, containerName, remoteStorageItemName, filestream, null)
        {
        }

        /// <summary>
        /// PutStorageObject constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="remoteStorageItemName">the name of the storage item to add meta information too</param>
        /// <param name="filetosend">the file stream of the file to put into cloudfiles</param>
        /// <param name="metadata">dictionary of meta tags to apply to the storage item</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="InvalidStorageObjectNameException">Thrown when the object name is invalid</exception>
        public PutStorageObject(string storageUrl, string containerName,
                                string remoteStorageItemName,
                                Stream filetosend,
                                Dictionary<string, string> metadata)
        {
            _fileUrl = CleanUpFilePath(remoteStorageItemName);
            _storageUrl = storageUrl;
            _containerName = containerName;
            _remoteStorageItemName = remoteStorageItemName;
            this.filetosend = filetosend;
            _metadata = metadata;
            


        }

        /// <summary>
        /// PutStorageObject constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="remoteStorageItemName">the name of the storage item to add meta information too</param>
        /// <param name="localFilePath">the path of the file to put into cloudfiles</param>
        /// <param name="metadata">dictionary of meta tags to apply to the storage item</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="InvalidContainerNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="InvalidStorageObjectNameException">Thrown when the object name is invalid</exception>
        public PutStorageObject(string storageUrl, string containerName, string remoteStorageItemName,
                                string localFilePath,
                                Dictionary<string, string> metadata)
        {
            _storageUrl = storageUrl;
            _containerName = containerName;
            _remoteStorageItemName = remoteStorageItemName;
            _metadata = metadata;

            _fileUrl = CleanUpFilePath(localFilePath);
            this.filetosend = new FileStream(_fileUrl, FileMode.Open); //added by ryan as stop gap

         //   BuildMimeTypeDict();

        }



       
        #region private methods so big I need to use regions
       
        




        private string CleanUpFilePath(string filePath)
        {
            return filePath.StripSlashPrefix().Replace(@"file:\\\", "");
        }

        #endregion
        public Uri CreateUri()
        {

            return new Uri(_storageUrl + "/" + _containerName.Encode() + "/" + _remoteStorageItemName.StripSlashPrefix().Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "PUT";


            if (_metadata != null && _metadata.Count > 0)
            {
                foreach (var s in _metadata.Keys)
                {
                    request.Headers.Add(Constants.META_DATA_HEADER + s, _metadata[s]);
                }
            }

            request.AllowWriteStreamBuffering = false;

           // request.ContentType = this.ContentType();
            request.SetContent(filetosend);
        }

    }
}