///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Domain;
using Rackspace.CloudFiles.domain.request;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.utils;

/// <example>
/// <code>
/// UserCredentials userCredentials = new UserCredentials("username", "api key");
/// IConnection connection = new Connection(userCredentials);
/// </code>
/// </example>
namespace Rackspace.CloudFiles
{
    /// <summary>
    /// enumeration of filters to place on the request url
    /// </summary>
    public enum GetItemListParameters
    {
        Limit,
        Marker,
        Prefix,
        Path
    }

    /// <summary>
    /// This class represents the primary means of interaction between a user and cloudfiles. Methods are provided representing all of the actions
    /// one can take against his/her account, such as creating containers and downloading storage objects. 
    /// </summary>
    /// <example>
    /// <code>
    /// UserCredentials userCredentials = new UserCredentials("username", "api key");
    /// IConnection connection = new Connection(userCredentials);
    /// </code>
    /// </example>
    public class Connection : IConnection
    {

	
		
		#region private fields

        private IEnsure _ensure = new Ensure();
        private bool retry;
        private List<ProgressCallback> callbackFuncs;
        private readonly GenerateRequestByType _requestfactory;
		
		private bool isNotNullOrEmpty(params string[] strings){
			foreach(var str in strings){
				if(String.IsNullOrEmpty(str))
					return false;
				
			}
			return true;
		}

        protected string CdnManagementUrl;
        protected UserCredentials _usercreds;
		#endregion
        /// <summary>
        /// A constructor used to create an instance of the Connection class
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// </code>
        /// </example>
        /// <param name="userCredentials">An instance of the UserCredentials class, containing all pertinent authentication information</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
      
		#region protected and private methods
      
        protected virtual void VerifyAuthentication()
        {
            if (!IsAuthenticated())
            {
                Authenticate();
            }
        }

  

        private void MakeStorageDirectory(string containerName, string remoteobjname )
        {
            if (string.IsNullOrEmpty(containerName) ||
                string.IsNullOrEmpty(remoteobjname))
                throw new ArgumentNullException();

         

            try
            {

                var makedirectory = new CreateStorageObjectAsDirectory(StorageUrl, containerName, remoteobjname);
                _requestfactory.Submit(makedirectory, AuthToken, _usercreds.ProxyCredentials);
            }
            catch (WebException webException)
            {
              

                var webResponse = (HttpWebResponse)webException.Response;
                if (webResponse == null) throw;
                if (webResponse.StatusCode == HttpStatusCode.BadRequest)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (webResponse.StatusCode == HttpStatusCode.PreconditionFailed)
                    throw new PreconditionFailedException(webException.Message);

                throw;
            }

		}
		private void Authenticate()
        {

		    StartProcess.
                ByDoing(() => { AuthenticateSequence(); }).
		        AndIfErrorThrownIs<Exception>().
		        Do(Nothing);
        }
        private bool IsAuthenticated()
        {
			return this.isNotNullOrEmpty(AuthToken, StorageUrl, this.CdnManagementUrl) && _usercreds != null;
        }
		private string getContainerCDNUri(Container container)
        {
            try
            {
                var public_container = GetPublicContainerInformation(container.Name);
                return public_container == null ? "" : public_container.CdnUri;
            }
            catch (ContainerNotFoundException)
            {
                return "";
            }
            catch (WebException we)
            {
               

                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationFailedException(we.Message);
                throw;
            }
        }
		private Dictionary<string, string> GetMetadata(ICloudFilesResponse getStorageItemResponse)
        {
            var metadata = new Dictionary<string, string>();
            var headers = getStorageItemResponse.Headers;
            foreach (var key in headers.AllKeys)
            {
                if (key.IndexOf(Constants.META_DATA_HEADER) > -1)
                    metadata.Add(key, headers[key]);
            }
            return metadata;
        }
		private void StoreFile(string filename, Stream contentStream)
        {
            using (var file = File.Create(filename))
            {
                contentStream.WriteTo(file);
            }
        }
		#endregion
		#region private methods to REFACTOR into a service
		private string BuildAccountJson()
		{
			string jsonResponse = "";
			var getAccountInformationJson = new GetAccountInformationSerialized(StorageUrl, Format.JSON);
            var getAccountInformationJsonResponse = _requestfactory.Submit(getAccountInformationJson, AuthToken);

            if (getAccountInformationJsonResponse.ContentBody.Count > 0)
				jsonResponse = String.Join("", getAccountInformationJsonResponse.ContentBody.ToArray());
		
            getAccountInformationJsonResponse.Dispose();
			return jsonResponse;
		}
		
