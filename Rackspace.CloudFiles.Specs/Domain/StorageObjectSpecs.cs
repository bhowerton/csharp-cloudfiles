using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rackspace.CloudFiles.domain;

namespace Rackspace.CloudFiles.unit.tests.domain.StorageItemSpecs
{
    [TestFixture]
    public class When_creating_a_storage_object
    {
        private StorageObject _storageObject;
     
        [SetUp]
        public void SetUp()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>
                                                      {
                                                          {"key1", "value1"},
                                                          {"key2", "value2"}
                                                      };

            _storageObject = new StorageObject("objectname", metadata, "text/plain", 0, new DateTime());
        }

        [TearDown]
        public void TearDown()
        {
            _storageObject.Dispose();
        }

        [Test]
        public void Should_have_storage_object_name()
        {
            
            Assert.That(_storageObject.ObjectName, Is.EqualTo("objectname"));
        }

        [Test]
        public void Should_have_content_type()
        {
            Assert.That(_storageObject.ContentType, Is.EqualTo("text/plain"));
        }

        [Test]
        public void Should_have_file_stream()
        {
            Assert.That(_storageObject.ObjectName, Is.Not.Null);
        }
        [Test]
        public void Should_have_last_modified_time()
        {
            Assert.That(_storageObject.LastModified, Is.Not.Null);
        }
        [Test]
        public void Should_have_meta_tags()
        {
            Assert.That(_storageObject.Metadata["key1"], Is.EqualTo("value1"));
            Assert.That(_storageObject.Metadata["key2"], Is.EqualTo("value2"));
        }
    }
}