using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles
{
    public class TemporaryDumpingGround
    {
        /// <summary>
        /// This method downloads a storage object from cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <returns>An instance of StorageObject with the stream containing the bytes representing the desired storage object</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public StorageObject GetStorageObject(string storageObjectName)
        {
            Ensure.NotNullOrEmpty(storageObjectName);
            throw new  NotImplementedException();//my commenting and exception: Ryan; this is a temporary spot for refactoring
          //  return GetStorageObject(storageObjectName, new Dictionary<RequestHeaderFields, string>());
        }

        /// <summary>
        /// An alternate method for downloading storage objects from cloudfiles directly to a file name specified in the method
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt", "C:\Local\File\Path\file.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The file name to save the storage object into on disk</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObject(string storageObjectName, string localFileName)
        {
            Ensure.NotNullOrEmpty(storageObjectName, localFileName);

            Ensure.ValidStorageObjectName(storageObjectName);

            GetStorageObject(storageObjectName, localFileName, new Dictionary<RequestHeaderFields, string>());
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
        /// </code>
        /// </example>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The file name to save the storage object into on disk</param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObject(string storageObjectName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {

            Ensure.NotNullOrEmpty(storageObjectName, localFileName);
            Ensure.ValidStorageObjectName(storageObjectName);
            throw new AbandonedMutexException();
            //this will become the core class for file transfer, sync calls will call this as well.
//
//            var getStorageItem = new GetStorageObject(StorageUrl, containerName, storageObjectName, requestHeaderFields);
//
//            try
//            {
//                var getStorageItemResponse = _requestfactory.Submit(getStorageItem, AuthToken, _usercreds.ProxyCredentials);
//                foreach (var callback in callbackFuncs)
//                {
//                    getStorageItemResponse.Progress += callback;
//                }
//                var stream = getStorageItemResponse.GetResponseStream();
//
//                StoreFile(localFileName, stream);
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                response.Close();
//                if (response.StatusCode == HttpStatusCode.NotFound)
//                    throw new StorageItemNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
        
        
        }
        
        /// <summary>
        /// This method retrieves meta information and size, in bytes, of a requested storage object
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// StorageObject storageItem = connection.GetStorageItemInformation("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <returns>An instance of StorageObject containing the byte size and meta information associated with the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public StorageObjectInformation GetStorageItemInformation(string storageObjectName)
        {
            //May not need this one, moving it over, but it will likely be replaced
            throw new NotImplementedException();
//            Ensure.NotNullOrEmpty(containerName, storageObjectName);
//            try
//            {
//                var getStorageItemInformation = new GetStorageItemInformation(StorageUrl, containerName, storageObjectName);
//                var getStorageItemInformationResponse = _requestfactory.Submit(getStorageItemInformation, AuthToken, _usercreds.ProxyCredentials);
//
//
//                var storageItemInformation = new StorageObjectInformation(getStorageItemInformationResponse.Headers);
//
//                return storageItemInformation;
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
//                    throw new StorageItemNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
        }
        /// <summary>
        /// This method downloads a storage object from cloudfiles asychronously
        /// </summary>
        /// <example>
        /// <code>
        /// private void transferComplete()
        /// {
        ///     if (InvokeRequired)
        ///     {
        ///         Invoke(new CloseCallback(Close), new object[]{});
        ///     }
        ///     else
        ///     {
        ///         if (!IsDisposed)
        ///             Close();
        ///     }
        /// }
        /// 
        /// private void fileTransferProgress(int bytesTransferred)
        /// {
        ///    if (InvokeRequired)
        ///    {
        ///        Invoke(new FileProgressCallback(fileTransferProgress), new object[] {bytesTransferred});
        ///    }
        ///    else
        ///    {
        ///        System.Console.WriteLine(totalTransferred.ToString());
        ///        totalTransferred += bytesTransferred;
        ///        bytesTransferredLabel.Text = totalTransferred.ToString();
        ///        var progress = (int) ((totalTransferred/filesize)*100.0f);
        ///        if(progress > 100)
        ///            progress = 100;
        ///        transferProgressBar.Value = progress ;
        ///    }
        /// }
        /// Dictionary{RequestHeaderFields, string} requestHeaderFields = Dictionary{RequestHeaderFields, string}();
        /// string dummy_etag = "5c66108b7543c6f16145e25df9849f7f";
        /// requestHeaderFields.Add(RequestHeaderFields.IfMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfNoneMatch, dummy_etag);
        /// requestHeaderFields.Add(RequestHeaderFields.IfModifiedSince, DateTime.Now.AddDays(6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.IfUnmodifiedSince, DateTime.Now.AddDays(-6).ToString());
        /// requestHeaderFields.Add(RequestHeaderFields.Range, "0-5");
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.GetStorageObjectAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt", requestHeaderFields);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The name to write the file to on your hard drive. </param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObjectAsync(string containerName, string storageItemName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
            //will decide if I use this or not
            /*
            var thread = new Thread(
                 () =>
                 {
                     try
                     {
                         GetStorageObject(containerName, storageItemName, localFileName, requestHeaderFields);
                     }
                    finally //Always fire the completed event
                     {
                         if (OperationComplete != null)
                         {
                             //Fire the operation complete event if there aren't any listeners
                             OperationComplete();
                         }
                     }
                 }
             );
            thread.Start();
             * */
        }

        /// <summary>
        /// This method downloads a storage object from cloudfiles asychronously
        /// </summary>
        /// <example>
        /// <code>
        /// private void transferComplete()
        /// {
        ///     if (InvokeRequired)
        ///     {
        ///         Invoke(new CloseCallback(Close), new object[]{});
        ///     }
        ///     else
        ///     {
        ///         if (!IsDisposed)
        ///             Close();
        ///     }
        /// }
        /// 
        /// private void fileTransferProgress(int bytesTransferred)
        /// {
        ///    if (InvokeRequired)
        ///    {
        ///        Invoke(new FileProgressCallback(fileTransferProgress), new object[] {bytesTransferred});
        ///    }
        ///    else
        ///    {
        ///        System.Console.WriteLine(totalTransferred.ToString());
        ///        totalTransferred += bytesTransferred;
        ///        bytesTransferredLabel.Text = totalTransferred.ToString();
        ///        var progress = (int) ((totalTransferred/filesize)*100.0f);
        ///        if(progress > 100)
        ///            progress = 100;
        ///        transferProgressBar.Value = progress ;
        ///    }
        /// }
        /// 
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.GetStorageObjectAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The name to write the file to on your hard drive. </param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageObjectAsync(string containerName, string storageItemName, string localFileName)
        {
            //will likely not use
//            var thread = new Thread(
//                 () =>
//                 {
//                     try
//                     {
//                         GetStorageObject(containerName, storageItemName, localFileName);
//                     }
//                    finally //Always fire the completed event
//                     {
//                         if (OperationComplete != null)
//                         {
//                             //Fire the operation complete event if there aren't any listeners
//                             OperationComplete();
//                         }
//                     }
//                 }
//             );
//            thread.Start();
        }

        /// <summary>
        /// An alternate method for downloading storage objects. This one allows specification of special HTTP 1.1 compliant GET headers
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
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt", requestHeaderFields);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object</param>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <returns>An instance of StorageObject with the stream containing the bytes representing the desired storage object</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public StorageObject GetStorageObject(string storageObjectName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
            //will get this working but may implement differently
            
            Ensure.NotNullOrEmpty(storageObjectName);
            Ensure.ValidStorageObjectName(storageObjectName);

//            try
//            {
//                var getStorageItem = new GetStorageObject(StorageUrl, containerName, storageObjectName, requestHeaderFields);
//                var getStorageItemResponse = _requestfactory.Submit(getStorageItem, AuthToken, _usercreds.ProxyCredentials);
//
//
//                var metadata = GetMetadata(getStorageItemResponse);
//                var storageItem = new StorageObject(TODO, containerName, storageObjectName, metadata, getStorageItemResponse.ContentType, getStorageItemResponse.GetResponseStream(), getStorageItemResponse.ContentLength, getStorageItemResponse.LastModified);
//                //                getStorageItemResponse.Dispose();
//                return storageItem;
//            }
//            catch (WebException we)
//            {
//
//
//                var response = (HttpWebResponse)we.Response;
//                response.Close();
//                if (response.StatusCode == HttpStatusCode.NotFound)
//                    throw new StorageItemNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
            throw new AbandonedMutexException();
        }

