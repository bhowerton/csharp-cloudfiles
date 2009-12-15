using System;
using System.Collections.Generic;
using System.Xml;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IContainer
    {
        IAuthenticatedRequestFactory Connection { get; }
        /// <summary>
        /// Size of the container
        /// </summary>
        long ByteUsed { get; set; }

        /// <summary>
        /// Number of items in the container
        /// </summary>
        long ObjectCount { get; set; }

        /// <summary>
        /// Name of the container
        /// </summary>
        string Name { get; }

      

       
        /// <summary>
        /// This method deletes a storage object in a given container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// connection.DeleteStorageObject("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="storageItemName">The name of the storage object to delete</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        void DeleteStorageObject(string storageItemName);

       

     
        /// <summary>
        /// This method applies meta tags to a storage object on cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// connection.SetStorageObjectMetaInformation("container name", "C:\Local\File\Path\file.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <param name="metadata">A dictionary containiner key/value pairs representing the meta data for this storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        void SetStorageObjectMetaInformation(string storageObjectName, Dictionary<string, string> metadata);

        /// <summary>
        /// This method ensures directory objects created for the entire path
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// connection.MakePath("containername", "/dir1/dir2/dir3/dir4");
        /// </code>
        /// </example>
        /// <param name="path">The path of directory objects to create</param>
        void MakePath(string path);
 
    
    }
}