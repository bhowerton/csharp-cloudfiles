using System;
using System.Collections.Generic;
using System.Xml;
using Rackspace.CloudFiles.domain;

namespace Rackspace.CloudFiles.Interfaces
{
    public interface IAccount
    {
        
        IAuthenticatedRequestFactory Connection { get; }

        long BytesUsed { get; }
        long StorageObjectCount { get; }
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
        Container CreateContainer(string containerName);

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
        void DeleteContainer(string containerName);

      
    }
}