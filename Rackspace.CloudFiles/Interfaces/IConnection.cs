using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Request;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IConnection
    {
        AccountInformation GetAccountInformation();
        string GetAccountInformationJson();
        XmlDocument GetAccountInformationXml();
        void CreateContainer(string containerName);
        void DeleteContainer(string continerName);
        List<string> GetContainers();
        List<string> GetContainerItemList(string containerName);
        List<string> GetContainerItemList(string containerName, Dictionary<GetItemListParameters, string> parameters);
        Container GetContainerInformation(string containerName);
        string GetContainerInformationJson(string containerName);
        XmlDocument GetContainerInformationXml(string containerName);
        void PutStorageObjectAsync(string containerName, Stream storageStream, string remoteStorageItemName);
        void PutStorageObjectAsync(string containerName, string localStorageItemName);
        void GetStorageObjectAsync(string containerName, string storageItemName, string localItemName);
        void PutStorageObject(string containerName, string localFilePath, Dictionary<string, string> metadata);
        void PutStorageObject(string containerName, string localFilePath);
        void PutStorageObject(string containerName, Stream storageStream, string remoteStorageItemName);
        void PutStorageObject(string containerName, Stream storageStream, string remoteStorageItemName, Dictionary<string, string> metadata);
        void DeleteStorageItem(string containerName, string storageItemname);
        StorageObject GetStorageObject(string containerName, string storageObjectName);
        void GetStorageObject(string containerName, string storageContainerName, string localFileName);
        StorageObject GetStorageObject(string containerName, string storageObjectName, Dictionary<RequestHeaderFields, string> requestHeaderFields);
        void GetStorageObject(string containerName, string storageObjectName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields);
        StorageObjectInformation GetStorageItemInformation(string containerName, string storageObjectName);
        void SetStorageObjectMetaInformation(string containerName, string storageObjectName, Dictionary<string, string> metadata);
        List<string> GetPublicContainers();
        Uri MarkContainerAsPublic(string containerName);
        Uri MarkContainerAsPublic(string containerName, int timeToLiveInSeconds);
        void MarkContainerAsPrivate(string containerName);
       
        Container GetPublicContainerInformation(string containerName);
        void MakePath(string containerName, string path);
      

        /// <summary>
        /// The storage url used to interact with cloud files
        /// </summary>
        string StorageUrl { get;  set; }

        /// <summary>
        /// the session based token used to ensure the user was authenticated
        /// </summary>
        string AuthToken { get;  set; }


        void SetDetailsOnPublicContainer(string publiccontainer, bool loggingenabled, int ttl, string referreracl, string useragentacl );
        XmlDocument GetPublicAccountInformationXML();
        string GetPublicAccountInformationJSON();
    }
}