//        /// <summary>
//        /// This method sets a container as public on the CDN
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
//        /// IConnection connection = new Account(userCredentials);
//        /// Uri containerPublicUrl = connection.MarkContainerAsPublic("container name");
//        /// </code>
//        /// </example>
//        /// <param name="containerName">The name of the container to mark public</param>
//        /// <returns>A string representing the URL of the public container or null</returns>
//        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
//        public Uri MarkContainerAsPublic()
//        {
//            return MarkContainerAsPublic(-1);
//        }
//
//        /// <summary>
//        /// This method sets a container as private on the CDN
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
//        /// IConnection connection = new Account(userCredentials);
//        /// connection.MarkContainerAsPrivate("container name");
//        /// </code>
//        /// </example>
//        /// <param name="containerName">The name of the container to mark public</param>
//        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
//        public void MarkContainerAsPrivate(string containerName)
//        {
//            Ensure.NotNullOrEmpty(containerName);
//            try
//            {
//                var request = _account.Connection.CreateRequest();
//                request.Method = HttpVerb.POST;
//                request.Headers.Add(Constants.X_CDN_ENABLED, "FALSE");
//                request.SubmitCdnRequest(containerName.Encode());
//
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                    throw new UnauthorizedAccessException("Your access credentials are invalid or have expired. ");
//                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
//                    throw new PublicContainerNotFoundException("The specified container does not exist.");
//                throw;
//            }
//
//        }

