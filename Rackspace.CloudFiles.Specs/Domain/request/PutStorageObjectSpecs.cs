using System;
using System.IO;
using Rackspace.CloudFiles.unit.tests.CustomMatchers;
using Moq;
using Rackspace.CloudFiles.domain.request;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.exceptions;
using SpecMaker.Core;
using SpecMaker.Core.Matchers;

namespace Rackspace.CloudFiles.Specs.Domain.request
{
    public class PutStorageObjectSpec : BaseSpec
    {
        private const int MAX_OBJECT_NAME_LENGTH = 256;
        private const int MAX_CONTAINER_NAME_LENGTH = 1024;
        public void when_creating_uri()
        {
            should("start with storage url passed from constructor", RuleIs.Pending);
            should("have container name in the middle", RuleIs.Pending);
            should("have storage item at end", RuleIs.Pending);
        }
        public void when_creating_uri_and_storage_item_has_forward_slashes_at_the_beginning()
        {
            var memstream = new MemoryStream();
            PutStorageObject storageobject = new PutStorageObject("http://storeme", "itemcont", "/stuffhacks.txt", memstream);
            Uri url = storageobject.CreateUri();
            should("remove all slashes", ()=> url.EndsWith("stuffhacks.txt"));
        }
        public void when_putting_a_storage_item_via_local_file_path_and_the_local_file_does_not_exist()
        {
            var mock = new Mock<ICloudFilesRequest>();
            should("throw FileNotFoundException", () => new PutStorageObject("a", "a", "a", "a").Apply(mock.Object), typeof(FileNotFoundException));
        }
        public void when_putting_a_storage_item_via_local_file_path_and_the_container_name_exceeds_the_maximum_length()
        {
            should("throw ContainerNameException", ()=> 
                                                   new PutStorageObject("a", new string('a', MAX_CONTAINER_NAME_LENGTH + 1), "a", "a")
                   ,typeof(ContainerNameException));
        }
        public void when_putting_a_storage_item_via_stream_and_the_container_name_exceeds_the_maximum_length()
        {
            var s = new MemoryStream(new byte[0]);
            should("throw ContainerNameException",()=>
                                                  new PutStorageObject("a", new string('a', MAX_CONTAINER_NAME_LENGTH + 1), "a", s)
                   ,typeof(ContainerNameException));
        }
        public void when_putting_a_storage_item_via_local_file_path_and_the_storage_item_name_exceeds_the_maximum_length()
        {
            should("throw ContainerNameException",
                   () => new PutStorageObject("a", "a", new string('a', MAX_OBJECT_NAME_LENGTH + 1), "a"),
                   typeof(StorageItemNameException)  );
        }
        public void when_putting_a_storage_item_via_stream_and_the_storage_item_name_exceeds_the_maximum_length()
        {
            var s = new MemoryStream(new byte[0]);
            should("throw ContainerNameException",()=>
                                                  new PutStorageObject("a", "a", new string('a', MAX_OBJECT_NAME_LENGTH + 1), s),
                   typeof(StorageItemNameException));
        }
    }
}