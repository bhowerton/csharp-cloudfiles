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
        
        private readonly Action<Exception> Nothing = (ex) => { };

       
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
            Ensure.ValidContainerName(containerName);
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

   



        public IAuthenticatedRequestFactory Connection
        {
            get; private set;
        }

        public long BytesUsed
        {
            get; private set;
        }
        public long StorageObjectCount
        {
            get { throw new NotImplementedException(); }
        }

       

        public PrivateContainer GetContainer(string containerName)
        {
            var request = Connection.CreateRequest();
            var response= request.SubmitStorageRequest(containerName);
            return new PrivateContainer(containerName, this);
        }
    }
}