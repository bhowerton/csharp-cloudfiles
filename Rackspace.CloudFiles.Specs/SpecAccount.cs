using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.Cloudfiles.Response.Interfaces;
using Rackspace.CloudFiles.Specs;
using Rackspace.CloudFiles.Specs.CustomAsserts;
using Rackspace.CloudFiles.Specs.Utils;

namespace Rackspace.CloudFiles.Specs
{
    public class BaseSpec
    {
        protected Account _account;
        protected WebMocks _fakehttp;

        [SetUp]
        public void Setup()
        {
            _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NoContent);
            _fakehttp.Response.SetupGet(x => x.Headers).Returns(new WebHeaderCollection()
                                                                    {
                                                                        {"X-Container-Object-Count", "7"},
                                                                        {"X-Container-Bytes-Used", "413"}
                                                                    });
            _account = new Account(_fakehttp.Factory.Object, 1, 1);
        }
    }
    [TestFixture]
    public class SpecAccountWhenCreatingNewContainer : BaseSpec
    {

        [Test]
        public void it_sets_the_put_verb_on_the_request()
        {
            _account.CreateContainer("goodname");
            _fakehttp.Request.VerifySet(x => x.Method = HttpVerb.PUT);
        }

        [Test]
        public void It_calls_the_storage_request_with_its_container_name()
        {
            _account.CreateContainer("goodname");
            _fakehttp.Request.Verify(x => x.SubmitStorageRequest("goodname"));
        }
        [Test]
        public void it_gets_the_object_count_and_bytes_used()
        {

            var container = _account.GetContainer("goodname");
            Assert.AreEqual(7, container.ObjectCount);
            Assert.AreEqual(413, container.ByteUsed);
        }


    }
    [TestFixture]
    public class SpecAccountWhenUsingContainerName
    {
        private Account _account;

        private void AssertAllThrowInvalidAccount(string containername)
        {
            Asserts.Throws<InvalidContainerNameException>(() => _account.CreateContainer(containername));
            Asserts.Throws<InvalidContainerNameException>(() => _account.DeleteContainer(containername));
            Asserts.Throws<InvalidContainerNameException>(() => _account.GetContainer(containername));
        }
        [SetUp]
        public void Setup()
        {
            var fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NoContent);
            fakehttp.Response.SetupGet(x => x.Headers).Returns(new WebHeaderCollection()
                                                                   {
                                                                       {"X-Container-Object-Count", "7"},
                                                                       {"X-Container-Bytes-Used", "413"}
                                                                   });
            _account = new Account(fakehttp.Factory.Object, 1, 1);
        }
        [Test]
        public void it_has_to_have_a_name_no_longer_than_256_bytes()
        {
            string longname = CreateString.WithLength(129);
            Assert.AreEqual(258, System.Text.Encoding.Unicode.GetByteCount(longname)); //verification
            AssertAllThrowInvalidAccount(longname);

            string shortname = CreateString.WithLength(128);
            Assert.AreEqual(256, System.Text.Encoding.Unicode.GetByteCount(shortname)); //verification
            _account.CreateContainer(shortname);
            _account.DeleteContainer(shortname);
            _account.GetContainer(shortname);

        }
        [Test]
        public void it_has_to_have_a_name_with_no_questionmarks()
        {
            string questionmarkname = "foobar?";
            AssertAllThrowInvalidAccount(questionmarkname);


        }
        [Test]
        public void it_has_to_have_a_name_with_no_forwardslashes()
        {
            string namewithslash = "foobar/";
            AssertAllThrowInvalidAccount(namewithslash);
        }

    }
    [TestFixture]
    public class SpecAccountWhenDeletingAnExistingContainer : BaseSpec
    {
        [Test]
        public void it_sets_the_delete_verb_on_the_request()
        {
            _account.DeleteContainer("goodname");
            _fakehttp.Request.VerifySet(x => x.Method = HttpVerb.DELETE);
        }

        [Test]
        public void It_calls_the_storage_request_with_its_container_name()
        {
            _account.DeleteContainer("goodname");
            _fakehttp.Request.Verify(x => x.SubmitStorageRequest("goodname"));
        }

    }
    [TestFixture]
    public class SpecAccountWhenDeletingAnExistingContainerAndItsNotEmpty
    {
        private WebMocks _webmocks;
        private Account _account;

        [SetUp]
        public void setup()
        {
            _webmocks = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.Conflict);
            _account = new Account(_webmocks.Factory.Object, 1, 1);
        }
        [Test]
        public void it_will_throw_container_not_empty_exception()
        {
            Asserts.Throws<ContainerNotEmptyException>(() => _account.DeleteContainer("goodname"));

        }

    }
    [TestFixture]
    public class SpecAccountWhenDeletingAContainerThatDoesNotExist
    {
        private Account _account;
        private WebMocks _fakehttp;

        [SetUp]
        public void setup()
        {
            _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NotFound);
            _account = new Account(_fakehttp.Factory.Object, 1, 1);

        }

        [Test]
        public void it_throws_ContainerNotFoundException()
        {
            Asserts.Throws<ContainerNotFoundException>(
                () => _account.DeleteContainer("foobar")

                );
        }

    }
    [TestFixture]
    public class SpecAccountWhenGettingAnExistingContainer : BaseSpec
    {
        [Test]
        public void it_sets_the_head_verb_on_the_request()
        {

            _account.GetContainer("goodname");
            _fakehttp.Request.VerifySet(x => x.Method = HttpVerb.HEAD);
        }

        [Test]
        public void It_calls_the_storage_request_with_its_container_name()
        {
            _account.GetContainer("goodname");
            _fakehttp.Request.Verify(x => x.SubmitStorageRequest("/goodname"));
        }
        [Test]
        public void it_gets_the_object_count_and_bytes_used()
        {

            var container = _account.GetContainer("goodname");
            Assert.AreEqual(7, container.ObjectCount);
            Assert.AreEqual(413, container.ByteUsed);
        }

    }
    [TestFixture]
    public class SpecAccountWhenGettingAContainerThatDoesNotExist
    {
        private Account _account;
        private WebMocks _fakehttp;

        [SetUp]
        public void setup()
        {
            _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NotFound);
            _account = new Account(_fakehttp.Factory.Object, 1, 1);
        }

        [Test]
        public void it_throws_ContainerNotFoundException()
        {
            Asserts.Throws<ContainerNotFoundException>(() => _account.GetContainer("foobar"));
        }

    }
    [TestFixture]
    public class SpecAccountWhenGettingAContainerList
    {
        private Mock<IAuthenticatedRequestFactory> _mockfactory;
        private Account _account;
        private MockAuthenticatedRequest _request;
        private Mock<ICloudFilesResponse> _response;

        [SetUp]
        public void setup()
        {
            _mockfactory = new Mock<IAuthenticatedRequestFactory>();
        
           
            _response = new Mock<ICloudFilesResponse>();
            _request = new MockAuthenticatedRequest(_response.Object);
            _mockfactory.Setup(j => j.CreateRequest()).Returns(_request);

            var readerwriter = new Mock<IHttpReaderWriter>();
            readerwriter.Setup(x => x.GetStringFromStream(It.IsAny<HttpWebResponse>())).Returns(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <account name=\"MichaelBarton\">" +
                "<container>" +
                "<name>test_container_1</name>" +
                "<count>2</count>" +
                "<bytes>78</bytes>" +
                "</container>" +
                "<container>" +
                "<name>test_container_2</name>" +
                "<count>1</count>" +
                "<bytes>17</bytes>" +
                "</container>" +
                "</account>");    
            _response.SetupGet(x => x.Status).Returns(HttpStatusCode.OK);
            _account = new Account(readerwriter.Object ,_mockfactory.Object, 1, 1);
        }

        [Test]
        public void it_uses_a_format_of_xml()
        {

            _account.GetContainers();
          //  _request.Verify(x => x.SubmitStorageRequest("?format=xml",
            //    It.IsAny<Action<HttpWebRequest>>()
              //  , It.IsAny<Action<HttpWebResponse>>()));
        }
        [Test]
        public void it_should_return_all_containers_in_account()
        {

//            var xmllines = TextStreamFactory.MakeFromString(
//                "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <account name=\"MichaelBarton\">" +
//                "<container>" +
//                "<name>test_container_1</name>" +
//                "<count>2</count>" +
//                "<bytes>78</bytes>" +
//                "</container>" +
//                "<container>" +
//                "<name>test_container_2</name>" +
//                "<count>1</count>" +
//                "<bytes>17</bytes>" +
//                "</container>" +
//                "</account>"
//                );



            var containers = _account.GetContainers();
            Assert.AreEqual(2, containers.Count);
            Assert.AreEqual("test_container_1", containers[0].Name);
            Assert.AreEqual(2, containers[0].ObjectCount);
            Assert.AreEqual(78, containers[0].ByteUsed);

            Assert.AreEqual("test_container_2", containers[1].Name);
            Assert.AreEqual(1, containers[1].ObjectCount);
            Assert.AreEqual(17, containers[1].ByteUsed);

        }


    }
    [TestFixture]
    public class SpecAccountWhenGettingAContainerListAndLimitingResults : BaseSpec
    {
        [Test]
        public void it_will_not_allow_you_to_request_more_than_10000()
        {
            int limit = 20000;
            Asserts.Throws<ArgumentOutOfRangeException>(() => _account.GetContainers(limit));
        }
        [Test]
        public void it_uses_a_format_of_xml()
        {
            _account.GetContainers(123);
            _fakehttp.Request.Verify(x => x.SubmitStorageRequest("?limit=123&format=xml"));
        }

    }
}