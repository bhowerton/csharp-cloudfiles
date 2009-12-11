using System.IO;
using NUnit.Framework;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Specs.Utils
{
    [TestFixture]
    public class SpecStreamHelpersWhenGettingXml
    {
        [Test]
        public void it_should_retrieve_as_string()
        {
            var memorystream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(memorystream))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                writer.WriteLine("<container name=\"fakecontainer\">");
                writer.WriteLine("</container>");
                writer.Flush();
                memorystream.Seek(0, 0);
                string xml = memorystream.ConvertToString();
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                "<container name=\"fakecontainer\">\r\n" +
                                "</container>\r\n", xml);
           

            }

        }
    }
}