using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rackspace.CloudFiles.Request;

namespace Rackspace.CloudFiles.Specs.Domain.request
{
  

    [TestFixture]
    public class when_getting_information_of_a_storage_item
    {
        private GetStorageItemInformation getStorageItemInformation;

        [SetUp]
        public void setup()
        {
            getStorageItemInformation = new GetStorageItemInformation("http://storageurl", "containername", "storageitemname");
        }

        [Test]
        public void should_have_properly_formmated_request_url()
        {
            Assert.That(getStorageItemInformation.CreateUri().ToString(), Is.EqualTo("http://storageurl/containername/storageitemname"));
        }

        [Test]
        public void should_have_a_http_head_method()
        {
            Asserts.AssertMethod(getStorageItemInformation, "HEAD");
  
        }

       
    }
}