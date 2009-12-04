using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Rackspace.CloudFiles.Specs.Domain
{
    [TestFixture]
    public class When_creating_a_container
    {
        private Container container;
        private string containerName;
        [SetUp]
        public void SetUp()
        {
            containerName = "foocontainer";
            container = new Container(containerName);
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
            container.TTL = 300;
            Assert.That(container.TTL, Is.EqualTo(300));
        }
    }
}