		AccountInformation BuildAccount()
		{
				
			var getAccountInformation = new GetAccountInformation(StorageUrl);
            var getAccountInformationResponse = _requestfactory.Submit(getAccountInformation, AuthToken);
            return  new AccountInformation(getAccountInformationResponse.Headers[Constants.X_ACCOUNT_CONTAINER_COUNT],    getAccountInformationResponse.Headers[Constants.X_ACCOUNT_BYTES_USED]);	
			
		}
		void AuthenticateSequence()
		{
			var getAuthentication = new GetAuthentication(_usercreds);
            var getAuthenticationResponse = _requestfactory.Submit(getAuthentication);
            // var getAuthenticationResponse = getAuthentication.Apply(request);

            if (getAuthenticationResponse.Status == HttpStatusCode.NoContent)
            {
            		StorageUrl = getAuthenticationResponse.Headers[Constants.X_STORAGE_URL];
                AuthToken = getAuthenticationResponse.Headers[Constants.X_AUTH_TOKEN];
                CdnManagementUrl = getAuthenticationResponse.Headers[Constants.X_CDN_MANAGEMENT_URL];
                return;
            }

            if (!retry && getAuthenticationResponse.Status == HttpStatusCode.Unauthorized)
            {
				retry = true;
                Authenticate();
                return;
            }			
		}
		XmlDocument BuildAccountXml()
		{
		  
			var accountInformationXml = new GetAccountInformationSerialized(StorageUrl, Format.XML);
            var getAccountInformationXmlResponse = _requestfactory.Submit(accountInformationXml, AuthToken);

            if (getAccountInformationXmlResponse.ContentBody.Count == 0) 
			{
				return	new XmlDocument();
			
			}
            var contentBody = String.Join("", getAccountInformationXmlResponse.ContentBody.ToArray());
			
            getAccountInformationXmlResponse.Dispose();

            try
            {
                var doc =  new XmlDocument();
					doc.LoadXml(contentBody);
				return doc;
            }
            catch (XmlException)
            {
				return  new XmlDocument();
				
            }
			 
           
		}
		void ContainerCreation(string containername){
		
              

                var createContainer = new CreateContainer(StorageUrl, containername);
                //   var createContainerResponse = _responseFactory.Create(new CloudFilesRequest(createContainer, UserCredentials.ProxyCredentials));
                var createContainerResponse = _requestfactory.Submit(createContainer, AuthToken);
                if (createContainerResponse.Status == HttpStatusCode.Accepted)
                    throw new ContainerAlreadyExistsException("The container already exists");
		}
		void RemoveContainer (string containerName)
		{
			var deleteContainer = new DeleteContainer(StorageUrl, containerName);
                _requestfactory.Submit(deleteContainer, AuthToken, _usercreds.ProxyCredentials);
		}
		List<string> BuildContainerList ()
		{
			IList<string> containerList = new List<string>();
			 var getContainers = new GetContainers(StorageUrl);
             var getContainersResponse = _requestfactory.Submit(getContainers, AuthToken, _usercreds.ProxyCredentials);
             if (getContainersResponse.Status == HttpStatusCode.OK)
             {
			
                containerList = getContainersResponse.ContentBody;
             }
			return containerList.ToList();
		}

        static void DetermineReasonForError(WebException ex, string containername){
			var response = (HttpWebResponse)ex.Response;
             if (response != null && response.StatusCode == HttpStatusCode.NotFound)
             	throw new ContainerNotFoundException("The requested container " + containername + " does not exist");
             if (response != null && response.StatusCode == HttpStatusCode.Conflict)
             	throw new ContainerNotEmptyException("The container you are trying to delete " + containername + "is not empty");	
			
		}
		#endregion
		public Connection(UserCredentials userCreds)
        {
            _requestfactory = new GenerateRequestByType();
            callbackFuncs = new List<ProgressCallback>();
           
            AuthToken = "";
            StorageUrl = "";
            if (userCreds == null) throw new ArgumentNullException("userCredentials");

            _usercreds = userCreds;
                      
            VerifyAuthentication();
        }
		#region publicmethods

