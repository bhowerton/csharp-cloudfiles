namespace Rackspace.CloudFiles.Examples.Retrieving
{
    public class GetAFileInContainer
    {
        public void SaveFile()
        {
            var account = Authenticate.Connection("foobar", "askljk");
            var container = account.GetContainer("MyContainer");
            var storageobject = container.GetStorageObject("Foo.txt");
            storageobject.SaveToDisk("c:\\myfoo\\Foo.txt");
        }
       
    }
}