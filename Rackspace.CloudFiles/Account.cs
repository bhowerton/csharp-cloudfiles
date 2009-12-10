///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
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




        static void DetermineReasonForError(WebException ex, string containername)
        {
            var response = (HttpWebResponse)ex.Response;
            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                throw new ContainerNotFoundException("The requested container " + containername + " does not exist");
            if (response != null && response.StatusCode == HttpStatusCode.Conflict)
                throw new ContainerNotEmptyException("The container you are trying to delete " + containername + "is not empty");

        }
        #endregion
        public Account(IAuthenticatedRequestFactory authenticatedRequestFactory,long containerCount, long bytesUsed)
        {
            Connection = authenticatedRequestFactory;
            ContainerCount = containerCount;
            BytesUsed = bytesUsed;
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
        public PrivateContainer CreateContainer(string containerName)
        {
            Ensure.NotNullOrEmpty(containerName);
            Ensure.ValidContainerName(containerName);

            var request = Connection.CreateRequest();
            request.Method = HttpVerb.PUT;
            var createContainerResponse = request.SubmitStorageRequest(containerName.Encode());
            if (createContainerResponse.Status == HttpStatusCode.Accepted)
                throw new ContainerAlreadyExistsException("The container already exists");

            var headers = new ContainerHeaders(createContainerResponse.Headers);
            return new PrivateContainer(containerName, this,headers.ObjectCount, headers.BytesUsed);
        }
        private class ContainerHeaders
        {
            public long BytesUsed { get; private set; }
            public long ObjectCount { get; private set; }
            public ContainerHeaders(NameValueCollection headers)
            {
                 ObjectCount = long.Parse(headers["X-Container-Object-Count"]);
                BytesUsed= long.Parse(headers["X-Container-Bytes-Used"]);
            }
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


            var request = Connection.CreateRequest();
            request.Method = HttpVerb.DELETE;
            var response = request.SubmitStorageRequest(containerName);
            if(response.Status==HttpStatusCode.Conflict)throw new ContainerNotEmptyException();
            if (response.Status == HttpStatusCode.NoContent)throw new ContainerNotFoundException();


        }


        public IAuthenticatedRequestFactory Connection
        {
            get;
            private set;
        }

        public long ContainerCount
        {
            get; private set;
        }

        public long BytesUsed
        {
            get;
            private set;
        }
      
        public PrivateContainer GetContainer(string containerName)
        {
            Ensure.NotNullOrEmpty(containerName);
            Ensure.ValidContainerName(containerName);

            var request = Connection.CreateRequest();
            request.Method = HttpVerb.HEAD;
            var response = request.SubmitStorageRequest(containerName);
            if (response.Status == HttpStatusCode.NoContent) throw new ContainerNotFoundException();
            var containerheaders = new ContainerHeaders(response.Headers);
           
            return new PrivateContainer(containerName, this, containerheaders.ObjectCount, containerheaders.BytesUsed); 
               
        }

        public IList<PrivateContainer> GetContainers(int limit)
        {
            limit.CanNotBeMoreThan(10000);
            var request = Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            request.SubmitStorageRequest("?limit="+limit+"&format=xml");
            return new List<PrivateContainer>();
        }

        public IList<PrivateContainer> GetContainers()
        {
            var request = Connection.CreateRequest();
            request.Method = HttpVerb.GET;
            var response = request.SubmitStorageRequest("?format=xml");

            var xml = response.ContentBody.ConvertToString();
            var masterelement = XElement.Parse(xml);
            var containers = masterelement.Elements("container");
            var objects =    containers.Select(x => new PrivateContainer(
                                                                                    x.Element("name").Value,
                                                                                    this,
                                                                                    long.Parse(x.Element("count").Value),
                                                                                    long.Parse(x.Element("bytes").Value)
                                                                                    ));

            return objects.ToArray();
        }
    }
}