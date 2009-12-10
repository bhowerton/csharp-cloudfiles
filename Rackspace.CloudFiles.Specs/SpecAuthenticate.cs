using System;
using System.Collections.Generic;
using System.Net;
using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.utils;

namespace Rackspace.CloudFiles.Specs
{
    [TestFixture]
    public class SpecAuthenticateWhenCreatingAccountConnection
    {
        private Mock<IAuthorizationRequest> _mockrequest;
        private Account _acct;
        private Mock<IAuthenticatedRequestFactory> _mockauthrequestfactory;
        private Mock<IAuthenticatedRequest> _mockrauthedrequest;
        private IDictionary<string,string> _webheaders;
        private Mock<ICloudFilesResponse> _authenticatedresponse;

        [SetUp]
        public void setup()
        {
            setupMocks();
            setupAuthenticateRequest();
            setupAccountRequest();

            var authenticate = new Authenticate(_mockrequest.Object);
            _acct = authenticate.ConnectToAccount("foobar", "fookey");
        }

        private void setupMocks()
        {
            _mockrequest = new Mock<IAuthorizationRequest>();
            _mockauthrequestfactory = new Mock<IAuthenticatedRequestFactory>();
            _mockrauthedrequest = new Mock<IAuthenticatedRequest>();
            _authenticatedresponse = new Mock<ICloudFilesResponse>();
            _webheaders = new Dictionary<string,string>();
        }
        private void setupAuthenticateRequest()
        {
            _mockrequest.SetupGet(x => x.Headers).Returns(_webheaders);
            _mockrequest.Setup(x => x.Submit()).Returns(_mockauthrequestfactory.Object);
        }
        private void setupAccountRequest()
        {
            _mockauthrequestfactory.Setup(x => x.CreateRequest()).Returns(_mockrauthedrequest.Object);
            _mockrauthedrequest.Setup(x => x.SubmitStorageRequest("")).Returns(_authenticatedresponse.Object);
            var webheaders = new WebHeaderCollection();
            webheaders.Add(Constants.X_ACCOUNT_CONTAINER_COUNT, "1");
            webheaders.Add(Constants.X_ACCOUNT_BYTES_USED, "70");
            _authenticatedresponse.SetupGet(x => x.Headers).Returns(webheaders);
        }
        [Test]
        public void its_authenticate_request_should_pass_username_in_headers()
        {
            Assert.AreEqual("foobar", _webheaders["X-Auth-User"]);
            
        }
        [Test]
        public void its_authenticate_request_should_pass_apikey_in_headers()
        {
            Assert.AreEqual("fookey", _webheaders["X-Auth-Key"]);
        }
        [Test]
        public void its_authenticate_request_should_use_rackspace_authentication_url()
        {
            _mockrequest.VerifySet(x=>x.Url="https://auth.api.rackspacecloud.com/v1.0");
        }
        [Test]
        public void its_authenticate_request_should_use_the_get_method()
        {
            _mockrequest.VerifySet(x=>x.Method="GET");
        }
        [Test]
        public void it_should_initalize_account_with_byte_used()
        {
            Assert.AreEqual(70, _acct.BytesUsed);
        }
        [Test]
        public void it_should_initialize_account_with_container_count()
        {
            Assert.AreEqual(1, _acct.ContainerCount);
        }
        [Test]
        public void it_should_submit_storage_url_on_account_creation()
        {
            _mockrauthedrequest.Verify(x=>x.SubmitStorageRequest(""));
        }
        [Test]
        public void it_should_use_head_method_on_account_creation()
        {
            _mockrauthedrequest.VerifySet(x=>x.Method=HttpVerb.HEAD);
        }
       
    }
}