///
/// See COPYING file for licensing information
///

using System;

namespace Rackspace.CloudFiles.exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class PreconditionFailedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public PreconditionFailedException(string msg) : base(msg)
        {
        }
    }
}