using System;
using Moq;
using Rackspace.CloudFiles.domain.request.Interfaces;
using Rackspace.CloudFiles.Request;
using SpecMaker.Core;
using SpecMaker.Core.Matchers;

namespace Rackspace.CloudFiles.Specs.Domain.request
{
    public class DeleteContainerSpecs : BaseSpec
    {
        public void when_deleting_a_container_and_storage_url_is_null()
        {
            should("throw ArgumentNullException", () => new DeleteContainer(null, "containername"),
                   typeof(ArgumentNullException));
        }
        public void when_deleting_a_container_and_storage_url_is_empty_string()
        {
            should("throw ArgumentNullException", () => new DeleteContainer("", "containername"),
                   typeof(ArgumentNullException));
        }
        public void when_deleting_a_container_and_container_name_is_null()
        {
            should("throw ArgumentNullException", () => new DeleteContainer("http://storageurl", null),
                   typeof(ArgumentNullException));
        }
        public void when_deleting_a_container_and_container_name_is_empty_string()
        {
            should("throw ArgumentNullException", () => new DeleteContainer("http://storageUrl", ""),
                   typeof(ArgumentNullException));
        }
        public void when_deleting_a_container()
        {
            var deleteContainer = new DeleteContainer("http://storageurl", "containername");
            var mockrequest = new Mock<ICloudFilesRequest>();
            deleteContainer.Apply(mockrequest.Object);
            should("have url made of storage url and container name", 
                   ()=>deleteContainer.CreateUri().ToString().Is("http://storageurl/containername"));
            should("have http delete method", ()=>
                                              mockrequest.VerifySet(x => x.Method = "DELETE")
                );
        }
    }
}