//        /// <summary>
//        /// This method sets a container as public on the CDN
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
//        /// IConnection connection = new Account(userCredentials);
//        /// Uri containerPublicUrl = connection.MarkContainerAsPublic("container name", 12345);
//        /// </code>
//        /// </example>
//        /// <param name="timeToLiveInSeconds">The maximum time (in seconds) content should be kept alive on the CDN before it checks for freshness.</param>
//        /// <returns>A string representing the URL of the public container or null</returns>
//        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
//        public Uri MarkContainerAsPublic(int timeToLiveInSeconds)
//        {
//
//            try
//            {
//
//                var request = _account.Connection.CreateRequest();
//                request.Method = HttpVerb.PUT;
//                if (timeToLiveInSeconds > -1) { request.Headers.Add(Constants.X_CDN_TTL, timeToLiveInSeconds.ToString()); }
//                var response = request.SubmitCdnRequest(Name);
//
//                return response == null ? null : new Uri(response.Headers[Constants.X_CDN_URI]);
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                    throw new AuthenticationFailedException("You do not have permission to request the list of public containers.");
//                throw;
//            }
//        }

//        /// <summary>
//        /// Retrieves a Container object containing the public CDN information
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
//        /// IConnection connection = new Account(userCredentials);
//        /// Container container = connection.GetPublicContainerInformation("container name")
//        /// </code>
//        /// </example>
//        /// <param name="containerName">The name of the container to query about</param>
//        /// <returns>An instance of Container with appropriate CDN information or null</returns>
//        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
//        public Container GetPublicContainerInformation(string containerName)
//        {
//            Ensure.NotNullOrEmpty(containerName);
//            Ensure.ValidContainerName(containerName);
//            try
//            {
//                var request = Connection.CreateRequest();
//                request.Method = HttpVerb.HEAD;
//                var response = request.SubmitCdnRequest(containerName.Encode() + "?enabled_only=true");
//                return response == null ?
//                    null
//                    : new Container(containerName, this) { CdnUri = response.Headers[Constants.X_CDN_URI], TTL = Convert.ToInt32(response.Headers[Constants.X_CDN_TTL]) };
//            }
//            catch (WebException ex)
//            {
//
//
//                var webResponse = (HttpWebResponse)ex.Response;
//                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Unauthorized)
//                    throw new UnauthorizedAccessException("Your authorization credentials are invalid or have expired.");
//                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.NotFound)
//                    throw new ContainerNotFoundException("The specified container does not exist.");
//                throw;
//            }
//        }
//        //private bool IsAuthenticated()
//        //{
//        //	return this.isNotNullOrEmpty(AuthToken, StorageUrl, this.CdnManagementUrl) && _usercreds != null;
//        //}
//        private string getContainerCDNUri(Container container)
//        {
//            try
//            {
//                //    var public_container = GetPublicContainerInformation(container.Name);
//                //   return public_container == null ? "" : public_container.CdnUri;
//                throw new NotImplementedException();
//            }
//            catch (ContainerNotFoundException)
//            {
//                return "";
//            }
//            catch (WebException we)
//            {
//
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                    throw new AuthenticationFailedException(we.Message);
//                throw;
//            }
//        }
//        /// <summary>
//        /// This method retrieves the names of the of the containers made public on the CDN
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
//        /// IConnection connection = new Account(userCredentials);
//        /// List{string} containers = connection.GetPublicContainers();
//        /// </code>
//        /// </example>
//        /// <returns>A list of the public containers</returns>
//        public List<string> GetPublicContainers()
//        {
//            try
//            {
//                var request = Connection.CreateRequest();
//                var getPublicContainersResponse = request.SubmitCdnRequest("?enabled_only=true");
//                var containerList = getPublicContainersResponse.ContentBody;
//                getPublicContainersResponse.Dispose();
//
//                return containerList.ToList();
//            }
//            catch (WebException we)
//            {
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                    throw new AuthenticationFailedException("You do not have permission to request the list of public containers.");
//                throw;
//            }
//        }

