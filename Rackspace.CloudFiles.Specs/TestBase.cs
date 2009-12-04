using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rackspace.CloudFiles.domain;
using Rackspace.CloudFiles.domain.request;
using Rackspace.CloudFiles.domain.response;

namespace Rackspace.CloudFiles.Specs
{
    public class TestBase
    {
        protected string storageUrl;
        protected string authToken;

        [SetUp]
        public void SetUpBase()
        {
            Uri uri = new Uri("http://auth");

            GetAuthentication request =
                new GetAuthentication(
                    new UserCredentials(
                        uri,
                        "username",
                        "password",
                        "v1",
                        "accountname"));

            IResponse response = new GenerateRequestByType().Submit(request, authToken);
            ;

            storageUrl = response.Headers[utils.Constants.X_STORAGE_URL];
            authToken = response.Headers[utils.Constants.X_AUTH_TOKEN];
            Assert.That(authToken.Length, Is.EqualTo(32));
            SetUp();
        }

        protected virtual void SetUp()
        {
        }
    }
}