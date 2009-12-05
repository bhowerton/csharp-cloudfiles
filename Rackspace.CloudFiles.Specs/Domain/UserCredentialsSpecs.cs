using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rackspace.CloudFiles.domain;

namespace Rackspace.CloudFiles.Specs.Domain
{
    [TestFixture]
    public class When_creating_usercredentials_with_auth_url
    {
        private UserCredentials userCreds;
        private HttpProxy httpProxy;
        private Uri authUrl;

        [SetUp]
        public void SetUp()
        {
            authUrl = new Uri("http://authurl");

            httpProxy = new HttpProxy("192.1.1.2", "proxyname", "proxypass", "foo.com");

            userCreds = new UserCredentials(
                authUrl,
                "loginname",
                "loginpass",
                "v1",
                "myaccount",
                httpProxy
                );
        }

        [Test]
        public void Should_have_username()
        {
            Assert.That(userCreds.Username, Is.EqualTo("loginname"));
        }

        [Test]
        public void Should_have_password()
        {
            Assert.That(userCreds.Api_access_key, Is.EqualTo("loginpass"));
        }

        [Test]
        public void Should_have_auth_url()
        {
            Assert.That(userCreds.AuthUrl, Is.EqualTo(authUrl));
        }

        [Test]
        public void Should_have_cloud_version()
        {
            Assert.That(userCreds.Cloudversion, Is.EqualTo("v1"));
        }

        [Test]
        public void Should_have_account_name()
        {
            Assert.That(userCreds.AccountName, Is.EqualTo("loginname"));
        }

        [Test]
        public void Should_have_proxy_user_name_when_proxy_information_is_set()
        {
            Assert.That(userCreds.HttpProxy.ProxyUsername, Is.EqualTo("proxyname"));
        }

        [Test]
        public void Should_have_proxy_password_when_proxy_information_is_set()
        {
            Assert.That(userCreds.HttpProxy.ProxyPassword, Is.EqualTo("proxypass"));
        }

        [Test]
        public void Should_have_proxy_address_when_proxy_information_is_set()
        {
            Assert.That(userCreds.HttpProxy.ProxyAddress, Is.EqualTo("foo.com"));
        }

        [Test]
        public void Should_have_proxy_domain_when_proxy_information_is_set()
        {
            Assert.That(userCreds.HttpProxy.ProxyDomain, Is.EqualTo("proxydomain"));
        }
    }

    [TestFixture]
    public class When_creating_user_credentials_without_auth_url
    {
        private UserCredentials userCreds;
        private HttpProxy httpProxy;

        [SetUp]
        public void Setup()
        {
            httpProxy = new HttpProxy("myfoo",
                                      "myfoo1", 
                                      "myfoo2", 
                                      "myfoo3");

            userCreds = new UserCredentials(
                "myfoo4",
                "myfoo5",
                httpProxy
                );
        }

        [Test]
        public void Should_default_auth_url_to_mosso_api_url()
        {
            Assert.That(userCreds.AuthUrl.ToString(), Is.EqualTo(utils.Constants.AUTH_URL));
        }
    }
}