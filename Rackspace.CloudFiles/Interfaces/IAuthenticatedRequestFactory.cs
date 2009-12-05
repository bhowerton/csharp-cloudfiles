namespace Rackspace.CloudFiles.Interfaces
{
    public interface IAuthenticatedRequestFactory
    {
        IAuthenticatedRequest CreateRequest();
    }
}