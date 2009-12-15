using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.Cloudfiles.Response.Interfaces;
using Rackspace.CloudFiles.Specs.CustomAsserts;
using Rackspace.CloudFiles.Specs.Utils;
using System.Net;
using System.Collections.Generic;
using System;

namespace Rackspace.CloudFiles.Specs
{
    [TestFixture]
    public class SpecContainerWhenGettingListOfObjects
    {

       
        private Container _container;
        private IList<StorageObject> _objects;
        private Mock<IAuthenticatedRequestFactory> _mockfactory;
        private Mock<ICloudFilesResponse> _mockresponse;
        private MockAuthenticatedRequest _mockrequest;

        [SetUp]
        public void setup()
        {


            var mockhttpreaderwriter = new Mock<IHttpReaderWriter>();

            mockhttpreaderwriter.Setup(x => x.GetStringFromStream(It.IsAny<HttpWebResponse>())).Returns(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <container name=\"test_container_1\">" +
                @"<object> 
				    <name>test_object_1</name> 
				    <hash>4281c348eaf83e70ddce0e07221c3d28</hash> 
				    <bytes>14</bytes> 
				    <content_type>application/octet-stream</content_type> 	
				    <last_modified>2009-02-03T05:26:32.612278</last_modified>
			    </object> 
			    <object>
				    <name>test_object_2</name> 
				    <hash>b039efe731ad111bc1b0ef221c3849d0</hash> 
				    <bytes>64</bytes> 
				    <content_type>application/octet-stream</content_type> 
				    <last_modified>2009-02-03T05:26:32.612278</last_modified>
			    </object> 
        		</container>");
            _mockfactory = new Mock<IAuthenticatedRequestFactory>();
            _mockresponse = new Mock<ICloudFilesResponse>();
            _mockrequest = new MockAuthenticatedRequest(_mockresponse.Object);
            _mockfactory.Setup(x => x.CreateRequest()).Returns(_mockrequest);
            _mockresponse.Setup(x => x.Status).Returns(HttpStatusCode.OK);

            var acct = new Account(mockhttpreaderwriter.Object , _mockfactory.Object, 1, 89);
            _container = new Container( "foobar", mockhttpreaderwriter.Object,acct, 1, 12);
            _objects = _container.GetStorageObjects();


        }
        [Test]
        public void should_use_get_method()
        {

            Assert.AreEqual(_mockrequest.Method ,HttpVerb.GET);

        }

        [Test]
        public void should_submit_storage_request_url_with_container_name()
        {

            Assert.AreEqual(_mockrequest.StorageUrlsPassed[0], "/foobar");

        }

        [Test]
        public void it_returns_objects_from_response()
        {

            Assert.AreEqual(2, _objects.Count);
            Assert.AreEqual("test_object_1", _objects[0].RemoteName);
            Assert.AreEqual("4281c348eaf83e70ddce0e07221c3d28", _objects[0].ETag);
            Assert.AreEqual(14, _objects[0].ContentLength);
            Assert.AreEqual("application/octet-stream", _objects[0].ContentType);
            DateTime datetime = DateTime.Parse("2/3/2009 5:26:32.612278");

            Assert.AreEqual(datetime, _objects[0].LastModified);
        }


    }
    [TestFixture]
    public class SpecContainerWhenGettingListOfObjectsAndThereAreNoObjectsInTheContainer
    {

        
        private Container _container;
        private IList<StorageObject> _objects;

        [SetUp]
        public void setup()
        {
            var httpreaderwriter = new Mock<IHttpReaderWriter>();
            httpreaderwriter.Setup(x => x.GetStringFromStream(It.IsAny<HttpWebResponse>())).Returns(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <container name=\"test_container_1\">" +
            @"</container>");
            var mockresponse = new Mock<ICloudFilesResponse>();
            var mockrequest = new MockAuthenticatedRequest(mockresponse.Object);
            var mockfactory = new Mock<IAuthenticatedRequestFactory>();
            mockfactory.Setup(x => x.CreateRequest()).Returns(mockrequest);
            
            var acct = new Account(httpreaderwriter.Object, mockfactory.Object, 1, 89);
            _container = new Container( "foobar", httpreaderwriter.Object,acct, 1, 12);
            _objects = _container.GetStorageObjects();
        }

        [Test]
        public void it_has_count_of_0()
        {
            Assert.AreEqual(0, _objects.Count);
        }

    }

    [TestFixture]
    public class SpecContainerWhenGettingIndividualObject
    {
        private WebMocks _fakehttp;
        private Container _container;
        private StorageObject _object;
        [SetUp]
        public void Setup()
        {
            _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NoContent);
            var account = new Account(_fakehttp.Factory.Object, 1, 21);
            _container = new Container("foobar", account, 1, 21);
            var responseheaders = new WebHeaderCollection();

