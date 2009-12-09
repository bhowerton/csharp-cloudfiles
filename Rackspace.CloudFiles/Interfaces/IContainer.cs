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
        long ByteCount { get; set; }

        /// <summary>
        /// Number of items in the container
        /// </summary>
        long ObjectCount { get; set; }

        /// <summary>
        /// Name of the container
        /// </summary>
        string Name { get; }

      

        /// <summary>
        /// This method retrieves the contents of a container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Dictionary{GetItemListParameters, string} parameters = new Dictionary{GetItemListParameters, string}();
        /// parameters.Add(GetItemListParameters.Limit, 2);
        /// parameters.Add(GetItemListParameters.Marker, 1);
        /// parameters.Add(GetItemListParameters.Prefix, "a");
        /// List{string} containerItemList = connection.GetContainerStorageObjectList("container name", parameters);
        /// </code>
        /// </example>
        /// <param name="parameters">Parameters to feed to the request to filter the returned list</param>
        /// <returns>An instance of List, containing the names of the storage objects in the give container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        List<string> GetContainerStorageObjectList(Dictionary<GetItemListParameters, string> parameters);

        /// <summary>
        /// This method retrieves the contents of a container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// List{string} containerItemList = connection.GetContainerStorageObjectList("container name");
        /// </code>
        /// </example>
        /// <returns>An instance of List, containing the names of the storage objects in the give container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        List<string> GetContainerStorageObjectList();

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
        void DeleteStorageItem(string storageItemName);

        /// <summary>
        /// An alternate method for downloading storage objects from cloudfiles directly to a file name specified in the method
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Dictionary{RequestHeaderFields, string} requestHeaderFields = Dictionary{RequestHeaderFields, string}();
        /// string dummy_etag = "5c66108b7543c6f16145e25df9849f7f";
        /// requestHeaderFields.Add(RequestHeaderFields.IfMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfNoneMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfModifiedSince, DateTime.Now.AddDays(6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.IfUnmodifiedSince, DateTime.Now.AddDays(-6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.Range, "0-5");
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteFileName.txt", "C:\Local\File\Path\file.txt", requestHeaderFields);


     
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
        /// needs error checking or code restructure to make sure this is only called on public container
        /// </summary>
        /// <param name="loggingenabled"></param>
        /// <param name="ttl"></param>
        /// <param name="referreracl"></param>
        /// <param name="useragentacl"></param>
        void SetDetailsOnPublicContainer(bool loggingenabled, int ttl, string referreracl, string useragentacl);

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

        /// <summary>
        /// JSON serialized format of the container's objects
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// string jsonResponse = connection.GetStorageObjectListInJson("container name");
        /// </code>
        /// </example>
        /// <returns>json string of object information inside the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        string GetStorageObjectListInJson();

        /// <summary>
        /// XML serialized format of the container's objects
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// XmlDocument xmlResponse = connection.GetStorageObjectListInXml("container name");
        /// </code>
        /// </example>
        /// <returns>xml document of object information inside the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        XmlDocument GetStorageObjectListInXml();
    }
}