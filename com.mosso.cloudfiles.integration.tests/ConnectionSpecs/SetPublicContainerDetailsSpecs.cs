using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace com.mosso.cloudfiles.integration.tests.ConnectionSpecs.SetPublicContainerDetailsSpecs
{
    [TestFixture]
    public class When_marking_a_container_as_private_and_the_container_does_not_exist : TestBase
    {
        [Test]
        public void Should_fall_through_successfully()
        {
            var containerList = connection.GetPublicContainers();
            Assert.That(containerList.Count, Is.EqualTo(0));
            connection.MarkContainerAsPrivate(Constants.CONTAINER_NAME);
        }
    }

    [TestFixture]
    public class When_marking_a_container_as_private_and_it_is_public_already : TestBase
    {
        [Test]
        public void should_remove_it_from_the_public_containers_list()
        {
            try
            {
                connection.CreateContainer(Constants.CONTAINER_NAME);
                Uri cdnUrl = connection.MarkContainerAsPublic(Constants.CONTAINER_NAME);
                Assert.That(cdnUrl, Is.Not.Null);
                Assert.That(cdnUrl.ToString().Length, Is.GreaterThan(0));

                connection.MarkContainerAsPrivate(Constants.CONTAINER_NAME);

                var publicContainers = connection.GetPublicContainers();
                Assert.That(publicContainers.Contains(Constants.CONTAINER_NAME), Is.False);
            }
            finally
            {
                connection.MarkContainerAsPrivate(Constants.CONTAINER_NAME);
                connection.DeleteContainer(Constants.CONTAINER_NAME);
            }
        }
    }
}