            _fakehttp.Response.SetupGet(x => x.ETag).Returns("8a964ee2a5e88be344f36c22562a6486");
            _fakehttp.Response.SetupGet(x => x.Headers).Returns(responseheaders);
            _fakehttp.Response.SetupGet(x => x.ContentLength).Returns(51200);
            _fakehttp.Response.SetupGet(x => x.LastModified).Returns(new DateTime(2009, 1, 1, 1, 1, 1, 1));
            _fakehttp.Response.SetupGet(x => x.ContentType).Returns("text/plain; charset=UTF-8");
            _object = _container.GetStorageObject("foobar.txt");



        }

        [Test]
        public void it_retrieves_etag()
        {
            Assert.AreEqual("8a964ee2a5e88be344f36c22562a6486", _object.ETag);
        }
        [Test]
        public void it_retrieves_contentlength()
        {
            Assert.AreEqual(51200, _object.ContentLength);
        }
        [Test]
        public void it_retrieves_lastmodified()
        {
            Assert.AreEqual(new DateTime(2009, 1, 1, 1, 1, 1, 1), _object.LastModified);
        }
        [Test]
        public void it_retrieves_contenttype()
        {
            Assert.AreEqual("text/plain; charset=UTF-8", _object.ContentType);
        }
        [Test]
        public void it_passes_head_method()
        {
            _fakehttp.Request.VerifySet(x => x.Method = HttpVerb.HEAD);
        }

        [Test]
        public void it_passes_container_name_and_storage_name_to_url()
        {
            _fakehttp.Request.Verify(x => x.SubmitStorageRequest("/foobar/foobar.txt"));
        }

    }
    [TestFixture]
    public class SpecContainerWhenGettingIndividualObjectAndItsNotThere
    {
        private WebMocks _fakehttp;
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NotFound);
            _container = new Container("foo", new Account(_fakehttp.Factory.Object, 1, 12), 1, 12);
        }

        [Test]
        public void it_throws_object_not_found_exception()
        {
            Asserts.Throws<StorageObjectNotFoundException>(() => _container.GetStorageObject("myfoo.txt"));
        }

    }
    [TestFixture]
    public class SpecContainerWhenGettingIndividualObjectAndAnotherStatusCodeOccurs
    {
        private WebMocks _fakehttp;
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.NotImplemented);
            _container = new Container("foo", new Account(_fakehttp.Factory.Object, 1, 12), 1, 12);
        }

        [Test]
        public void it_throws_object_not_found_exception()
        {
            Asserts.Throws<Exception>(() => _container.GetStorageObject("myfoo.txt"));
        }

    }
    [TestFixture]
    public class SpecContainerWhenMakingDirectoryPath
    {
        private WebMocks _webmocks;
        private Container _container;

        [SetUp]
        public void setup()
        {
            _webmocks = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.Created);
            _webmocks.Request.Setup(
                x =>
                x.SubmitStorageRequest(It.IsAny<string>(), It.IsAny<Action<HttpWebRequest>>(),
                                       It.IsAny<Action<HttpWebResponse>>())).Returns(_webmocks.Response.Object);
            _container = new Container("container", new Account(_webmocks.Factory.Object, 1, 2), 1, 2);
            _container.MakePath("/foobar/myfoo/myfooworld");
        }

        [Test]
        public void it_uses_PUT_method()
        {
            _webmocks.Request.VerifySet(x => x.Method = HttpVerb.PUT);
        }

        [Test]
        public void it_runs_a_request_for_each_path()
        {
           
            _webmocks.Request.Verify(x => x.SubmitStorageRequest("/container/foobar", It.IsAny<Action<HttpWebRequest>>(), It.IsAny<Action<HttpWebResponse>>()), Times.Once());

            _webmocks.Request.Verify(x => x.SubmitStorageRequest("/container/foobar%2fmyfoo", It.IsAny<Action<HttpWebRequest>>(), It.IsAny<Action<HttpWebResponse>>()), Times.Once());
            _webmocks.Request.Verify(x => x.SubmitStorageRequest("/container/foobar%2fmyfoo%2fmyfooworld", It.IsAny<Action<HttpWebRequest>>(), It.IsAny<Action<HttpWebResponse>>()), Times.Once());
        }

        [Test]
        public void it_sets_content_type_to_application_dir()
        {
            _webmocks.Request.VerifySet(x => x.ContentType = "application/directory");
        }


    }
}