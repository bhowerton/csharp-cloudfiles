using NUnit.Framework;
using Rackspace.CloudFiles.Specs.Utils;
using System.Net;
using System.Collections.Generic;
using System;
namespace Rackspace.CloudFiles.Specs
{
   	[TestFixture]
	public class SpecContainerWhenGettingListOfObjects{
		
		private WebMocks _fakehttp;
		private Container _container;
		private IList<StorageObject> _objects;
		
		[SetUp]
		public void setup()
		{
		
			_fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.OK);
			_fakehttp.Response.SetupGet(x=>x.ContentBody).Returns(new []{"<?xml version=\"1.0\" encoding=\"UTF-8\"?> <container name=\"test_container_1\">",
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
																		</container>"});
			var acct = new Account(_fakehttp.Factory.Object, 1,89);
			_container = new Container("foobar",acct, 1,12);
			_objects = _container.GetStorageObjects();
			
		
		}
		[Test]
		public void should_use_get_method()
		{
		
			_fakehttp.Request.VerifySet(x=>x.Method=HttpVerb.GET);
		
		}
		
		[Test]
		public void should_submit_storage_request_url_with_container_name()
		{
		
			 _fakehttp.Request.Verify(x=>x.SubmitStorageRequest("foobar"));
		
		}
		
		[Test]
		public void it_returns_objects_from_response()
		{
		
			Assert.AreEqual(2, _objects.Count);
			Assert.AreEqual("test_object_1",_objects[0].RemoteName );
			Assert.AreEqual("4281c348eaf83e70ddce0e07221c3d28", _objects[0].ETag);
			Assert.AreEqual(14,_objects[0].ContentLength );
			Assert.AreEqual("application/octet-stream", _objects[0].ContentType);
			Assert.AreEqual(new DateTime(2009,2, 3,5,26,32,612), _objects[0].LastModified);
		}
		
	
	}
   
}