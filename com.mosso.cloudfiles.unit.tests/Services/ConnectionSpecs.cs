using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rackspace.CloudFiles.domain;

namespace Rackspace.CloudFiles.Specs.Services
{
    [TestFixture]
    public class When_instantiating_a_connection_object
    {
        [Test]
        public void Should_instantiate_engine_without_throwing_exception_when_authentication_passes()
        {
            UserCredentials userCreds = new UserCredentials(
                new Uri("http://auth"),
                "username",
                "password",
                "v1", 
                "accountname");

            MockConnection conection = new MockConnection(userCreds);

            Assert.That(conection.AuthenticationSuccessful, Is.True);
        }
    }

    internal class MockConnection : Connection
    {
        public MockConnection(UserCredentials userCreds) : base(userCreds){}

        public bool AuthenticationSuccessful { get; private set; }

        protected override void VerifyAuthentication()
        {
            AuthenticationSuccessful = true;
        }
    }
}