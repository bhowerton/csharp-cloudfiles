using System;
using System.Net;
using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.Request.Interfaces;
using Rackspace.CloudFiles.Specs.CustomAsserts;


namespace Rackspace.CloudFiles.Specs.Domain.request
{
    [TestFixture]
    public class DeleteStorageObjectSpecs
    {
       
        [Test]
        public void when_deleting_a_storage_item()
        {
            var deleteStorageItem = new DeleteStorageObject("http://storageurl", "containername", "storageitemname");
            var _mockrequest = new Mock<ICloudFilesRequest>();
            _mockrequest.SetupGet(x => x.Headers).Returns(new WebHeaderCollection());
            deleteStorageItem.Apply(_mockrequest.Object);
            
           AssertIt.should("start with storageurl, have container name next, and then end with the item being deleted",
                   ()=> Assert.AreEqual(deleteStorageItem.CreateUri() ,("http://storageurl/containername/storageitemname")));
            AssertIt.should("use HTTP DELETE method",()=> _mockrequest.VerifySet(x => x.Method = "DELETE"));
        }
    }
}