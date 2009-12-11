using System;
using System.Net;
using Moq;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.Cloudfiles.Response.Interfaces;

namespace Rackspace.CloudFiles.Specs.Utils
{
    public class MockAuthenticatedRequestFactory :IAuthenticatedRequestFactory
    {
        private readonly IAuthenticatedRequest _request;

        public MockAuthenticatedRequestFactory(IAuthenticatedRequest request )
        {
            _request = request;
        }

        public IAuthenticatedRequest CreateRequest()
        {
            return _request;
        }
        
    }
    public class WebMocks
    {
        public Mock<IAuthenticatedRequest> Request { get; set; }
        public Mock<IAuthenticatedRequestFactory> Factory { get; set; }
        public Mock<ICloudFilesResponse> Response { get; set; }
    }
    public class FakeHttpResponse
    {

        public static WebMocks CreateWithResponseCode(HttpStatusCode code)
       {
           var mockrequest = new Mock<IAuthenticatedRequest>();
           var mockresponse = new Mock<ICloudFilesResponse>();
            var mockfactory = new Mock<IAuthenticatedRequestFactory>();
            mockfactory.Setup(x => x.CreateRequest()).Returns(mockrequest.Object);
           
               mockrequest.Setup(x => x.SubmitStorageRequest(It.IsAny<string>())).Returns(mockresponse.Object);
               mockrequest.Setup(x => x.SubmitCdnRequest(It.IsAny<string>())).Returns(mockresponse.Object);
               mockresponse.SetupGet(x => x.Status).Returns(code);


            return new WebMocks()
                       {
                           Factory = mockfactory,
                           Response = mockresponse,
                           Request = mockrequest
                       };
       }
    }
}