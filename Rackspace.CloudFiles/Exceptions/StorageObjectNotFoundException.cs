///
/// See COPYING file for licensing information
///

using System;

namespace Rackspace.CloudFiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the requested storage item does not exist on cloudfiles in the container specified 
    /// </summary>
    public class StorageObjectNotFoundException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public StorageObjectNotFoundException()
        {
        }

        /// <summary>
        /// A constructor for more explicitly describing the reason for failure
        /// </summary>
        /// <param name="msg">The message detailing the failure</param>
        public StorageObjectNotFoundException(string msg) : base(msg)
        {
        }
    }
}