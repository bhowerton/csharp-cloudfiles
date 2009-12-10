using System;
using System.Text;
using Rackspace.CloudFiles.exceptions;

namespace Rackspace.CloudFiles.utils
{
    

    public static class Ensure
    {
        public static void CanNotBeMoreThan(this int actualnumber, int limit)
        {
            if (actualnumber > limit) throw new ArgumentOutOfRangeException("the this variable must be less than " + limit + " but is " + actualnumber);
        }
        public static void NotNullOrEmpty(params string[] args)
        {
            Array.ForEach(args, s =>
                                    {
                                        if (String.IsNullOrEmpty(s))
                                            throw new ArgumentNullException();
                                    });
        }
        public static void ValidStorageObjectName(string name)
        {
             
            if (   
                name.IndexOf("?") < 0 &&
                name.Length <= Constants.MAX_STORAGE_OBJECT_NAME_LENGTH)
                return;
            throw new InvalidStorageObjectNameException();
        }
        public static void ValidContainerName(string name)
        {
            if(
                name.IndexOf("?") < 0 &&
                   name.IndexOf("/") < 0 &&
                   Encoding.Unicode.GetByteCount(name) <= Constants.MAX_CONTAINER_NAME_LENGTH
                
                )
                return;
            throw new InvalidContainerNameException();
        }
        
    }
}