namespace Rackspace.CloudFiles.Examples.CDNManagement
{
    public class CDNContainerServices
    {
        public void CDNEnableContainerWithDefaults()
        {
            var account = Authenticate.Connection("fooname", "akjlafj1423");
            var cdnservice = new CdnService(account);
            var containers = cdnservice.GetContainers();
            var privatecontainer = account.GetContainer("container");
            cdnservice.MakeContainerPublic(privatecontainer);
            
        }   
        public void CDNEnableContaienrWithAllSpecifiedOption()
        {
            var account = Authenticate.Connection("fooname", "akjlafj1423");
            var cdnservice = new CdnService(account);
            var containers = cdnservice.GetContainers();
            var privatecontainer = account.GetContainer("container");
           
            const int ttl = 3600;
            const bool loggingenabled = true;
            const string useracl = "fll";
            const string referreracl = "fjk";
            cdnservice.MakeContainerPublic(privatecontainer, ttl, loggingenabled, useracl, referreracl);
            
        }
        public void CDNDisableContainer()
        {
            var account = Authenticate.Connection("foobar", "ljklj");
            var cdnservice = new CdnService(account);
            var publiccontainers = cdnservice.GetContainers();
            cdnservice.MakeContainerPrivate(publiccontainers[0]);
            
        }
    }

}