//
//        public XmlDocument GetPublicAccountInformationXML()
//        {
//            return StartProcess.
//
//                ByDoing(() =>
//                {
//
//                    var request = Connection.CreateRequest();
//                    request.Method = HttpVerb.GET;
//                    var getSerializedResponse =
//                        request.SubmitCdnRequest("?format=" +
//                                                 EnumHelper.GetDescription(Format.XML) +
//                                                 "&enabled_only=true");
//                    var xmlResponse = String.Join("",
//                                                  getSerializedResponse.ContentBody.ToArray());
//                    getSerializedResponse.Dispose();
//
//                    if (xmlResponse == null) return new XmlDocument();
//
//                    var xmlDocument = new XmlDocument();
//                    try
//                    {
//                        xmlDocument.LoadXml(xmlResponse);
//
//                    }
//                    catch (XmlException)
//                    {
//                        return xmlDocument;
//                    }
//
//                    return xmlDocument;
//                })
//                .AndIfErrorThrownIs<WebException>()
//                .Do((ex) =>
//                {
//                    var response = (HttpWebResponse)ex.Response;
//                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                        throw new UnauthorizedAccessException(
//                            "Your access credentials are invalid or have expired. ");
//                    if (response != null && response.StatusCode == HttpStatusCode.NotFound)
//                        throw new PublicContainerNotFoundException("The specified container does not exist.");
//
//
//                });
//        }
//
//
//        public string GetPublicAccountInformationJSON()
//        {
//            return StartProcess.ByDoing(() =>
//            {
//                var request = Connection.CreateRequest();
//                request.Method = HttpVerb.GET;
//                var getSerializedResponse =
//                    request.SubmitCdnRequest("?format=" +
//                                             EnumHelper.GetDescription(Format.JSON) +
//                                             "&enabled_only=true");
//
//                return string.Join("",
//                                   getSerializedResponse.ContentBody.ToArray());
//            })
//                .AndIfErrorThrownIs<WebException>()
//                .Do((ex) =>
//                {
//                    var response = (HttpWebResponse)ex.Response;
//                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                        throw new UnauthorizedAccessException(
//                            "Your access credentials are invalid or have expired. ");
//                    if (response != null && response.StatusCode == HttpStatusCode.NotFound)
//                        throw new PublicContainerNotFoundException("The specified container does not exist.");
//
//
//                });
//        }
//        /// <summary>
//        /// This method retrieves the number of storage objects in a container, and the total size, in bytes, of the container
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
//        /// IConnection connection = new Account(userCredentials);
//        /// Container container = connection.GetContainerInformation("container name");
//        /// </code>
//        /// </example>
//        /// <param name="containerName">The name of the container to query about</param>
//        /// <returns>An instance of container, with the number of storage objects contained and total byte allocation</returns>
//        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
//        public Container GetContainerInformation(string containerName)
//        {
//            Ensure.NotNullOrEmpty(containerName);
//            Ensure.ValidContainerName(containerName);
//            try
//            {
//                var authenticatedRequest = Connection.CreateRequest();
//                authenticatedRequest.Method = HttpVerb.HEAD;
//                var response = authenticatedRequest.SubmitStorageRequest(containerName);
//                var container = new Container(containerName, this)
//                {
//                    ByteCount =
//                        long.Parse(
//                        response.Headers[Constants.X_CONTAINER_BYTES_USED]),
//                    ObjectCount =
//                        long.Parse(
//                        response.Headers[
//                            Constants.X_CONTAINER_STORAGE_OBJECT_COUNT])
//                };
//                var url = getContainerCDNUri(container);
//                if (!string.IsNullOrEmpty(url))
//                    url += "/";
//                container.CdnUri = url;
//                return container;
//            }
//            catch (WebException we)
//            {
//
//
//                var response = (HttpWebResponse)we.Response;
//                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
//                    throw new ContainerNotFoundException("The requested container does not exist");
//                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
//                    throw new AuthenticationFailedException(we.Message);
//                throw;
//            }
//        }
    }
}