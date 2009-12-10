using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Request;
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


    }
    /// <summary>
    /// Container
    /// </summary>
    public class BaseContainer:IContainer
    {
        protected readonly IAccount _account;


        protected BaseContainer(string name, IAccount account, long objectcount, long bytesused)
        {
            _account = account;
            Name = name;
            ObjectCount = objectcount;
            ByteUsed = bytesused;
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
        public List<string> GetContainerStorageObjectList(Dictionary<GetItemListParameters, string> parameters)
        {
            StringBuilder _stringBuilder = new StringBuilder();


            foreach (GetItemListParameters param in parameters.Keys)
            {
                var paramName = param.ToString().ToLower();
                //FIXME: what does this do
                if (param == GetItemListParameters.Limit)
                    int.Parse(parameters[param]);

                if (_stringBuilder.Length > 0)
                    _stringBuilder.Append("&");
                else
                    _stringBuilder.AppendFormat("?");
                _stringBuilder.Append(paramName + "=" + parameters[param].Encode());
            }
            return GetContainerStorageObjectList(_stringBuilder.ToString());
          
        }
        private List<string> GetContainerStorageObjectList(string urlparams)
        {
            var containerItemList = new List<string>();

            try
            {

                var request = _account.Connection.CreateRequest();
                request.Method = HttpVerb.GET;
                var getContainerItemListResponse = request.SubmitStorageRequest(this.Name.Encode() + urlparams);
                if (getContainerItemListResponse.Status == HttpStatusCode.OK)
                {
                    containerItemList.AddRange(getContainerItemListResponse.ContentBody);
                }
            }
            catch (WebException we)
            {

                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new ContainerNotFoundException("The requested container does not exist!");

                throw;
            }
            return containerItemList;
        }
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
        public List<string> GetContainerStorageObjectList()
        {

            return GetContainerStorageObjectList("");
        }
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
            try
            {
                var request = _account.Connection.CreateRequest();
                request.Method = HttpVerb.DELETE;

                request.SubmitStorageRequest(this.Name.Encode() + "/" + storageItemName.StripSlashPrefix().Encode());
            }
            catch (WebException we)
            {


                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new StorageItemNotFoundException("The requested storage object for deletion does not exist");

                throw;
            }
        }



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
        public void SetStorageObjectMetaInformation(string storageObjectName, Dictionary<string, string> metadata)
        {
            Ensure.NotNullOrEmpty(storageObjectName);
            Ensure.ValidStorageObjectName(storageObjectName);


            try
            {
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
                request.SubmitStorageRequest(Name.Encode() + "/" + storageObjectName.Encode());
             
            }
            catch (WebException we)
            {


                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new StorageItemNotFoundException("The requested storage object does not exist");

                throw;
            }
        }
        /// <summary>
        /// needs error checking or code restructure to make sure this is only called on public container
        /// </summary>
        /// <param name="loggingenabled"></param>
        /// <param name="ttl"></param>
        /// <param name="referreracl"></param>
        /// <param name="useragentacl"></param>
        public void SetDetailsOnPublicContainer(bool loggingenabled, int ttl, string referreracl, string useragentacl)
        {

            try
            {

                var request = _account.Connection.CreateRequest();
                request.Method = HttpVerb.POST;
                request.Headers.Add(Constants.X_LOG_RETENTION, loggingenabled.Capitalize());
                if (ttl > -1) request.Headers.Add(Constants.X_CDN_TTL, ttl.ToString());
                if (!String.IsNullOrEmpty(useragentacl)) request.Headers.Add(Constants.X_USER_AGENT_ACL, useragentacl);
                if (!String.IsNullOrEmpty(referreracl)) request.Headers.Add(Constants.X_REFERRER_ACL, referreracl);
                request.SubmitCdnRequest(Name.Encode() );
            }
            catch (WebException we)
            {


                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Your access credentials are invalid or have expired. ");
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new PublicContainerNotFoundException("The specified container does not exist.");
                throw;
            }



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
            try
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
                    request.SetContent(new MemoryStream(new byte[0]));
                    request.SubmitStorageRequest(Name.Encode() + "/" + directory.StripSlashPrefix().Encode());
                    firstItem = false;
                }
            }
            catch (WebException ex)
            {
                var webResponse = (HttpWebResponse)ex.Response;
                if (webResponse == null) throw;
                if (webResponse.StatusCode == HttpStatusCode.BadRequest)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (webResponse.StatusCode == HttpStatusCode.PreconditionFailed)
                    throw new PreconditionFailedException(ex.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }






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
        public string GetStorageObjectListInJson()
        {
            try
            {
                var getSerializedResponse = BaseGetContainerObjectList(Format.JSON);
                var jsonResponse = String.Join("", getSerializedResponse.ContentBody.ToArray());
                getSerializedResponse.Dispose();
                return jsonResponse;
            }
            catch (WebException we)
            {


                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new ContainerNotFoundException("The requested container does not exist");

                throw;
            }
        }
        private ICloudFilesResponse BaseGetContainerObjectList(Format format)
        {
            var request = _account.Connection.CreateRequest();
            return request.SubmitStorageRequest(Name.Encode() + "?format=" + EnumHelper.GetDescription(format));
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
        public XmlDocument GetStorageObjectListInXml()
        {
            try
            {
                var getSerializedResponse = BaseGetContainerObjectList(Format.XML);
                var xmlResponse = String.Join("", getSerializedResponse.ContentBody.ToArray());
                getSerializedResponse.Dispose();

                if (xmlResponse == null) return new XmlDocument();

                var xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(xmlResponse);

                }
                catch (XmlException)
                {
                    return xmlDocument;
                }

                return xmlDocument;
            }
            catch (WebException we)
            {


                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new ContainerNotFoundException("The requested container does not exist");

                throw;
            }
        }

        #endregion
        
        public StorageObject GetStorageObject(string storageObjectName)
        {
            throw new NotImplementedException();
        }
        public IList<StorageObject> GetStorageObjects()
        {
             var request = _account.Connection.CreateRequest();
			request.Method= HttpVerb.GET;
			var response = request.SubmitStorageRequest(Name);
			var xml = response.ContentBody.ConvertToString();
			var rootelemtn = XElement.Parse(xml);
			var objectelements = rootelemtn.Elements("object");
			
			var objects = objectelements.Select(x=>
			                                    new StorageObject(this,x.Element("name").Value, new Dictionary<string,string>(),
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