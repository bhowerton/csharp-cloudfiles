using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Request;
using Rackspace.Cloudfiles.Response.Interfaces;
using Rackspace.CloudFiles.utils;
using System.Xml.Linq;

namespace Rackspace.CloudFiles
{
    public class Container : BaseContainer
    {
        public Container(string containerName, IAccount request, long objectcount, long bytesused)
            : base(containerName, request, objectcount, bytesused)
        {
        }

        public Container(string containerName,IHttpReaderWriter readerwriter,
            IAccount request, long objectcount, long bytesused):base(
            containerName, readerwriter, request, objectcount, bytesused
            ){}
    }
    /// <summary>
    /// Container
    /// </summary>
    public class BaseContainer : IContainer
    {
        private readonly IHttpReaderWriter _httpReaderWriter;
        protected readonly IAccount _account;


        protected BaseContainer(string name, IHttpReaderWriter httpReaderWriter, IAccount account, long objectcount, long bytesused)
        {
            _httpReaderWriter = httpReaderWriter;
            _account = account;
            Name = name;
            ObjectCount = objectcount;
            ByteUsed = bytesused;
        }
        protected BaseContainer(string name, IAccount account, long objetcount, long bytesused)
            : this(name, new HttpReaderWriter(), account, objetcount, bytesused)
        {

        }
        #region properties

        public IAuthenticatedRequestFactory Connection
        {
            get { return _account.Connection; }
        }

        /// <summary>
        /// Size of the container
        /// </summary>
        public long ByteUsed { get; set; }

        /// <summary>
        /// Number of items in the container
        /// </summary>
        public long ObjectCount { get; set; }

        /// <summary>
        /// Name of the container
        /// </summary>
        public string Name { get; private set; }


        #endregion
        #region methods

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
        public void DeleteStorageItem(string storageItemName)
        {
            Ensure.NotNullOrEmpty(storageItemName);

            var request = _account.Connection.CreateRequest();
            request.Method = HttpVerb.DELETE;

            var response = request.SubmitStorageRequest("/" + this.Name.Encode() + "/" + storageItemName.StripSlashPrefix().Encode());

            if (response != null && response.Status == HttpStatusCode.NotFound)
                throw new StorageObjectNotFoundException("The requested storage object for deletion does not exist");

        }





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
        public void SetStorageObjectMetaInformation(string storageObjectName, Dictionary<string, string> metadata)
        {
            Ensure.NotNullOrEmpty(storageObjectName);
            Ensure.ValidStorageObjectName(storageObjectName);



            var request = _account.Connection.CreateRequest();
            Action attachMetaData = () =>
                                      {
                                          foreach (var pair in metadata)
                                          {
                                              if (pair.Key.Length > Constants.MAXIMUM_META_KEY_LENGTH)
                                                  throw new MetaKeyLengthException(
                                                      "The meta key length exceeds the maximum length of " +
                                                      Constants.MAXIMUM_META_KEY_LENGTH);
                                              if (pair.Value.Length > Constants.MAXIMUM_META_VALUE_LENGTH)
                                                  throw new MetaValueLengthException(
                                                      "The meta value length exceeds the maximum length of " +
                                                      Constants.MAXIMUM_META_VALUE_LENGTH);

                                              request.Headers.Add(Constants.META_DATA_HEADER + pair.Key, pair.Value);
                                          }
                                      };
            request.Method = HttpVerb.POST;
            attachMetaData.Invoke();
            var response = request.SubmitStorageRequest("/" + Name.Encode() + "/" + storageObjectName.Encode());

            if (response != null && response.Status == HttpStatusCode.NotFound) throw new StorageObjectNotFoundException("The requested storage object does not exist");



        }


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
        public void MakePath(string path)
        {



            var directories = path.StripSlashPrefix().Split('/');
            var directory = "";
            var firstItem = true;
            foreach (var item in directories)
            {
                if (string.IsNullOrEmpty(item)) continue;
                if (!firstItem) directory += "/";
                directory += item.Encode();
                var request = _account.Connection.CreateRequest();
                request.Method = HttpVerb.PUT;
                request.ContentType = "application/directory";


                var urltoappend = "/" + Name.Encode() + "/" + directory.Encode();
                var response = request.SubmitStorageRequest(urltoappend, req => req.ContentLength = 0, res => { });
                if (response.Status == HttpStatusCode.BadRequest) throw new ContainerNotFoundException("The requested container does not exist");
                if (response.Status == HttpStatusCode.PreconditionFailed) throw new PreconditionFailedException("Precondition error you are missing some required fields for this request");

                firstItem = false;
            }




        }

        private ICloudFilesResponse BaseGetContainerObjectList(Format format, string getresponse)
        {
            var request = _account.Connection.CreateRequest();
            return request.SubmitStorageRequest("/" + Name.Encode() + "?format=" + format.GetDescription(), x => { }, resp => getresponse =
                resp.GetResponseStream().ConvertToString());
        }
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
        public string GetStorageObjectListInSpecifiedFormat(Format outputformat)
        {

            string retvalue = "";
            BaseGetContainerObjectList(outputformat, retvalue);
            return retvalue;
        }

        #endregion

        public StorageObject GetStorageObject(string storageObjectName)
        {
            var request = _account.Connection.CreateRequest();
            request.Method = HttpVerb.HEAD;
            var response = request.SubmitStorageRequest("/" + Name.Encode() + "/" + storageObjectName);
            StorageObject storageObject;
            if (response.Status == HttpStatusCode.NoContent)
            {
                storageObject = new StorageObject(this, storageObjectName, response.ContentType, response.ContentLength,
                                                  response.LastModified, response.ETag);

                return storageObject;
            }
            if (response.Status == HttpStatusCode.NotFound) throw new StorageObjectNotFoundException();
            throw new Exception("Response status was " + response.Status);

        }
        public IList<StorageObject> GetStorageObjects()
        {
            var request = _account.Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            string xml = "";
            var response = request.SubmitStorageRequest("/"+Name, req => { }, resp => xml= _httpReaderWriter.GetStringFromStream(resp));

            var rootelemtn = XElement.Parse(xml);
            var objectelements = rootelemtn.Elements("object");

            var objects = objectelements.Select(x =>
                                                new StorageObject(this, x.Element("name").Value,
                                                      x.Element("content_type").Value,
                                                                  long.Parse(x.Element("bytes").Value),
                                                                  x.Element("last_modified").Value.ParseCfDateTime(),
                                                                  x.Element("hash").Value
                                                                  )
                                                );
            return objects.ToArray();
        }
    }
}