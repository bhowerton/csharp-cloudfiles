using System;
using Rackspace.CloudFiles.exceptions;

namespace Rackspace.CloudFiles.utils
{
    public interface IEnsure
    {
        void NotNullOrEmpty(params string[] args);
        void ValidStorageObjectName(string name);
        void ValidContainerName(string name);
    }

    public class Ensure : IEnsure
    {
        public void NotNullOrEmpty(params string[] args)
        {
            Array.ForEach(args, s =>
                                    {
                                        if (String.IsNullOrEmpty(s))
                                            throw new ArgumentNullException();
                                    });
                   
                                             
                                  
        }
        public void ValidStorageObjectName(string name)
        {
             
            if (   
                name.IndexOf("?") < 0 &&
                name.Length <= Constants.MAX_STORAGE_OBJECT_NAME_LENGTH)
                return;
            throw new InvalidStorageObjectNameException();
        }
        public  void ValidContainerName(string name)
        {
            if(
                name.IndexOf("?") < 0 &&
                   name.IndexOf("/") < 0 &&
                   name.Length <= Constants.MAX_CONTAINER_NAME_LENGTH
                
                )
                return;
            throw new InvalidContainerNameException();
        }
        
    }
}