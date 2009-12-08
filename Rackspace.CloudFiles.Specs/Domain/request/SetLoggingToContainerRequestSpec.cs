using System.Net;
using NUnit.Framework;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.Request.Interfaces;
using Moq;
using Rackspace.CloudFiles.Specs.CustomAsserts;

namespace Rackspace.CloudFiles.Specs.Domain.request
{
    [TestFixture]
    public class SetLoggingToContainerRequestSpec 
    {
        #region setup infoz
        private Mock<ICloudFilesRequest> requestmock;
        private WebHeaderCollection webheaders;
        [SetUp]
        public void SetupApply(bool isenabled)
        {
            var loggingtopublicontainer = new SetLoggingToContainerRequest("fakecontainer", "http://fake", isenabled);
            requestmock = new Mock<ICloudFilesRequest>();
            webheaders = new WebHeaderCollection();
            requestmock.SetupGet(x => x.Headers).Returns(webheaders);
            loggingtopublicontainer.Apply(requestmock.Object);
        }
        #endregion

        [Test]
        public void when_creating_the_uri()
        {
            const string container = "mycontainer";
            const string url = "http://myurl.com";

            var loggingtopublicontainer = new SetLoggingToContainerRequest(container, url, true);
            var uri = loggingtopublicontainer.CreateUri();

            AssertIt.should("use a management url as the base url", () => uri.StartsWith(url));
            AssertIt.should("put public container at end of url", () => uri.EndsWith(container));
        }
        [Test]
        public void when_logging_is_not_set()
        {
            SetupApply(false);

            AssertIt.should("set method to put", () => requestmock.VerifySet(x => x.Method = "POST"));
            AssertIt.should("set X-Log-Retention to False", () => webheaders.KeyValueFor("X-Log-Retention").HasValueOf("False"));
        }
        [Test]
        public void when_logging_is_set()
        {

            SetupApply(true);

            AssertIt.should("set method to put", () => requestmock.VerifySet(x => x.Method = "POST"));
            AssertIt.should("set X-Log-Retention to True", () => webheaders.KeyValueFor("X-Log-Retention").HasValueOf("True"));
        }
    }
}