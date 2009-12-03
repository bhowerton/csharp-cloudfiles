///
/// See COPYING file for licensing information
///

using System;

namespace Rackspace.CloudFiles.exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NoContainersFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public NoContainersFoundException(string msg) : base(msg)
        {
        }
    }
}