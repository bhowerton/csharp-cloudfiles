using System;
using System.Net;
using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Specs.CustomAsserts;

namespace Rackspace.CloudFiles.Specs
{
    [TestFixture]
    public class SpecContainerWhenCreatingContainer
    {
        private Mock<IAuthenticatedRequest> _request;
        private Container _container;

        [SetUp]
        public void setup()
        {
            var response = new Mock<ICloudFilesResponse>();
            var factory = new Mock<IAuthenticatedRequestFactory>();
            _request = new Mock<IAuthenticatedRequest>();
            factory.Setup(x => x.CreateRequest()).Returns(_request.Object);
             
          
            response.SetupGet(x => x.StatusCode).Returns(HttpStatusCode.Accepted);
            _request.Setup(x => x.SubmitStorageRequest("containername")).Returns(response.Object);
            var connect = new Account(factory.Object);
            _container = connect.CreateContainer("containername");
        }
        [Test]
        public void it_calls_put_verb()
        {
            _request.VerifySet(x=>x.Method=HttpVerb.PUT);
        }

        [Test]
        public void it_passes__contaier_name_to_storage_request_url()
        {
            _request.Verify(x=>x.SubmitStorageRequest("containername"));
        }

        [Test]
        public void it_returns_container_with_the_name_specified()
        {
            Assert.AreEqual("containername", _container.Name);
        }

       
    }
    [TestFixture]
    public class SpecContainerWhenCreatingContainerAndItAlreadyExists
    {
        private Account _connect;


        [SetUp]
        public void Setup()
        {
            var factory = new Mock<IAuthenticatedRequestFactory>();
            var request = new Mock<IAuthenticatedRequest>();
            var response = new Mock<ICloudFilesResponse>();
            factory.Setup(x => x.CreateRequest()).Returns(request.Object);
            response.SetupGet(x => x.StatusCode).Returns(HttpStatusCode.Accepted);
            request.Setup(x => x.SubmitStorageRequest("containername")).Returns(response.Object);
            _connect = new Account(factory.Object);
        }


        [Test] public void it_throws_containeralreadyexistsexception()
        {
            Asserts.Throws<ContainerAlreadyExistsException>(() => { _connect.CreateContainer("containername"); });
        }
    }
}