        private readonly Action<Exception> Nothing= (ex)=>{};
        public delegate void OperationCompleteCallback();

        public event OperationCompleteCallback OperationComplete;

        public delegate void ProgressCallback(int bytesWritten);
        

        public string StorageUrl
        {
            get; set;
        }

        public string AuthToken
        {
            set; get;
        }

        

        public void AddProgressWatcher(ProgressCallback progressCallback)
        {
            callbackFuncs.Add(progressCallback);
        }
        /// <summary>
        /// This method returns the number of containers and the size, in bytes, of the specified account
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// AccountInformation accountInformation = connection.GetAccountInformation();
        /// </code>
        /// </example>
        /// <returns>An instance of AccountInformation, containing the byte size and number of containers associated with this account</returns>
        public  AccountInformation GetAccountInformation()
		{

            return StartProcess.
                ByDoing<AccountInformation>(BuildAccount).
                AndIfErrorThrownIs<Exception>().
                Do(Nothing);
        }
	
        /// <summary>
        /// Get account information in json format
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// string jsonReturnValue = connection.GetAccountInformationJson();
        /// </code>
        /// </example>
        /// <returns>JSON serialized format of the account information</returns>
        public  string GetAccountInformationJson()
        {


            return StartProcess
                .ByDoing<string>(BuildAccountJson)
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing);
            
			 
        }

