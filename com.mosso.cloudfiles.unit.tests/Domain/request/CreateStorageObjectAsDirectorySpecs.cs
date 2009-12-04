using System;
using System.IO;
using Rackspace.CloudFiles.unit.tests.CustomMatchers;
using Moq;
using Rackspace.CloudFiles.domain.request;
using Rackspace.CloudFiles.domain.request.Interfaces;
using SpecMaker.Core;
using SpecMaker.Core.Matchers;

namespace Rackspace.CloudFiles.Specs.Domain.request
{
    public class CreateStorageObjectAsDirectorySpecs:BaseSpec
    {
        public void when_adding_storage_object()
        {


            var createContainer = new CreateStorageObjectAsDirectory("http://storageurl", "containername", "objname");
            var mock = new Mock<ICloudFilesRequest>();
            createContainer.Apply(mock.Object);


            should("append container and object name to storage url",
                   () => createContainer.CreateUri().ToString().Is("http://storageurl/containername/objname"));
            should("use PUT method", () =>
                                     mock.VerifySet(x => x.Method = "PUT")
                );
            should("have content type of application/directory", () =>
                                                                 mock.VerifySet(x => x.ContentType = "application/directory")
                );
            should("set content with basic empty object", () =>
                                                          mock.Verify(x => x.SetContent(It.IsAny<MemoryStream>(), It.IsAny<Connection.ProgressCallback>()))
                );
        }

        public void when_creating_uri_and_storage_item_has_forward_slashes_at_the_beginning()
        {

            var item = new CreateStorageObjectAsDirectory("http://storeme", "itemcont", "/dir1/dir2");
            Uri url = item.CreateUri();
            should("remove all forward slashes", () => url.EndsWith("dir1/dir2"));
        }
    }
}