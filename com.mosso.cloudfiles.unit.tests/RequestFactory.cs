using System.IO;

namespace Rackspace.CloudFiles.unit.tests
{
    public interface RequestFactory
    {
        Request Create(string uri);
    }

    public interface Request
    {
        Stream GetRequestStream();
        Response GetResponse();
    }

    public interface Response
    {
        Stream GetResponseStream();
    }
}