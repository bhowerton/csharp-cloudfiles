///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.Domain;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.utils;

/// <example>
/// <code>
/// UserCredentials userCredentials = new UserCredentials("username", "api key");
/// IConnection connection = new Account(userCredentials);
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
    /// IAccount connection = new Account(userCredentials);
    /// </code>
    /// </example>
    public class Account : IAccount
    {
       

        #region protected and private methods



        //private bool IsAuthenticated()
        //{
        //	return this.isNotNullOrEmpty(AuthToken, StorageUrl, this.CdnManagementUrl) && _usercreds != null;
        //}
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
        private static void StoreFile(string filename, Stream contentStream)
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
            var request = Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            var response = request.SubmitStorageRequest("?format=" + EnumHelper.GetDescription(Format.JSON));
            if (response.ContentBody.Count > 0)
                jsonResponse = String.Join("", response.ContentBody.ToArray());

            response.Dispose();
            return jsonResponse;
        }

        AccountInformation BuildAccount()
        {

            var request = Connection.CreateRequest();
            request.Method = HttpVerb.HEAD;
            var getAccountInformationResponse = request.SubmitStorageRequest("/");
            return new AccountInformation(getAccountInformationResponse.Headers[Constants.X_ACCOUNT_CONTAINER_COUNT], getAccountInformationResponse.Headers[Constants.X_ACCOUNT_BYTES_USED]);

        }

        XmlDocument BuildAccountXml()
        {

            //	var accountInformationXml = new GetAccountInformationSerialized(StorageUrl, Format.XML);
            ///    var getAccountInformationXmlResponse = _requestfactory.Submit(accountInformationXml, AuthToken);
            var request = Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            var getAccountInformationXmlResponse = request.SubmitStorageRequest("?format=" + EnumHelper.GetDescription(Format.XML));
            if (getAccountInformationXmlResponse.ContentBody.Count == 0)
            {
                return new XmlDocument();

            }
            var contentBody = String.Join("", getAccountInformationXmlResponse.ContentBody.ToArray());

            getAccountInformationXmlResponse.Dispose();

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(contentBody);
                return doc;
            }
            catch (XmlException)
            {
                return new XmlDocument();

            }


        }
        List<string> BuildContainerList()
        {
            IList<string> containerList = new List<string>();
            var request = Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            var getContainersResponse = request.SubmitStorageRequest("");
            if (getContainersResponse.Status == HttpStatusCode.OK)
            {

                containerList = getContainersResponse.ContentBody;
            }
            return containerList.ToList();
        }

        static void DetermineReasonForError(WebException ex, string containername)
        {
            var response = (HttpWebResponse)ex.Response;
            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                throw new ContainerNotFoundException("The requested container " + containername + " does not exist");
            if (response != null && response.StatusCode == HttpStatusCode.Conflict)
                throw new ContainerNotEmptyException("The container you are trying to delete " + containername + "is not empty");

        }
        #endregion
        public Account(IAuthenticatedRequestFactory authenticatedRequestFactory)
        {
            Connection = authenticatedRequestFactory;

        }
        #region publicmethods

        private readonly Action<Exception> Nothing = (ex) => { };







        public string StorageUrl
        {
            get;
            set;
        }

        public string AuthToken
        {
            set;
            get;
        }




        /// <summary>
        /// This method returns the number of containers and the size, in bytes, of the specified account
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// AccountInformation accountInformation = connection.GetAccountInformation();
        /// </code>
        /// </example>
        /// <returns>An instance of AccountInformation, containing the byte size and number of containers associated with this account</returns>
        public AccountInformation GetAccountInformation()
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
        /// IConnection connection = new Account(userCredentials);
        /// string jsonReturnValue = connection.GetAccountInformationJson();
        /// </code>
        /// </example>
        /// <returns>JSON serialized format of the account information</returns>
        public string GetAccountInformationJson()
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
        /// IConnection connection = new Account(userCredentials);
        /// XmlDocument xmlReturnValue = connection.GetAccountInformationXml();
        /// </code>
        /// </example>
        /// <returns>XML serialized format of the account information</returns>
        public XmlDocument GetAccountInformationXml()
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
        /// IConnection connection = new Account(userCredentials);
        /// connection.CreateContainer("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The desired name of the container</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public Container CreateContainer(string containerName)
        {
            Ensure.NotNullOrEmpty(containerName);

            StartProcess
                .ByDoing(() =>
                             {
                                 var request = Connection.CreateRequest();
                                 request.Method = HttpVerb.PUT;
                                 var createContainerResponse = request.SubmitStorageRequest(containerName.Encode());
                                 if (createContainerResponse.Status == HttpStatusCode.Accepted)
                                     throw new ContainerAlreadyExistsException("The container already exists");

                             })
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing);
            return new Container(containerName, this);
        }

        /// <summary>
        /// This method is used to delete a container on cloudfiles
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// connection.DeleteContainer("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to delete</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public void DeleteContainer(string containerName)
        {
            Ensure.NotNullOrEmpty(containerName);
            Ensure.ValidContainerName(containerName);

            StartProcess
                .ByDoing(() =>
                             {
                                 var request = Connection.CreateRequest();
                                 request.Method = HttpVerb.DELETE;
                                 request.SubmitStorageRequest(containerName.Encode());
                             }
                )
                .AndIfErrorThrownIs<WebException>()
                .Do(ex => DetermineReasonForError(ex, containerName));
        }

        /// <summary>
        /// This method retrieves a list of containers associated with a given account
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// List{string} containers = connection.GetContainers();
        /// </code>
        /// </example>
        /// <returns>An instance of List, containing the names of the containers this account owns</returns>
        public List<string> GetContainers()
        {


            return StartProcess
                .ByDoing<List<string>>(BuildContainerList)
                .AndIfErrorThrownIs<Exception>()
                .Do(Nothing);


        }

        /// <summary>
        /// This method retrieves the names of the of the containers made public on the CDN
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// List{string} containers = connection.GetPublicContainers();
        /// </code>
        /// </example>
        /// <returns>A list of the public containers</returns>
        public List<string> GetPublicContainers()
        {
            try
            {
                var request = Connection.CreateRequest();
                var getPublicContainersResponse = request.SubmitCdnRequest("?enabled_only=true");
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
        /// Retrieves a Container object containing the public CDN information
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Container container = connection.GetPublicContainerInformation("container name")
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to query about</param>
        /// <returns>An instance of Container with appropriate CDN information or null</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public Container GetPublicContainerInformation(string containerName)
        {
            Ensure.NotNullOrEmpty(containerName);
            Ensure.ValidContainerName(containerName);
            try
            {
                var request = Connection.CreateRequest();
                request.Method = HttpVerb.HEAD;
                var response = request.SubmitCdnRequest(containerName.Encode() + "?enabled_only=true");
                return response == null ?
                    null
                    : new Container(containerName, this) { CdnUri = response.Headers[Constants.X_CDN_URI], TTL = Convert.ToInt32(response.Headers[Constants.X_CDN_TTL]) };
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




        public XmlDocument GetPublicAccountInformationXML()
        {
            return StartProcess.
                ByDoing(() =>
                                         {

                                             var request = Connection.CreateRequest();
                                             request.Method = HttpVerb.GET;
                                             var getSerializedResponse =
                                                 request.SubmitCdnRequest("?format=" +
                                                                          EnumHelper.GetDescription(Format.XML) +
                                                                          "&enabled_only=true");
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
                            var response = (HttpWebResponse)ex.Response;
                            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                                throw new UnauthorizedAccessException(
                                    "Your access credentials are invalid or have expired. ");
                            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                                throw new PublicContainerNotFoundException("The specified container does not exist.");


                        });
        }

        
        public string GetPublicAccountInformationJSON()
        {
            return StartProcess.ByDoing(() =>
                                                    {
                                                        var request = Connection.CreateRequest();
                                                        request.Method = HttpVerb.GET;
                                                        var getSerializedResponse =
                                                            request.SubmitCdnRequest("?format=" +
                                                                                     EnumHelper.GetDescription(Format.JSON) +
                                                                                     "&enabled_only=true");
                                                       
                                                        return string.Join("",
                                                                           getSerializedResponse.ContentBody.ToArray());
                                                    })
                .AndIfErrorThrownIs<WebException>()
                .Do((ex) =>
                        {
                            var response = (HttpWebResponse)ex.Response;
                            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                                throw new UnauthorizedAccessException(
                                    "Your access credentials are invalid or have expired. ");
                            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                                throw new PublicContainerNotFoundException("The specified container does not exist.");


                        });
        }
        /// <summary>
        /// This method retrieves the number of storage objects in a container, and the total size, in bytes, of the container
        /// </summary>
        /// <example>
        /// <code>
        /// UserCredentials userCredentials = new UserCredentials("username", "api key");
        /// IConnection connection = new Account(userCredentials);
        /// Container container = connection.GetContainerInformation("container name");
        /// </code>
        /// </example>
        /// <param name="containerName">The name of the container to query about</param>
        /// <returns>An instance of container, with the number of storage objects contained and total byte allocation</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public Container GetContainerInformation(string containerName)
        {
            Ensure.NotNullOrEmpty(containerName);
            Ensure.ValidContainerName(containerName);
            try
            {
                var authenticatedRequest = Connection.CreateRequest();
                authenticatedRequest.Method = HttpVerb.HEAD;
                var response = authenticatedRequest.SubmitStorageRequest(containerName);
                var container = new Container(containerName, this)
                {
                    ByteCount =
                        long.Parse(
                        response.Headers[Constants.X_CONTAINER_BYTES_USED]),
                    ObjectCount =
                        long.Parse(
                        response.Headers[
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


        public IAuthenticatedRequestFactory Connection
        {
            get; private set;
        }

        #endregion
    }
}