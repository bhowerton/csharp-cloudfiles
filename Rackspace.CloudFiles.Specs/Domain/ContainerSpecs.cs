using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rackspace.CloudFiles.Interfaces;

namespace Rackspace.CloudFiles.Specs.Domain
{
    [TestFixture]
    public class When_creating_a_container
    {
        private PublicContainer container;
        private string containerName;
        [SetUp]
        public void SetUp()
        {
            containerName = "foocontainer";
            container = new PublicContainer(containerName, new Mock<IAccount>().Object);
        }

        [Test]
        public void Should_have_container_name()
        {
            Assert.That(container.Name, Is.EqualTo(containerName));
        }

        [Test]
        public void Should_have_time_to_live_initialized_to_negative_one()
        {
            Assert.That(container.TTL, Is.EqualTo(-1));
            
        }
    }
}