        /// <summary>
        /// Get account information in xml format
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// XmlDocument xmlReturnValue = connection.GetAccountInformationXml();
        /// </code>
        /// </example>
        /// <returns>XML serialized format of the account information</returns>
        public  XmlDocument GetAccountInformationXml()
        {
            return StartProcess
                .ByDoing<XmlDocument>(BuildAccountXml)
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing);
			
			 
        }

        /// <summary>
        /// This method is used to create a container on cloudfiles with a given name
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.CreateContainer("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The desired name of the container</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void CreateContainer(string containerName)
        {
			_ensure.NotNullOrEmpty(containerName);

            StartProcess
                .ByDoing(() => ContainerCreation(containerName))
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing);
        }

        /// <summary>
        /// This method is used to delete a container on cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.DeleteContainer("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to delete</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void DeleteContainer(string containerName)
        {
            _ensure.NotNullOrEmpty(containerName);
            _ensure.ValidContainerName(containerName);

            StartProcess
                .ByDoing(() => RemoveContainer(containerName))
                .AndIfErrorThrownIs<WebException>()
                .Do(ex => DetermineReasonForError(ex, containerName));
		}

		

        

        /// <summary>
        /// This method retrieves a list of containers associated with a given account
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// List{string} containers = connection.GetContainers();
        /// </code>
        /// </example>
        /// <returns>An instance of List, containing the names of the containers this account owns</returns>
        public  List<string> GetContainers()
        {


            return StartProcess
                .ByDoing<List<string>>(BuildContainerList)
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing);
			
		 
        }

        /// <summary>
        /// This method retrieves the contents of a container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// List{string} containerItemList = connection.GetContainerItemList("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container</param>
        /// <returns>An instance of List, containing the names of the storage objects in the give container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  List<string> GetContainerItemList(string containerName)
        {
            _ensure.NotNullOrEmpty(containerName);
            _ensure.ValidContainerName(containerName);

            return StartProcess
                .ByDoing(() => GetContainerItemList(containerName, null))
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing)
                ;
			 
            
        }

        /// <summary>
        /// This method ensures directory objects created for the entire path
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.MakePath("containername", "/dir1/dir2/dir3/dir4");
        /// </code>
        /// </example>
        /// <param name="containerName">The container to create the directory objects in</param>
        /// <param name="path">The path of directory objects to create</param>
        public  void MakePath(string containerName, string path)
        {
            _ensure.ValidContainerName(containerName);

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
                    MakeStorageDirectory(containerName, directory);
                    firstItem = false;
                }
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }

    
        /// <summary>
        /// This method retrieves the contents of a container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{GetItemListParameters, string} parameters = new Dictionary{GetItemListParameters, string}();
        /// parameters.Add(GetItemListParameters.Limit, 2);
        /// parameters.Add(GetItemListParameters.Marker, 1);
        /// parameters.Add(GetItemListParameters.Prefix, "a");
        /// List{string} containerItemList = connection.GetContainerItemList("container name", parameters);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container</param>
        /// <param name="parameters">Parameters to feed to the request to filter the returned list</param>
        /// <returns>An instance of List, containing the names of the storage objects in the give container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  List<string> GetContainerItemList(string containerName, Dictionary<GetItemListParameters, string> parameters)
        {
            _ensure.NotNullOrEmpty(containerName);
            _ensure.ValidContainerName(containerName);
         

            var containerItemList = new List<string>();

            try
            {
                var getContainerItemList = new GetContainerItemList(StorageUrl, containerName, parameters);
                var getContainerItemListResponse = _requestfactory.Submit(getContainerItemList, AuthToken, _usercreds.ProxyCredentials);
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
        /// This method retrieves the number of storage objects in a container, and the total size, in bytes, of the container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Container container = connection.GetContainerInformation("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to query about</param>
        /// <returns>An instance of container, with the number of storage objects contained and total byte allocation</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  Container GetContainerInformation(string containerName)
        {
            _ensure.NotNullOrEmpty(containerName);
            _ensure.ValidContainerName(containerName);

         

            try
            {
                var getContainerInformation = new GetContainerInformation(StorageUrl, containerName);
                var getContainerInformationResponse = _requestfactory.Submit(getContainerInformation, AuthToken, _usercreds.ProxyCredentials);
                var container = new Container(containerName)
                                    {
                                        ByteCount =
                                            long.Parse(
                                            getContainerInformationResponse.Headers[Constants.X_CONTAINER_BYTES_USED]),
                                        ObjectCount =
                                            long.Parse(
                                            getContainerInformationResponse.Headers[
                                                Constants.X_CONTAINER_STORAGE_OBJECT_COUNT])
                                    };
                var url = getContainerCDNUri(container);
                if (!string.IsNullOrEmpty(url))
                    url += "/";
                container.CdnUri = url;
                return container;
            }
            catch (WebException we)
            {
                

                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationFailedException(we.Message);
                throw;
            }
        }

    

        /// <summary>
        /// JSON serialized format of the container's objects
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// string jsonResponse = connection.GetContainerInformationJson("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">name of the container to get information</param>
        /// <returns>json string of object information inside the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  string GetContainerInformationJson(string containerName)
        {
            _ensure.NotNullOrEmpty(containerName);
            _ensure.ValidContainerName(containerName);
            

            

            try
            {
                var getContainerInformation = new GetContainerInformationSerialized(StorageUrl, containerName, Format.JSON);
                var getSerializedResponse = _requestfactory.Submit(getContainerInformation, AuthToken, _usercreds.ProxyCredentials);
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

        /// <summary>
        /// XML serialized format of the container's objects
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// XmlDocument xmlResponse = connection.GetContainerInformationXml("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">name of the container to get information</param>
        /// <returns>xml document of object information inside the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  XmlDocument GetContainerInformationXml(string containerName)
        {
            _ensure.NotNullOrEmpty(containerName);
            _ensure.ValidContainerName(containerName);
          

            try
            {
                var getContainerInformation = new GetContainerInformationSerialized(StorageUrl, containerName, Format.XML);
                var getSerializedResponse = _requestfactory.Submit(getContainerInformation, AuthToken, _usercreds.ProxyCredentials);
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

        /// <summary>
        /// This method uploads a storage object to cloudfiles with meta tags
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// connection.PutStorageObject("container name", "C:\Local\File\Path\file.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="localFilePath">The complete file path of the storage object to be uploaded</param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void PutStorageItem(string containerName, string localFilePath, Dictionary<string, string> metadata)
        {
            _ensure.NotNullOrEmpty(containerName, localFilePath);
            _ensure.ValidContainerName(containerName);

            try
            {
                var remoteName = Path.GetFileName(localFilePath);
                var localName = localFilePath.Replace("/", "\\");
                var putStorageItem = new PutStorageObject(StorageUrl, containerName, remoteName, localName, metadata);
                foreach (var callback in callbackFuncs)
                {
                    putStorageItem.Progress += callback;
                }
                _requestfactory.Submit(putStorageItem, AuthToken, _usercreds.ProxyCredentials);
            }
            catch (WebException webException)
            {
            

                var webResponse = (HttpWebResponse)webException.Response;
                if (webResponse == null) throw;
                if (webResponse.StatusCode == HttpStatusCode.BadRequest)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (webResponse.StatusCode == HttpStatusCode.PreconditionFailed)
                    throw new PreconditionFailedException(webException.Message);

                throw;
            }
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.PutStorageObject("container name", "C:\Local\File\Path\file.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="localFilePath">The complete file path of the storage object to be uploaded</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void PutStorageItem(string containerName, string localFilePath)
        {
            _ensure.NotNullOrEmpty(containerName, localFilePath);
            _ensure.ValidContainerName(containerName);

            PutStorageItem(containerName, localFilePath, new Dictionary<string, string>());
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles with an alternate name
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// FileInfo file = new FileInfo("C:\Local\File\Path\file.txt");
        /// connection.PutStorageObject("container name", file.Open(FileMode.Open), "RemoteFileName.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="remoteStorageItemName">The alternate name as it will be called on cloudfiles</param>
        /// <param name="storageStream">The stream representing the storage item to upload</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void PutStorageItem(string containerName, Stream storageStream, string remoteStorageItemName)
        {
            _ensure.NotNullOrEmpty(containerName, remoteStorageItemName);
            _ensure.ValidContainerName(containerName);
            PutStorageItem(containerName, storageStream, remoteStorageItemName, new Dictionary<string, string>());
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
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
        /// IConnection connection = new Connection(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.PutStorageItemAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="remoteStorageItemName">The alternate name as it will be called on cloudfiles</param>
        /// <param name="storageStream">The stream representing the storage item to upload</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void PutStorageItemAsync(string containerName, Stream storageStream, string remoteStorageItemName)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        PutStorageItem(containerName, storageStream, remoteStorageItemName);
                    }
                        finally  //Always fire the completed event
                    {
                        if (OperationComplete != null)
                        {
                            //Fire the operation complete event if there are any listeners
                            OperationComplete();
                        }
                    }
                }
                );
            thread.Start();
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
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
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// connection.PutStorageItemAsync("container name", "LocalFileName.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="localStorageItemName">The name of the file locally </param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageItemAsync(string containerName, string localStorageItemName, Dictionary<string, string> metadata)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        PutStorageItem(containerName, localStorageItemName, metadata);
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
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
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
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// FileInfo file = new FileInfo("C:\Local\File\Path\file.txt");
        /// connection.PutStorageItemAsync("container name", file.Open(FileMode.Open), "RemoteFileName.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="remoteStorageItemName">The alternate name as it will be called on cloudfiles</param>
        /// <param name="storageStream">The stream representing the storage item to upload</param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void PutStorageItemAsync(string containerName, Stream storageStream, string remoteStorageItemName, Dictionary<string, string> metadata)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        PutStorageItem(containerName, storageStream, remoteStorageItemName, metadata);
                    }
                    finally
                    {
                        if (OperationComplete != null)
                        {
                            //Fire the operation complete event if there are any listeners
                            OperationComplete();
                        }
                    }
                }
                );
            thread.Start();
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles asychronously
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
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.PutStorageItemAsync("container name", "LocalFileName.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="localStorageItemName">The name of the file locally </param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void PutStorageItemAsync(string containerName, string localStorageItemName)
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        PutStorageItem(containerName, localStorageItemName);
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
        }

        /// <summary>
        /// This method uploads a storage object to cloudfiles with an alternate name
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// FileInfo file = new FileInfo("C:\Local\File\Path\file.txt");
        /// connection.PutStorageObject("container name", file.Open(FileMode.Open), "RemoteFileName.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to put the storage object in</param>
        /// <param name="storageStream">The file stream to upload</param>
        /// <param name="metadata">An optional parameter containing a dictionary of meta tags to associate with the storage object</param>
        /// <param name="remoteStorageItemName">The name of the storage object as it will be called on cloudfiles</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void PutStorageItem(string containerName, Stream storageStream, string remoteStorageItemName, Dictionary<string, string> metadata)
        {
            _ensure.NotNullOrEmpty(containerName, remoteStorageItemName);
            _ensure.ValidContainerName(containerName);
           
            try
            {
                var putStorageItem = new PutStorageObject(StorageUrl, containerName, remoteStorageItemName, storageStream, metadata);
                foreach (var callback in callbackFuncs)
                {
                    putStorageItem.Progress += callback;
                }
                _requestfactory.Submit(putStorageItem, AuthToken, _usercreds.ProxyCredentials);
            }
            catch (WebException webException)
            {
               

                var webResponse = (HttpWebResponse)webException.Response;
                if (webResponse == null) throw;
                if (webResponse.StatusCode == HttpStatusCode.BadRequest)
                    throw new ContainerNotFoundException("The requested container does not exist");
                if (webResponse.StatusCode == HttpStatusCode.PreconditionFailed)
                    throw new PreconditionFailedException(webException.Message);

                throw;
                //following exception is cause when status code is 422 (unprocessable entity)
                //unfortunately, the HttpStatusCode enum does not have that value
                //throw new InvalidETagException("The ETag supplied in the request does not match the ETag calculated by the server");
            }
        }

        /// <summary>
        /// This method deletes a storage object in a given container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.DeleteStorageObject("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object</param>
        /// <param name="storageItemName">The name of the storage object to delete</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void DeleteStorageItem(string containerName, string storageItemName)
        {
            _ensure.NotNullOrEmpty(containerName, storageItemName);
            _ensure.ValidContainerName(containerName);

            try
            {
                var deleteStorageItem = new DeleteStorageObject(StorageUrl, containerName, storageItemName);
                _requestfactory.Submit(deleteStorageItem, AuthToken);
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
        /// This method downloads a storage object from cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <returns>An instance of StorageObject with the stream containing the bytes representing the desired storage object</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  StorageObject GetStorageObject(string containerName, string storageItemName)
        {
            if (string.IsNullOrEmpty(containerName) ||
               string.IsNullOrEmpty(storageItemName))
                throw new ArgumentNullException();

         


            return GetStorageObject(containerName, storageItemName, new Dictionary<RequestHeaderFields, string>());
        }

        /// <summary>
        /// An alternate method for downloading storage objects. This one allows specification of special HTTP 1.1 compliant GET headers
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials); 
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
        public  StorageObject GetStorageObject(string containerName, string storageObjectName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
            _ensure.NotNullOrEmpty(containerName, storageObjectName);
            _ensure.ValidContainerName(containerName);
            _ensure.ValidStorageObjectName(storageObjectName);
            try
            {
                var getStorageItem = new GetStorageObject(StorageUrl, containerName, storageObjectName, requestHeaderFields);
                var getStorageItemResponse = _requestfactory.Submit(getStorageItem, AuthToken, _usercreds.ProxyCredentials);


                var metadata = GetMetadata(getStorageItemResponse);
                var storageItem = new StorageObject(storageObjectName, metadata, getStorageItemResponse.ContentType, getStorageItemResponse.GetResponseStream(), getStorageItemResponse.ContentLength, getStorageItemResponse.LastModified);
                //                getStorageItemResponse.Dispose();
                return storageItem;
            }
            catch (WebException we)
            {
               

                var response = (HttpWebResponse)we.Response;
                response.Close();
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new StorageItemNotFoundException("The requested storage object does not exist");

                throw;
            }
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
        /// IConnection connection = new Connection(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.GetStorageItemAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The name to write the file to on your hard drive. </param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void GetStorageItemAsync(string containerName, string storageItemName, string localFileName)
        {
            var thread = new Thread(
                 () =>
                 {
                     try
                     {
                         GetStorageObject(containerName, storageItemName, localFileName);
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
        /// IConnection connection = new Connection(userCredentials);
        /// connection.AddProgressWatcher(fileTransferProgress);
        /// connection.OperationComplete += transferComplete;
        /// connection.GetStorageItemAsync("container name", "RemoteStorageItem.txt", "RemoteStorageItem.txt", requestHeaderFields);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageItemName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The name to write the file to on your hard drive. </param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void GetStorageItemAsync(string containerName, string storageItemName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {
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
        }

        /// <summary>
        /// An alternate method for downloading storage objects from cloudfiles directly to a file name specified in the method
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// StorageObject storageItem = connection.GetStorageObject("container name", "RemoteStorageItem.txt", "C:\Local\File\Path\file.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageContainerName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The file name to save the storage object into on disk</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void GetStorageObject(string containerName, string storageContainerName, string localFileName)
        {
            _ensure.NotNullOrEmpty(containerName, storageContainerName, localFileName);
            _ensure.ValidContainerName(containerName);
            _ensure.ValidStorageObjectName(storageContainerName);

            GetStorageObject(containerName, storageContainerName, localFileName, new Dictionary<RequestHeaderFields, string>());
        }

        /// <summary>
        /// An alternate method for downloading storage objects from cloudfiles directly to a file name specified in the method
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
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
        /// <param name="containerName">The name of the container that contains the storage object to retrieve</param>
        /// <param name="storageObjectName">The name of the storage object to retrieve</param>
        /// <param name="localFileName">The file name to save the storage object into on disk</param>
        /// <param name="requestHeaderFields">A dictionary containing the special headers and their values</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void GetStorageObject(string containerName, string storageObjectName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields)
        {

            _ensure.NotNullOrEmpty(containerName, storageObjectName, localFileName);
            _ensure.ValidContainerName(containerName);
            _ensure.ValidStorageObjectName(storageObjectName);

            var getStorageItem = new GetStorageObject(StorageUrl, containerName, storageObjectName, requestHeaderFields);

            try
            {
                var getStorageItemResponse = _requestfactory.Submit(getStorageItem, AuthToken, _usercreds.ProxyCredentials);
                foreach (var callback in callbackFuncs)
                {
                    getStorageItemResponse.Progress += callback;
                }
                var stream = getStorageItemResponse.GetResponseStream();

                StoreFile(localFileName, stream);
            }
            catch (WebException we)
            {
                
                HttpWebResponse response = (HttpWebResponse)we.Response;
                response.Close();
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new StorageItemNotFoundException("The requested storage object does not exist");

                throw;
            }
        }

      


        /// <summary>
        /// This method applies meta tags to a storage object on cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Dictionary{string, string} metadata = new Dictionary{string, string}();
        /// metadata.Add("key1", "value1");
        /// metadata.Add("key2", "value2");
        /// metadata.Add("key3", "value3");
        /// connection.SetStorageObjectMetaInformation("container name", "C:\Local\File\Path\file.txt", metadata);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container containing the storage object</param>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <param name="metadata">A dictionary containiner key/value pairs representing the meta data for this storage object</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void SetStorageObjectMetaInformation(string containerName, string storageObjectName, Dictionary<string, string> metadata)
        {
            _ensure.NotNullOrEmpty(containerName, storageObjectName);
            _ensure.ValidStorageObjectName(storageObjectName);
            

            try
            {
                var setStorageItemInformation = new SetStorageItemMetaInformation(StorageUrl, containerName, storageObjectName, metadata);
                _requestfactory.Submit(setStorageItemInformation, AuthToken, _usercreds.ProxyCredentials);
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
        /// This method retrieves meta information and size, in bytes, of a requested storage object
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// StorageObject storageItem = connection.GetStorageItemInformation("container name", "RemoteStorageItem.txt");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container that contains the storage object</param>
        /// <param name="storageObjectName">The name of the storage object</param>
        /// <returns>An instance of StorageObject containing the byte size and meta information associated with the container</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  StorageObjectInformation GetStorageItemInformation(string containerName, string storageObjectName)
        {
            _ensure.NotNullOrEmpty(containerName, storageObjectName);
            try
            {
                var getStorageItemInformation = new GetStorageItemInformation(StorageUrl, containerName, storageObjectName);
                var getStorageItemInformationResponse = _requestfactory.Submit(getStorageItemInformation, AuthToken, _usercreds.ProxyCredentials);


                var storageItemInformation = new StorageObjectInformation(getStorageItemInformationResponse.Headers);

                return storageItemInformation;
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
        /// This method retrieves the names of the of the containers made public on the CDN
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// List{string} containers = connection.GetPublicContainers();
        /// </code>
        /// </example>
        /// <returns>A list of the public containers</returns>
        public  List<string> GetPublicContainers()
        {
          

            try
            {
                var getPublicContainers = new GetPublicContainers(CdnManagementUrl);
                var getPublicContainersResponse = _requestfactory.Submit(getPublicContainers, AuthToken);
                var containerList = getPublicContainersResponse.ContentBody;
                getPublicContainersResponse.Dispose();

                return containerList.ToList();
            }
            catch (WebException we)
            {
               
                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationFailedException("You do not have permission to request the list of public containers.");
                throw;
            }
        }

        /// <summary>
        /// This method sets a container as public on the CDN
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Uri containerPublicUrl = connection.MarkContainerAsPublic("container name", 12345);
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to mark public</param>
        /// <param name="timeToLiveInSeconds">The maximum time (in seconds) content should be kept alive on the CDN before it checks for freshness.</param>
        /// <returns>A string representing the URL of the public container or null</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  Uri MarkContainerAsPublic(string containerName, int timeToLiveInSeconds)
        {
            _ensure.NotNullOrEmpty(containerName);

         

            try
            {
                var request = new MarkContainerAsPublic(CdnManagementUrl, containerName, timeToLiveInSeconds);
                var response = _requestfactory.Submit(request, AuthToken);

                return response == null ? null : new Uri(response.Headers[Constants.X_CDN_URI]);
            }
            catch (WebException we)
            {
              
                var response = (HttpWebResponse)we.Response;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationFailedException("You do not have permission to request the list of public containers.");
                throw;
            }
        }

        /// <summary>
        /// This method sets a container as public on the CDN
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Uri containerPublicUrl = connection.MarkContainerAsPublic("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to mark public</param>
        /// <returns>A string representing the URL of the public container or null</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  Uri MarkContainerAsPublic(string containerName)
        {
            return MarkContainerAsPublic(containerName, -1);
        }

        /// <summary>
        /// This method sets a container as private on the CDN
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// connection.MarkContainerAsPrivate("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to mark public</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  void MarkContainerAsPrivate(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();

         

            try
            {
                var request = new SetPublicContainerDetails(CdnManagementUrl, containerName,false, false, -1, "", "");
                _requestfactory.Submit(request, AuthToken);
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
        /// Retrieves a Container object containing the public CDN information
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Connection(userCredentials);
        /// Container container = connection.GetPublicContainerInformation("container name")
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to query about</param>
        /// <returns>An instance of Container with appropriate CDN information or null</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public  Container GetPublicContainerInformation(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();

          

            try
            {
                var request = new GetPublicContainerInformation(CdnManagementUrl, containerName);
                var response = _requestfactory.Submit(request, AuthToken);
                return response == null ?
                    null
                    : new Container(containerName) { CdnUri = response.Headers[Constants.X_CDN_URI], TTL = Convert.ToInt32(response.Headers[Constants.X_CDN_TTL]) };
            }
            catch (WebException ex)
            {
               

                var webResponse = (HttpWebResponse)ex.Response;
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Your authorization credentials are invalid or have expired.");
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new ContainerNotFoundException("The specified container does not exist.");
                throw;
            }
        }
        
       
       public  void SetDetailsOnPublicContainer(string publiccontainer, bool loggingenabled, int ttl, string referreracl, string useragentacl )
       {
           _ensure.NotNullOrEmpty(publiccontainer);
           _ensure.ValidContainerName(publiccontainer);

           try
           {

               var request = new SetPublicContainerDetails(CdnManagementUrl,publiccontainer,true, loggingenabled, ttl,useragentacl,referreracl);
               _requestfactory.Submit(request, AuthToken);
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

        public  XmlDocument GetPublicAccountInformationXML()
        {
            return StartProcess.
                ByDoing(() =>
                                         {
                                             var request = new GetPublicContainersInformationSerialized(
                                                 CdnManagementUrl, Format.XML);
                                             var getSerializedResponse = _requestfactory.Submit(request, AuthToken);
                                             var xmlResponse = String.Join("",
                                                                           getSerializedResponse.ContentBody.ToArray());
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
                                         })
                .AndIfErrorThrownIs<WebException>()
                .Do((ex) =>
                        {
                            var response = (HttpWebResponse) ex.Response;
                            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                                throw new UnauthorizedAccessException(
                                    "Your access credentials are invalid or have expired. ");
                            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                                throw new PublicContainerNotFoundException("The specified container does not exist.");


                        });
        }

        
        public  string GetPublicAccountInformationJSON()
        {
            return StartProcess.ByDoing(() =>
                                                    {
                                                        var request =
                                                            new GetPublicContainersInformationSerialized(
                                                                CdnManagementUrl, Format.JSON);
                                                        var getSerializedResponse = _requestfactory.Submit(request,
                                                                                                           AuthToken);
                                                        return string.Join("",
                                                                           getSerializedResponse.ContentBody.ToArray());
                                                    })
                .AndIfErrorThrownIs<WebException>()
                .Do((ex) =>
                        {
                            var response = (HttpWebResponse) ex.Response;
                            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                                throw new UnauthorizedAccessException(
                                    "Your access credentials are invalid or have expired. ");
                            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                                throw new PublicContainerNotFoundException("The specified container does not exist.");


                        });
        }

       
        #endregion
    }
}