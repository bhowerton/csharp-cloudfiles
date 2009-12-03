using System.IO;

namespace Rackspace.CloudFiles.unit.tests.mocks
{
    public class MockResponse : Response
    {
        public MockStream Stream = new MockStream();

        public Stream GetResponseStream()
        {
            return Stream;
        }
    }
}