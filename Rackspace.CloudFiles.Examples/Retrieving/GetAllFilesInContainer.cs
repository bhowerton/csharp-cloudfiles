using System.Collections.Generic;
using System.IO;

namespace Rackspace.CloudFiles.Examples.Retrieving
{
    public class GetAllFilesInContainer
    {
        public void SaveToStream()
        {
            var account = Authenticate.Connection("username", "f1231faze");
            var container = account.GetContainer("MyContainer");
            var storageobjects = container.GetStorageObjects();
            const string basedir = "c:\\foo\\";
            foreach (var so in storageobjects)
            {
                using(var filestream = File.OpenWrite(basedir+so.RemoteName))
                {
                    so.Save(filestream);
                }
            }
        }
        public void SaveToDirectory()
        {

            var account = Authenticate.Connection("username", "f1231faze");
            var container = account.GetContainer("MyContainer");
            var storageobjects = container.GetStorageObjects();
            const string basedir = "c:\\foo\\";
            foreach (var so in storageobjects)
            {
                so.Save(basedir + so.RemoteName);
                
            }
        }
    }
}