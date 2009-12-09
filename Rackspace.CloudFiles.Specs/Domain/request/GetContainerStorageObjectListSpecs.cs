using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rackspace.CloudFiles.Request;
using Rackspace.CloudFiles.Request.Interfaces;
using Moq;
using Rackspace.CloudFiles.Specs.CustomAsserts;


namespace Rackspace.CloudFiles.Specs.Domain.request
{
    [TestFixture]
    public class GentContainerStorageObjectListSpec 
    {
       [Test]
        public void when_getting_a_list_of_items_in_a_container_with_query_parameters()
        {
            var getContainerItemList = new GetContainerItemList("http://storageurl", "containername");
            var uri = getContainerItemList.CreateUri();
            var _mockrequest = new Mock<ICloudFilesRequest>();
            getContainerItemList.Apply(_mockrequest.Object);
            AssertIt.should("url should have storage url at beginning ", () => uri.StartsWith("http://storageurl"));
            AssertIt.should("url should have container name at the end ", () => uri.EndsWith("containername"));
            AssertIt.should("use HTTP GET method", () => _mockrequest.VerifySet(x => x.Method = "GET"));
        }
        [Test]
        public void when_getting_a_list_of_items_in_a_container_with_limit_query_parameter()
        {
            var parameters = new Dictionary<GetItemListParameters, string> { { GetItemListParameters.Limit, "2" } };
            Uri uri;
            Mock<ICloudFilesRequest> _mockrequest = GetMockrequest(parameters, out uri);

            AssertIt.should("url should have storage url at beginning ", () => uri.StartsWith("http://storageurl"));
            AssertIt.should("url should have container name followed by query string and limit at the end ",
                   () => uri.EndsWith("containername?limit=2"));
            AssertIt.should("use HTTP GET method", () => _mockrequest.VerifySet(x => x.Method = "GET"));
        }
        [Test]
        public void when_getting_a_list_of_items_in_a_container_with_marker_query_parameter()
        {
            var parameters = new Dictionary<GetItemListParameters, string> { { GetItemListParameters.Marker, "abc" } };
            Uri uri;
            Mock<ICloudFilesRequest> _mockrequest = GetMockrequest(parameters, out uri);

           AssertIt. should("have url with storage url at beginning ", () => uri.StartsWith("http://storageurl"));
           AssertIt.should("have url with container name followed by query string with marker at the end ",
                   () => uri.EndsWith("containername?marker=abc"));
           AssertIt. should("use HTTP GET method", () => _mockrequest.VerifySet(x => x.Method = "GET"));
        }
        [Test]
        public void when_getting_a_list_of_items_in_a_container_with_prefix_query_parameter()
        {
            var parameters = new Dictionary<GetItemListParameters, string> { { GetItemListParameters.Prefix, "a" } };
            Uri uri;
            Mock<ICloudFilesRequest> mockrequest = GetMockrequest(parameters, out uri);

            AssertIt.should("have url with storage url at beginning ", () => uri.StartsWith("http://storageurl"));
            AssertIt.should("have url with container name followed by query string with prefix at the end ",
                   () => uri.EndsWith("containername?prefix=a"));
            AssertIt.should("use HTTP GET method", () => mockrequest.VerifySet(x => x.Method = "GET"));
        }
        [Test]
        public void when_getting_a_list_of_items_in_a_container_with_path_query_parameter()
        {
            var parameters = new Dictionary<GetItemListParameters, string> { { GetItemListParameters.Path, "dir1/subdir2/" } };
      
            Uri uri;
            Mock<ICloudFilesRequest> _mockrequest = GetMockrequest(parameters, out uri);

            AssertIt.should("have url with storage url at beginning ", () => uri.StartsWith("http://storageurl"));
            AssertIt.should("have url with container name followed by query string with path at the end ",
                   () => uri.EndsWith("containername?path=dir1/subdir2/"));
            AssertIt.should("use HTTP GET method", () => _mockrequest.VerifySet(x => x.Method = "GET"));
        }
        [Test]
        public void when_getting_a_list_of_items_in_a_container_with_more_than_one_query_parameter()
        {
            var parameters = new Dictionary<GetItemListParameters, string>
                                 {
                                     { GetItemListParameters.Limit, "2" },
                                     { GetItemListParameters.Marker, "abc" },
                                     { GetItemListParameters.Prefix, "a" },
                                     { GetItemListParameters.Path, "dir1/subdir2/" }
                                 };
            Uri uri;
            Mock<ICloudFilesRequest> _mockrequest = GetMockrequest(parameters, out uri);

            AssertIt.should("have url with storage url at beginning ", () => uri.StartsWith("http://storageurl"));
            AssertIt.should("have url with container name followed by query strings ",
                   () => uri.EndsWith("containername?limit=2&marker=abc&prefix=a&path=dir1/subdir2/"));
            AssertIt.should("use HTTP GET method", () => _mockrequest.VerifySet(x => x.Method = "GET"));
            
        }
        #region privates
        private Mock<ICloudFilesRequest> GetMockrequest(Dictionary<GetItemListParameters, string> parameters, out Uri uri)
        {
            var getContainerItemList = new GetContainerItemList("http://storageurl", "containername", parameters);
            var _mockrequest = new Mock<ICloudFilesRequest>();
            getContainerItemList.Apply(_mockrequest.Object);
            uri = getContainerItemList.CreateUri();
            return _mockrequest;
        }
        #endregion
    }
}