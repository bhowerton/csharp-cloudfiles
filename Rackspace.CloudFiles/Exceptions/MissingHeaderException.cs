using System;

namespace Rackspace.CloudFiles.exceptions
{
    public class MissingHeaderException:Exception
    {
        public MissingHeaderException(string s):base(s)
        {
        
        }
    }
}