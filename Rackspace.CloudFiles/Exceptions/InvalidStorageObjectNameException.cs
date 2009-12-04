///
/// See COPYING file for licensing information
///

using System;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.exceptions
{
    /// <summary>
    /// Thrown when the name of the object is invalid
    /// </summary>
    public class InvalidStorageObjectNameException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public InvalidStorageObjectNameException()
            : this("Either the object name exceeds the maximum length of " + Constants.MAX_STORAGE_OBJECT_NAME_LENGTH + " or it has invalid characters (?)")
        {
        }

        /// <summary>
        /// A constructor for describing the explicit issue with the object name
        /// </summary>
        /// <param name="message">A message indicating the explicit issue with the container name.</param>
        public InvalidStorageObjectNameException(String message) : base(message)
        {
        }
    }
}