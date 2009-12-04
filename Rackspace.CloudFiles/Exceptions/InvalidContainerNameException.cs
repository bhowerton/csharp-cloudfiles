///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.exceptions
{
    /// <summary>
    /// Thrown when the name of the container is invalid
    /// </summary>
    public class InvalidContainerNameException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public InvalidContainerNameException() : this("Either the container name exceeds the maximum length of " + Constants.MAX_CONTAINER_NAME_LENGTH + " or it has invalid characters (/ or ?)")
        {
        }

        /// <summary>
        /// A constructor for describing the explicit issue with the container name
        /// </summary>
        /// <param name="message">A message indicating the explicit issue with the container name.</param>
        public InvalidContainerNameException(String message) : base(message)
        {
        }
    }
}