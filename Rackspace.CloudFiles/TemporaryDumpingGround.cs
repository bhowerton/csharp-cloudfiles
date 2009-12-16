using System;
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
      //  public StorageObjectInformation GetStorageItemInformation(string storageObjectName)
        //{
            //May not need this one, moving it over, but it will likely be replaced
          //  throw new NotImplementedException();
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
//                    throw new StorageObjectNotFoundException("The requested storage object does not exist");
//
//                throw;
//            }
   

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
//                    BytesUsed =
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
        
//
//        private Dictionary<string, string> GetMetadata(ICloudFilesResponse getStorageItemResponse)
//        {
//            var metadata = new Dictionary<string, string>();
//            var headers = getStorageItemResponse.Headers;
//            foreach (var key in headers.AllKeys)
//            {
//                if (key.IndexOf(Constants.META_DATA_HEADER) > -1)
//                    metadata.Add(key, headers[key]);
//            }
//            return metadata;
//        }
//        private static void StoreFile(string filename, Stream contentStream)
//        {
//            using (var file = File.Create(filename))
//            {
//                contentStream.WriteTo(file);
//            }
//        }
//        #endregion
//        #region private methods to REFACTOR into a service
//        private string BuildAccountJson()
//        {
//            string jsonResponse = "";
//            var request = Connection.CreateRequest();
//            request.Method = HttpVerb.GET;
//            var response = request.SubmitStorageRequest("?format=" + EnumHelper.GetDescription(Format.JSON));
//            if (response.ContentBody.Count > 0)
//                jsonResponse = String.Join("", response.ContentBody.ToArray());
//
//            response.Dispose();
//            return jsonResponse;
//        }
//
//        AccountInformation BuildAccount()
//        {
//
//            var request = Connection.CreateRequest();
//            request.Method = HttpVerb.HEAD;
//            var getAccountInformationResponse = request.SubmitStorageRequest("/");
//            return new AccountInformation(getAccountInformationResponse.Headers[Constants.X_ACCOUNT_CONTAINER_COUNT], getAccountInformationResponse.Headers[Constants.X_ACCOUNT_BYTES_USED]);
//
//        }
//
//        XmlDocument BuildAccountXml()
//        {
//
//            //	var accountInformationXml = new GetAccountInformationSerialized(StorageUrl, Format.XML);
//            ///    var getAccountInformationXmlResponse = _requestfactory.Submit(accountInformationXml, AuthToken);
//            var request = Connection.CreateRequest();
//            request.Method = HttpVerb.GET;
//            var getAccountInformationXmlResponse = request.SubmitStorageRequest("?format=" + EnumHelper.GetDescription(Format.XML));
//            if (getAccountInformationXmlResponse.ContentBody.Count == 0)
//            {
//                return new XmlDocument();
//
//            }
//            var contentBody = String.Join("", getAccountInformationXmlResponse.ContentBody.ToArray());
//
//            getAccountInformationXmlResponse.Dispose();
//
//            try
//            {
//                var doc = new XmlDocument();
//                doc.LoadXml(contentBody);
//                return doc;
//            }
//            catch (XmlException)
//            {
//                return new XmlDocument();
//
//            }
//
//
//        }
//        List<string> BuildContainerList()
//        {
//            IList<string> containerList = new List<string>();
//            var request = Connection.CreateRequest();
//            request.Method = HttpVerb.GET;
//            var getContainersResponse = request.SubmitStorageRequest("");
//            if (getContainersResponse.Status == HttpStatusCode.OK)
//            {
//
//                containerList = getContainersResponse.ContentBody;
//            }
//            return containerList.ToList();
//        }

//        public class GetPublicContainers : IAddToWebRequest
//        {
//            private readonly string _cdnManagementUrl;
//
//            public GetPublicContainers(string cdnManagementUrl)
//            {
//                _cdnManagementUrl = cdnManagementUrl;
//                if (string.IsNullOrEmpty(cdnManagementUrl))
//                    throw new ArgumentNullException();
//            }
//
//            public Uri CreateUri()
//            {
//                return new Uri(_cdnManagementUrl + "?enabled_only=true");
//            }
//
//            public void Apply(ICloudFilesRequest request)
//            {
//                request.Method = "GET";
//
//            }
////        }
//        public class SetLoggingToContainerRequest : IAddToWebRequest
//        {
//            private readonly string _publiccontainer;
//            private readonly string _cdnManagmentUrl;
//            private readonly bool _loggingenabled;
//
//            public SetLoggingToContainerRequest(string publiccontainer, string cdnManagmentUrl, bool loggingenabled)
//            {
//                _publiccontainer = publiccontainer;
//                _cdnManagmentUrl = cdnManagmentUrl;
//                _loggingenabled = loggingenabled;
//
//            }
//
//            public Uri CreateUri()
//            {
//                return new Uri(_cdnManagmentUrl + "/" + _publiccontainer.Encode());
//            }
//
//            public void Apply(ICloudFilesRequest request)
//            {
//                request.Method = "POST";
//                string enabled = "False";
//                if (_loggingenabled)
//                    enabled = "True";
//                request.Headers.Add("X-Log-Retention", enabled);
//
//            }
//        }
//        public class SetPublicContainerDetails : IAddToWebRequest
//        {
//            private readonly string _cdnManagementUrl;
//            private readonly string _containerName;
//            private readonly bool _isCdnEnabled;
//            private readonly bool _isLoggingEnabled;
//            private readonly int _timeToLiveInSeconds;
//            private readonly string _agentacl;
//            private readonly string _refacl;
//
//            /// <summary>
//            /// Assigns various details to containers already publicly available on the CDN
//            /// </summary>
//            /// <param name="cdnManagementUrl">The CDN URL</param>
//            /// <param name="containerName">The name of the container to update the details for</param>
//            /// <param name="isCdnEnabled">Sets whether or not specified container is available on the CDN</param>
//            /// <param name="timeToLiveInSeconds"></param>
//            public SetPublicContainerDetails(string cdnManagementUrl, string containerName, bool isCdnEnabled, bool isLoggingEnabled, int timeToLiveInSeconds, string agentacl, string refacl)
//            {
//
//                if (String.IsNullOrEmpty(cdnManagementUrl) ||
//                    String.IsNullOrEmpty(containerName))
//                    throw new ArgumentNullException();
//                _cdnManagementUrl = cdnManagementUrl;
//                _containerName = containerName;
//                _isCdnEnabled = isCdnEnabled;
//                _isLoggingEnabled = isLoggingEnabled;
//                _timeToLiveInSeconds = timeToLiveInSeconds;
//                _agentacl = agentacl;
//                _refacl = refacl;
//            }
//
//            public Uri CreateUri()
//            {
//                return new Uri(_cdnManagementUrl + "/" + _containerName.Encode());
//            }
//
//            public void Apply(ICloudFilesRequest request)
//            {
//                request.Method = "POST";
//                request.Headers.Add(Constants.X_CDN_ENABLED, _isCdnEnabled.Capitalize());
//                request.Headers.Add(Constants.X_LOG_RETENTION, _isLoggingEnabled.Capitalize());
//                if (_timeToLiveInSeconds > -1) request.Headers.Add(Constants.X_CDN_TTL, _timeToLiveInSeconds.ToString());
//                if (!String.IsNullOrEmpty(_agentacl)) request.Headers.Add(Constants.X_USER_AGENT_ACL, _agentacl);
//                if (!String.IsNullOrEmpty(_refacl)) request.Headers.Add(Constants.X_REFERRER_ACL, _refacl);
//
//            }
//        }
    }
}