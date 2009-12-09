using System;
using System.Net;
using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.domain.response.Interfaces;
using Rackspace.CloudFiles.exceptions;
using Rackspace.CloudFiles.Interfaces;
using Rackspace.CloudFiles.Specs.Utils;

namespace Rackspace.CloudFiles.Specs
{
    [TestFixture]
    public class SpecAccountWhenCreatingNewContainer
    {
        private Account _account;
        private WebMocks _fakehttp;


        [SetUp]
        public void Setup()
        {
             _fakehttp = FakeHttpResponse.CreateWithResponseCode(HttpStatusCode.OK);
            _account = new Account(_fakehttp.Factory.Object);
        }
        [Test]
        public void it_has_to_have_a_name_no_longer_than_256_bytes()
        {
            string longname = CreateString.WithLength(129);
            Assert.AreEqual(258, System.Text.Encoding.Unicode.GetByteCount(longname)); //verification
            try
            {
                _account.CreateContainer(longname);
                Assert.Fail();
            }
            catch (InvalidContainerNameException)
            {

            }
            string shortname = CreateString.WithLength(128);
            Assert.AreEqual(256, System.Text.Encoding.Unicode.GetByteCount(shortname)); //verification
            _account.CreateContainer(shortname);

        }
        [Test]
        public void it_has_to_have_a_name_with_no_questionmarks()
        {
            
            try
            {
                _account.CreateContainer("foobar?");
                Assert.Fail();
            }
            catch (InvalidContainerNameException)
            {

            }
        }
        [Test]
        public void it_has_to_have_a_name_with_no_forwardslashes()
        {

            try
            {
                _account.CreateContainer("foobar/");
                Assert.Fail();
            }
            catch (InvalidContainerNameException)
            {

            }
        }
        [Test]
        public void it_sets_the_put_verb_on_the_request()
        {
            _account.CreateContainer("goodname");
            _fakehttp.Request.VerifySet(x=>x.Method=HttpVerb.PUT);
        }
    }
}